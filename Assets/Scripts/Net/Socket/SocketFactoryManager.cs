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
		registerFactory(typeof(CSHeartBeat), PACKET_TYPE.PT_CS_HEART_BEAT);
		registerFactory(typeof(CSLogin), PACKET_TYPE.PT_CS_LOGIN);
		registerFactory(typeof(CSCreateRoom), PACKET_TYPE.PT_CS_CREATE_ROOM);
		registerFactory(typeof(CSJoinRoom), PACKET_TYPE.PT_CS_JOIN_ROOM);
		registerFactory(typeof(CSRegister), PACKET_TYPE.PT_CS_REGISTER);
		registerFactory(typeof(CSCheckName), PACKET_TYPE.PT_CS_CHECK_NAME);
		registerFactory(typeof(CSCheckAccount), PACKET_TYPE.PT_CS_CHECK_ACCOUNT);
		registerFactory(typeof(CSReady), PACKET_TYPE.PT_CS_READY);
		registerFactory(typeof(CSLeaveRoom), PACKET_TYPE.PT_CS_LEAVE_ROOM);
		registerFactory(typeof(CSDiceDone), PACKET_TYPE.PT_CS_DICE_DONE);
		registerFactory(typeof(CSRequestDrop), PACKET_TYPE.PT_CS_REQUEST_DROP);
		registerFactory(typeof(CSConfirmAction), PACKET_TYPE.PT_CS_CONFIRM_ACTION);
		int needCSCount = PACKET_TYPE.PT_CS_MAX - PACKET_TYPE.PT_CS_MIN - 1;
		if (mFactoryList.Count != needCSCount)
		{
			UnityUtility.logError("not all CS packet registered! cur count : " + mFactoryList.Count + ", need count : " + needCSCount);
		}
		// 服务器->客户端
		registerFactory(typeof(SCHeartBeatRet), PACKET_TYPE.PT_SC_HEART_BEAT_RET);
		registerFactory(typeof(SCLoginRet), PACKET_TYPE.PT_SC_LOGIN_RET);
		registerFactory(typeof(SCCreateRoomRet), PACKET_TYPE.PT_SC_CREATE_ROOM_RET);
		registerFactory(typeof(SCNotifyBanker), PACKET_TYPE.PT_SC_NOTIFY_BANKER);
		registerFactory(typeof(SCJoinRoomRet), PACKET_TYPE.PT_SC_JOIN_ROOM_RET);
		registerFactory(typeof(SCOtherPlayerJoinRoom), PACKET_TYPE.PT_SC_OTHER_PLAYER_JOIN_ROOM);
		registerFactory(typeof(SCOtherPlayerLeaveRoom), PACKET_TYPE.PT_SC_OTHER_PLAYER_LEAVE_ROOM);
		registerFactory(typeof(SCOtherPlayerOffline), PACKET_TYPE.PT_SC_OTHER_PLAYER_OFFLINE);
		registerFactory(typeof(SCStartGame), PACKET_TYPE.PT_SC_START_GAME);
		registerFactory(typeof(SCRegisterRet), PACKET_TYPE.PT_SC_REGISTER_RET);
		registerFactory(typeof(SCCheckNameRet), PACKET_TYPE.PT_SC_CHECK_NAME_RET);
		registerFactory(typeof(SCCheckAccountRet), PACKET_TYPE.PT_SC_CHECK_ACCOUNT_RET);
		registerFactory(typeof(SCReadyRet), PACKET_TYPE.PT_SC_READY_RET);
		registerFactory(typeof(SCOtherPlayerReady), PACKET_TYPE.PT_SC_OTHER_PLAYER_READY);
		registerFactory(typeof(SCLeaveRoomRet), PACKET_TYPE.PT_SC_LEAVE_ROOM_RET);
		registerFactory(typeof(SCDiceDoneRet), PACKET_TYPE.PT_SC_DICE_DONE_RET);
		registerFactory(typeof(SCNotifyGetStartMahjong), PACKET_TYPE.PT_SC_NOTIFY_GET_START_MAHJONG);
		registerFactory(typeof(SCNotifyReorderMahjong), PACKET_TYPE.PT_SC_NOTIFY_REORDER_MAHJONG);
		registerFactory(typeof(SCNotifyGetStartDone), PACKET_TYPE.PT_SC_NOTIFY_GET_START_DONE);
		registerFactory(typeof(SCAskDrop), PACKET_TYPE.PT_SC_ASK_DROP);
		registerFactory(typeof(SCNotifyGetMahjong), PACKET_TYPE.PT_SC_NOTIFY_GET_MAHJONG);
		registerFactory(typeof(SCAskAction), PACKET_TYPE.PT_SC_ASK_ACTION);
		registerFactory(typeof(SCOtherPlayerDrop), PACKET_TYPE.PT_SC_OTHER_PLAYER_DROP);
		registerFactory(typeof(SCRequestDropRet), PACKET_TYPE.PT_SC_REQUEST_DROP_RET);
		registerFactory(typeof(SCPlayerHu), PACKET_TYPE.PT_SC_PLAYER_HU);
		registerFactory(typeof(SCPlayerGang), PACKET_TYPE.PT_SC_PLAYER_GANG);
		registerFactory(typeof(SCPlayerPeng), PACKET_TYPE.PT_SC_PLAYER_PENG);
		registerFactory(typeof(SCPlayerPass), PACKET_TYPE.PT_SC_PLAYER_PASS);
		registerFactory(typeof(SCOtherPlayerHu), PACKET_TYPE.PT_SC_OTHER_PLAYER_HU);
		registerFactory(typeof(SCOtherPlayerGang), PACKET_TYPE.PT_SC_OTHER_PLAYER_GANG);
		registerFactory(typeof(SCOtherPlayerPeng), PACKET_TYPE.PT_SC_OTHER_PLAYER_PENG);
		registerFactory(typeof(SCOtherPlayerPass), PACKET_TYPE.PT_SC_OTHER_PLAYER_PASS);
		int needSCCount = PACKET_TYPE.PT_SC_MAX - PACKET_TYPE.PT_SC_MIN - 1;
		if (mFactoryList.Count - needCSCount != needSCCount)
		{
			UnityUtility.logError("not all SC packet registered! cur count : " + (mFactoryList.Count - needCSCount) + ", need count : " + needSCCount);
		}
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