using UnityEngine;
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
	protected IPAddress				mIP					= null;
	protected int					mPort				= -1;
	protected IPEndPoint			mRecieveIPE			= null;   // 侦听端口
	protected Socket				mServerSoket		= null;
	protected Socket				mBroadcastSocket	= null;
	protected EndPoint				mBraodCastEP		= null;    // 广播端
	protected int					mBroadcastPort		= -1;
	protected Thread				mReceiveThread		= null;
	protected Thread				mOutputTread		= null;
	protected List<OUTPUT_STREAM>	mOutputList			= new List<OUTPUT_STREAM>();
	protected List<INPUT_ELEMENT>	mInputList			= new List<INPUT_ELEMENT>();
	protected List<INPUT_ELEMENT>	mRecieveList		= new List<INPUT_ELEMENT>();
	protected SocketFactory			mSocketFactory		= new SocketFactory();
	protected bool					mRun				= true;
	public void init()
	{
		mSocketFactory.init();

		mPort = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_PORT);
		mBroadcastPort = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_BROADCAST_PORT);
		mIP = IPAddress.Any;

		mRecieveIPE = new IPEndPoint(mIP, mPort);
		// 创建socket  
		mServerSoket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		// 绑定地址  
		mServerSoket.Bind(mRecieveIPE);
		mBroadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		mBroadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
		// 广播端
		mBraodCastEP = new IPEndPoint(IPAddress.Broadcast, mBroadcastPort);
		mReceiveThread = new Thread(updateUdpServer);
		mReceiveThread.Start();
		mOutputTread = new Thread(updateOutput);
		mOutputTread.Start();
	}
	public void update(float elapsedTime)
	{
		processInput();
	}
	public void destroy()
	{
		mRun = false;
		mReceiveThread.Abort();
		mReceiveThread = null;
		mOutputTread.Abort();
		mOutputTread = null;
		mServerSoket.Close();
		mServerSoket = null;
		mBroadcastSocket.Close();
		mBroadcastSocket = null;
	}
	public SocketPacket createPacket(SOCKET_PACKET type)
	{
		return mSocketFactory.createPacket(type);
	}
	public void sendMessage(SocketPacket packet)
	{
		// 将消息包中的数据准备好,然后放入发送列表中
		packet.fillData();
		lock (mOutputList)
		{
			mOutputList.Add(new OUTPUT_STREAM(packet.getData(), packet.getSize()));
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------
	protected void processInput()
	{
		// 等待解锁接收流的读写,并锁定接收流
		lock (mRecieveList)
		{
			int receiveCount = mRecieveList.Count;
			for (int i = 0; i < receiveCount; ++i)
			{
				mInputList.Add(mRecieveList[i]);
			}
			mRecieveList.Clear();
		}

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
		while(mRun)
		{
			lock (mOutputList)
			{
				int outputCount = mOutputList.Count;
				for (int i = 0; i < outputCount; ++i)
				{
					mBroadcastSocket.SendTo(mOutputList[i].mData, mBraodCastEP);
				}
				mOutputList.Clear();
			}
		}
	}
	protected void receivePacket(SOCKET_PACKET type, byte[] data, int dataSize)
	{
		lock (mRecieveList)
		{
			mRecieveList.Add(new INPUT_ELEMENT(type, data, dataSize));
		}
	}
	protected void updateUdpServer()
	{
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint ep = (EndPoint)endpoint;
		while (mRun)
		{
			byte[] recBuff = new byte[mMaxReceiveCount];
			int intReceiveLength = mServerSoket.ReceiveFrom(recBuff, ref ep);
			if (intReceiveLength > 0)
			{
				SOCKET_PACKET spType = mSocketFactory.getSocketType(recBuff, intReceiveLength);
				receivePacket(spType, recBuff, intReceiveLength);
			}
		}
	}
}