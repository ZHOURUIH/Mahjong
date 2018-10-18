using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

public class INPUT_ELEMENT
{
	public INPUT_ELEMENT(PACKET_TYPE type, byte[] data)
	{
		mType = type;
		mData = data;
	}
	public PACKET_TYPE mType;
	public byte[] mData;
};

public enum SOCKET_CONNECT
{
	SC_SERVER,
	SC_MAX,
}

public class SocketManager : FrameComponent
{
	protected SocketFactory mSocketFactory;
	protected Dictionary<SOCKET_CONNECT, SocketConnect> mConnectList;
	public SocketManager(string name)
		:base(name)
	{
		mSocketFactory = new SocketFactory();
		mConnectList = new Dictionary<SOCKET_CONNECT, SocketConnect>();
	}
	public override void init()
	{
		mConnectList.Add(SOCKET_CONNECT.SC_SERVER, new SocketConnect("Server"));
		mSocketFactory.init();
		// 获得IP和端口号
		IPAddress serverIP = null;
		string ipString = mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_TCP_SERVER_IP);
		if (ipString == "")
		{
			IPAddress[] ipList = Dns.GetHostAddresses(mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_TCP_HOST_NAME));
			if (ipList.Length > 0)
			{
				serverIP = ipList[0];
			}
		}
		else
		{
			serverIP = IPAddress.Parse(ipString);
		}
		int port = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_PORT);
		mConnectList[SOCKET_CONNECT.SC_SERVER].init(serverIP, port);
	}
	public override void update(float elapsedTime)
	{
		foreach (var item in mConnectList)
		{
			item.Value.update(elapsedTime);
		}
	}
	public override void destroy()
	{
		foreach(var item in mConnectList)
		{
			item.Value.destroy();
		}
	}
	public SocketPacket createPacket(PACKET_TYPE type)
	{
		return mSocketFactory.createPacket(type);
	}
	public T createPacket<T>() where T : SocketPacket
	{
		return mSocketFactory.createPacket(typeof(T)) as T;
	}
	public int getPacketSize(PACKET_TYPE type)
	{
		return mSocketFactory.getPacketSize(type);
	}
	public void sendMessage<T>(SOCKET_CONNECT connect = SOCKET_CONNECT.SC_SERVER) where T : SocketPacket
	{
		mConnectList[connect].sendMessage<T>();
	}
	public void sendMessage(SocketPacket packet, SOCKET_CONNECT connect = SOCKET_CONNECT.SC_SERVER)
	{
		mConnectList[connect].sendMessage(packet);
	}
	public SocketConnect getConnect(SOCKET_CONNECT connect = SOCKET_CONNECT.SC_SERVER)
	{
		return mConnectList[connect];
	}
}