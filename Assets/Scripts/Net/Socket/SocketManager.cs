using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

public class OUTPUT_STREAM
{
	public OUTPUT_STREAM(byte[] data, int dataSize)
	{
		mData = data;
		mDataSize = dataSize;
	}
	public byte[] mData;
	public int mDataSize;
};

public class INPUT_ELEMENT
{
	public INPUT_ELEMENT(SOCKET_PACKET type, byte[] data, int dataSize)
	{
		mType = type;
		mData = data;
		mDataSize = dataSize;
	}
	public SOCKET_PACKET mType;
	public byte[] mData;
	public int mDataSize;
};

public class SocketManager : GameBase
{
	protected const int				mMaxReceiveCount	= 1024;
	protected Socket mServerSoket;
	protected Socket mBroadcastSocket;
	protected EndPoint mBroadcastEP;
	protected Thread mReceiveThread;
	protected Thread mOutputTread;
	protected List<OUTPUT_STREAM> mOutputList;
	protected List<INPUT_ELEMENT> mInputList;
	protected List<INPUT_ELEMENT> mRecieveList;
	protected ThreadLock mOutputLock;
	protected ThreadLock mInputLock;
	protected SocketFactory mSocketFactory;
	protected bool mRunning;
	protected bool mReceiveFinish;
	protected bool mOutputFinish;
	public SocketManager()
	{
		mOutputList = new List<OUTPUT_STREAM>();
		mInputList = new List<INPUT_ELEMENT>();
		mRecieveList = new List<INPUT_ELEMENT>();
		mOutputLock = new ThreadLock();
		mInputLock = new ThreadLock();
		mSocketFactory = new SocketFactory();
	}
	public void init()
	{
		mRunning = true;
		mReceiveFinish = false;
		mOutputFinish = false;
		try
		{
			mSocketFactory.init();
			int port = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_PORT);
			int broadcastPort = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_BROADCAST_PORT);
			// 创建socket  
			mServerSoket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			// 绑定地址  
			mServerSoket.Bind(new IPEndPoint(IPAddress.Any, port));
			mBroadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			mBroadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			// 广播端
			mBroadcastEP = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
			mReceiveThread = new Thread(updateUdpServer);
			mReceiveThread.Start();
			mOutputTread = new Thread(updateOutput);
			mOutputTread.Start();
		}
		catch(Exception)
		{
			UnityUtility.logError("初始化网络失败!请确保测试软件等其他可能占用网络端口的程序已关闭!");
			mGameFramework.stop();
		}
	}
	public void update(float elapsedTime)
	{
		processInput();
	}
	public void destroy()
	{
		mServerSoket.Close();
		mServerSoket = null;
		mBroadcastSocket.Close();
		mBroadcastSocket = null;
		mRunning = false;
		while (!mReceiveFinish) {}
		mReceiveThread.Abort();
		mReceiveThread = null;
		while (!mOutputFinish) {}
		mOutputTread.Abort();
		mOutputTread = null;
		UnityUtility.logInfo("退出完毕", LOG_LEVEL.LL_FORCE);
	}
	public SocketPacket createPacket(SOCKET_PACKET type)
	{
		return mSocketFactory.createPacket(type);
	}
	public void sendMessage(SocketPacket packet)
	{
		// 将消息包中的数据准备好,然后放入发送列表中
		packet.fillData();
		mOutputLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		mOutputList.Add(new OUTPUT_STREAM(packet.getData(), packet.getSize()));
		mOutputLock.unlock(LOCK_TYPE.LT_WRITE);
	}
	//-------------------------------------------------------------------------------------------------------------------------
	protected void processInput()
	{
		// 等待解锁接收流的读写,并锁定接收流
		mInputLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		int receiveCount = mRecieveList.Count;
		for (int i = 0; i < receiveCount; ++i)
		{
			mInputList.Add(mRecieveList[i]);
		}
		mRecieveList.Clear();
		mInputLock.unlock(LOCK_TYPE.LT_WRITE);

		int streamCount = mInputList.Count;
		for (int i = 0; i < streamCount; ++i)
		{
			INPUT_ELEMENT element = mInputList[i];
			SocketPacket packetReply = createPacket(element.mType);
			packetReply.readData(element.mData, element.mDataSize);
			packetReply.execute();
		}
		mInputList.Clear();
	}
	protected void updateOutput()
	{
		while (mRunning)
		{
			try
			{
				mOutputLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
				if (mBroadcastSocket == null)
				{
					break;
				}
				int outputCount = mOutputList.Count;
				for (int i = 0; i < outputCount; ++i)
				{
					mBroadcastSocket.SendTo(mOutputList[i].mData, mBroadcastEP);
				}
				mOutputList.Clear();
				mOutputLock.unlock(LOCK_TYPE.LT_WRITE);
				Thread.Sleep(30);
			}
			catch(Exception)
			{
				mOutputLock.unlock(LOCK_TYPE.LT_WRITE);
				break;
			}
		}
		mOutputFinish = true;
	}
	protected void receivePacket(SOCKET_PACKET type, byte[] data, int dataSize)
	{
		if(!mRunning)
		{
			return;
		}
		mInputLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		mRecieveList.Add(new INPUT_ELEMENT(type, data, dataSize));
		mInputLock.unlock(LOCK_TYPE.LT_WRITE);
	}
	protected void updateUdpServer()
	{
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint ep = (EndPoint)endpoint;
		while (mRunning)
		{
			try
			{
				byte[] recBuff = new byte[mMaxReceiveCount];
				if (mServerSoket == null)
				{
					break;
				}
				int intReceiveLength = mServerSoket.ReceiveFrom(recBuff, ref ep);
				if (intReceiveLength > 0)
				{
					SOCKET_PACKET spType = mSocketFactory.getSocketType(recBuff, intReceiveLength);
					receivePacket(spType, recBuff, intReceiveLength);
				}
			}
			catch(Exception)
			{
				UnityUtility.logInfo("捕获空指针异常", LOG_LEVEL.LL_FORCE);
				break;
			}
		}
		mReceiveFinish = true;
		UnityUtility.logInfo("线程退出完毕", LOG_LEVEL.LL_FORCE);
	}
}