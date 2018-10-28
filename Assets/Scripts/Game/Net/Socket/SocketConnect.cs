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
	protected DoubleBuffer<byte[]> mOutputList; // 使用双缓冲提高发送消息的效率
	protected DoubleBuffer<INPUT_ELEMENT> mRecieveList;
	protected int mHeartBeatTimes;
	protected float mHeartBeatTimeCount = 0.0f;
	protected float mHeartBeatMaxTime = 0.0f;
	protected byte[] mRecvBuff;
	public SocketConnect(string name)
		: base(name)
	{
		mMaxReceiveCount = 8 * 1024;
		mOutputList = new DoubleBuffer<byte[]>();
		mRecieveList = new DoubleBuffer<INPUT_ELEMENT>();
		mReceiveThread = new CustomThread("SocketReceive");
		mSendThread = new CustomThread("SocketSend");
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
		catch (Exception e)
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
		writeInt(packetData, ref index, (int)(packet.getPacketType()));
		writeInt(packetData, ref index, packet.getSize());
		if (packet.getSize() > 0)
		{
			packet.write(packetData, GameDefine.PACKET_HEADER_SIZE);
		}
		// 添加到输出缓冲区
		mOutputList.addToBuffer(packetData);
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
		// 解析所有已经收到的消息包
		var readList = mRecieveList.getReadList();
		foreach (var item in readList)
		{
			SocketPacket packetReply = mSocketManager.createPacket(item.mType);
			packetReply.setConnect(this);
			packetReply.read(item.mData);
			packetReply.execute();
		}
		readList.Clear();
	}
	// 发送Socket消息
	protected void sendSocket(ref bool run)
	{
		if (mServerSocket == null)
		{
			run = false;
			return;
		}

		try
		{
			var readList = mOutputList.getReadList();
			foreach (var item in readList)
			{
				int sendCount = mServerSocket.Send(item, item.Length, SocketFlags.None);
				if (sendCount != item.Length)
				{
					logError("发送失败");
				}
			}
			readList.Clear();
		}
		catch (SocketException)
		{
			run = false;
			return;
		}
	}
	// 接收Socket消息
	protected void receiveSocket(ref bool run)
	{
		if (mServerSocket == null)
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
				if (cmd != null)
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
				PACKET_TYPE type = (PACKET_TYPE)readInt(mRecvBuff, ref index);
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
					int realDataSize = readInt(mRecvBuff, ref index);
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
					if (packetSize != 0)
					{
						byte[] recvData = new byte[packetSize];
						// 读取消息内容(byte[])
						readBytes(mRecvBuff, ref index, recvData);
						mRecieveList.addToBuffer(new INPUT_ELEMENT(type, recvData));
					}
					else
					{
						mRecieveList.addToBuffer(new INPUT_ELEMENT(type, null));
					}
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