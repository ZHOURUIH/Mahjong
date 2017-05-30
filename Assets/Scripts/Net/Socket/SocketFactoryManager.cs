using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocketFactoryManager
{
	protected Dictionary<PACKET_TYPE, SocketFactory> mFactoryList;
	public SocketFactoryManager()
	{
		mFactoryList = new Dictionary<PACKET_TYPE, SocketFactory>();
	}
	public void init()
	{
		// 注册所有消息
		// 客户端->服务器
		registerFactory(typeof(CSLogin), PACKET_TYPE.PT_CS_LOGIN);
		// 服务器->客户端
		registerFactory(typeof(SCLoginRet), PACKET_TYPE.PT_SC_LOGIN_RET);
	}
	public SocketFactory getFactory(PACKET_TYPE type)
	{
		if (mFactoryList.ContainsKey(type))
		{
			return mFactoryList[type];
		}
		return null;
	}
	public int getPacketSize(PACKET_TYPE type)
	{
		SocketFactory factory = getFactory(type);
		if(factory != null)
		{
			return factory.getPacketSize();
		}
		return 0;
	}
	public SocketPacket createPacket(PACKET_TYPE type)
	{
		SocketFactory factory = getFactory(type);
		if(factory != null)
		{
			return factory.createPacket();
		}
		return null;
	}
	//------------------------------------------------------------------------------------------------------------
	protected SocketFactory createFactory(Type classType, PACKET_TYPE type)
	{
		return UnityUtility.createInstance<SocketFactory>(typeof(SocketFactory), new object[] { classType, type });
	}
	public SocketFactory registerFactory(Type classType, PACKET_TYPE type)
	{
		SocketFactory factory = createFactory(classType, type);
		mFactoryList.Add(factory.getType(), factory);
		return factory;
	}
}