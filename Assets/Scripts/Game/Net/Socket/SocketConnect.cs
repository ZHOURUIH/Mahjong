using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

public class SocketConnect : CommandReceiver
{
	protected int mMaxReceiveCount;
	protected Socket mServerSocket;
	protected CustomThread mReceiveThread;
	protected CustomThread mSendThread;
	protected ThreadLock mOutputLock;
	protected ThreadLock mReceiveLock;
	protected List<byte[]>[] mOutputList;	// 使用双缓冲提高发送消息的效率
	protected int mOutputWriteIndex;
	protected int mOutputReadIndex;
	protected List<INPUT_ELEMENT>[] mRecieveList;
	protected int mReceiveWriteIndex;
	protected int mReceiveReadIndex;
	protected int mHeartBeatTimes;
	protected float mHeartBeatTimeCount = 0.0f;
	protected float mHeartBeatMaxTime = 0.0f;
	protected byte[] mRecvBuff;
	public SocketConnect(string name)
		:base(name)
	{
		mMaxReceiveCount = 8 * 1024;
		mOutputList = new List<byte[]>[2];
		mOutputList[0] = new List<byte[]>();
		mOutputList[1] = new List<byte[]>();
		mOutputWriteIndex = 0;
		mOutputReadIndex = 1;
		mRecieveList = new List<INPUT_ELEMENT>[2];
		mRecieveList[0] = new List<INPUT_ELEMENT>();
		mRecieveList[1] = new List<INPUT_ELEMENT>();
		mReceiveWriteIndex = 0;
		mOutputReadIndex = 1;
		mReceiveThread = new CustomThread("SocketReceive");
		mSendThread = new CustomThread("SocketSend");
		mReceiveLock = new ThreadLock();
		mOutputLock = new ThreadLock();
		mRecvBuff = new byte[mMaxReceiveCount];
	}
	public void init(IPAddress ip, int port)
	{
		try
		{
			mHeartBeatMaxTime = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_HEART_BEAT_NITERVAL);
			// 创建socket  
			mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			mServerSocket.Connect(ip, port);
		}
		catch(Exception e)
		{
			logInfo("init socket exception : " + e.Message + ", stack : " + e.StackTrace, LOG_LEVEL.LL_FORCE);
			mServerSocket = null;
			CommandSocketConnectNetState cmd = newCmd(out cmd);
			cmd.mNetState = NET_STATE.NS_NET_CLOSE;
			pushCommand(cmd, this);
			return;
		}
		mSendThread.start(sendSocket);
		mReceiveThread.start(receiveSocket);
	}
	public void update(float elapsedTime)
	{
		if (mHeartBeatTimeCount >= 0.0f)
		{
			mHeartBeatTimeCount += elapsedTime;
			if (mHeartBeatTimeCount >= mHeartBeatMaxTime)
			{
				heartBeat();
				// 停止计时,等待服务器确认心跳后再开始计时
				mHeartBeatTimeCount = -1.0f;
			}
		}
		processInput();
	}
	public override void destroy()
	{
		if (mServerSocket != null)
		{
			mServerSocket.Close();
			mServerSocket = null;
		}
		mSendThread.destroy();
		mReceiveThread.destroy();
	}
	public void sendMessage<T>() where T : SocketPacket
	{
		sendMessage(mSocketManager.createPacket<T>());
	}
	public void sendMessage(SocketPacket packet)
	{
		if (mServerSocket == null)
		{
			return;
		}
		// 将消息包中的数据准备好,然后放入发送列表中
		// 前四个字节分别是两个short,代表消息类型和消息内容长度
		byte[] packetData = new byte[GameDefine.PACKET_HEADER_SIZE + packet.getSize()];
		int index = 0;
		// 消息类型
		BinaryUtility.writeInt(packetData, ref index, (int)(packet.getPacketType()));
		// 消息长度
		BinaryUtility.writeInt(packetData, ref index, packet.getSize());
		if (packet.getSize() > 0)
		{
			// 消息内容
			packet.write(packetData, GameDefine.PACKET_HEADER_SIZE);
		}
		mOutputLock.waitForUnlock();
		// 添加到写缓冲中
		mOutputList[mOutputWriteIndex].Add(packetData);
		mOutputLock.unlock();
	}
	public void notifyHeartBeatRet(int heartBeatTimes)
	{
		if (heartBeatTimes != mHeartBeatTimes)
		{
			string info = "心跳错误!";
			GameUtility.messageOK(info);
			UnityUtility.logError(info);
		}
		else
		{
			mHeartBeatTimeCount = 0.0f;
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------
	// 处理接收到的所有消息
	protected void processInput()
	{
		// 交换读写缓冲区
		mReceiveLock.waitForUnlock();
		MathUtility.swap(ref mReceiveWriteIndex, ref mReceiveReadIndex);
		mReceiveLock.unlock();
		// 解析所有已经收到的消息包
		int receiveCount = mRecieveList[mReceiveReadIndex].Count;
		for (int i = 0; i < receiveCount; ++i)
		{
			INPUT_ELEMENT element = mRecieveList[mReceiveReadIndex][i];
			SocketPacket packetReply = mSocketManager.createPacket(element.mType);
			packetReply.setConnect(this);
			packetReply.read(element.mData);
			packetReply.execute();
		}
		mRecieveList[mReceiveReadIndex].Clear();
	}
	// 发送Socket消息
	protected void sendSocket(ref bool run)
	{
		if (mServerSocket == null)
		{
			run = false;
			return;
		}
		// 交换读写缓冲区
		mOutputLock.waitForUnlock();
		MathUtility.swap(ref mOutputWriteIndex, ref mOutputReadIndex);
		mOutputLock.unlock();
		try
		{
			int dataCount = mOutputList[mOutputReadIndex].Count;
			for (int i = 0; i < dataCount; ++i)
			{
				int sendCount = mServerSocket.Send(mOutputList[mOutputReadIndex][i], mOutputList[mOutputReadIndex][i].Length, SocketFlags.None);
				if(sendCount != mOutputList[mOutputReadIndex][i].Length)
				{
					logError("发送失败");
				}
			}
			mOutputList[mOutputReadIndex].Clear();
		}
		catch(SocketException)
		{
			run = false;
			return;
		}
	}
	// 接收Socket消息
	protected void receiveSocket(ref bool run)
	{
		if(mServerSocket == null)
		{
			run = false;
			return;
		}
		try
		{
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
			EndPoint ep = endpoint;
			int nRecv = mServerSocket.ReceiveFrom(mRecvBuff, ref ep);
			if (nRecv <= 0)
			{
				CommandSocketConnectNetState cmd = newCmd(out cmd, true, true);
				if(cmd != null)
				{
					cmd.mNetState = NET_STATE.NS_NET_CLOSE;
					pushDelayCommand(cmd, this);
				}
				run = false;
				return;
			}
			int index = 0;
			while (true)
			{
				if (index + GameDefine.PACKET_HEADER_SIZE > nRecv)
				{
					break;
				}
				// 读取包类型
				PACKET_TYPE type = (PACKET_TYPE)BinaryUtility.readInt(mRecvBuff, ref index);
				// 客户端接收到的必须是SC类型的
				if (type <= PACKET_TYPE.PT_SC_MIN || type >= PACKET_TYPE.PT_SC_MAX)
				{
					string info = "packet type error : " + type;
					GameUtility.messageOK(info, true);
					UnityUtility.logError(info, false);
					break;
				}
				int packetSize = mSocketManager.getPacketSize(type);
				if (packetSize >= 0)
				{
					// 读取消息长度
					int realDataSize = BinaryUtility.readInt(mRecvBuff, ref index);
					if (realDataSize != packetSize)
					{
						string info = "wrong packet size! type : " + type + ", readed : " + realDataSize + ", packet size : " + packetSize;
						GameUtility.messageOK(info, true);
						UnityUtility.logError(info, false);
						break;
					}
					if (packetSize > nRecv - GameDefine.PACKET_HEADER_SIZE)
					{
						string info = "wrong packet data! packet : " + type + ", need size : " + packetSize + ", receive size : " + (nRecv - sizeof(PACKET_TYPE));
						GameUtility.messageOK(info, true);
						UnityUtility.logError(info, false);
						break;
					}
					mReceiveLock.waitForUnlock();
					if (packetSize != 0)
					{
						byte[] recvData = new byte[packetSize];
						// 读取消息内容(byte[])
						BinaryUtility.readBytes(mRecvBuff, ref index, recvData);
						mRecieveList[mReceiveWriteIndex].Add(new INPUT_ELEMENT(type, recvData));
					}
					else
					{
						mRecieveList[mReceiveWriteIndex].Add(new INPUT_ELEMENT(type, null));
					}
					mReceiveLock.unlock();
					// 该段消息内存已经解析完了
					if (index == nRecv)
					{
						break;
					}
				}
				// 如果消息解析发生错误,则不再解析
				else
				{
					break;
				}
			}
		}
		catch (SocketException)
		{
			CommandSocketConnectNetState cmd = newCmd(out cmd, true, true);
			if (cmd != null)
			{
				cmd.mNetState = NET_STATE.NS_SERVER_CLOSE;
				pushDelayCommand(cmd, this);
			}
			run = false;
			return;
		}
	}
	protected void heartBeat()
	{
		CSHeartBeat beat = mSocketManager.createPacket<CSHeartBeat>();
		beat.setHeartBeatTimes(++mHeartBeatTimes);
		sendMessage(beat);
		logInfo("客户端心跳 : " + mHeartBeatTimes, LOG_LEVEL.LL_FORCE);
	}
}