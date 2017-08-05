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
		registerFactory<CSHeartBeat>(PACKET_TYPE.PT_CS_HEART_BEAT);
		registerFactory<CSLogin>(PACKET_TYPE.PT_CS_LOGIN);
		registerFactory<CSCreateRoom>(PACKET_TYPE.PT_CS_CREATE_ROOM);
		registerFactory<CSJoinRoom>(PACKET_TYPE.PT_CS_JOIN_ROOM);
		registerFactory<CSRegister>(PACKET_TYPE.PT_CS_REGISTER);
		registerFactory<CSCheckName>(PACKET_TYPE.PT_CS_CHECK_NAME);
		registerFactory<CSCheckAccount>(PACKET_TYPE.PT_CS_CHECK_ACCOUNT);
		registerFactory<CSReady>(PACKET_TYPE.PT_CS_READY);
		registerFactory<CSLeaveRoom>(PACKET_TYPE.PT_CS_LEAVE_ROOM);
		registerFactory<CSDiceDone>(PACKET_TYPE.PT_CS_DICE_DONE);
		registerFactory<CSRequestDrop>(PACKET_TYPE.PT_CS_REQUEST_DROP);
		registerFactory<CSConfirmAction>(PACKET_TYPE.PT_CS_CONFIRM_ACTION);
		registerFactory<CSContinueGame>(PACKET_TYPE.PT_CS_CONTINUE_GAME);
		registerFactory<CSBackToMahjongHall>(PACKET_TYPE.PT_CS_BACK_TO_MAHJONG_HALL);
		int needCSCount = PACKET_TYPE.PT_CS_MAX - PACKET_TYPE.PT_CS_MIN - 1;
		if (mFactoryList.Count != needCSCount)
		{
			UnityUtility.logError("not all CS packet registered! cur count : " + mFactoryList.Count + ", need count : " + needCSCount);
		}
		// 服务器->客户端
		registerFactory<SCHeartBeatRet>(PACKET_TYPE.PT_SC_HEART_BEAT_RET);
		registerFactory<SCLoginRet>(PACKET_TYPE.PT_SC_LOGIN_RET);
		registerFactory<SCCreateRoomRet>(PACKET_TYPE.PT_SC_CREATE_ROOM_RET);
		registerFactory<SCNotifyBanker>(PACKET_TYPE.PT_SC_NOTIFY_BANKER);
		registerFactory<SCJoinRoomRet>(PACKET_TYPE.PT_SC_JOIN_ROOM_RET);
		registerFactory<SCOtherPlayerJoinRoom>(PACKET_TYPE.PT_SC_OTHER_PLAYER_JOIN_ROOM);
		registerFactory<SCOtherPlayerLeaveRoom>(PACKET_TYPE.PT_SC_OTHER_PLAYER_LEAVE_ROOM);
		registerFactory<SCOtherPlayerOffline>(PACKET_TYPE.PT_SC_OTHER_PLAYER_OFFLINE);
		registerFactory<SCStartGame>(PACKET_TYPE.PT_SC_START_GAME);
		registerFactory<SCRegisterRet>(PACKET_TYPE.PT_SC_REGISTER_RET);
		registerFactory<SCCheckNameRet>(PACKET_TYPE.PT_SC_CHECK_NAME_RET);
		registerFactory<SCCheckAccountRet>(PACKET_TYPE.PT_SC_CHECK_ACCOUNT_RET);
		registerFactory<SCReadyRet>(PACKET_TYPE.PT_SC_READY_RET);
		registerFactory<SCOtherPlayerReady>(PACKET_TYPE.PT_SC_OTHER_PLAYER_READY);
		registerFactory<SCLeaveRoomRet>(PACKET_TYPE.PT_SC_LEAVE_ROOM_RET);
		registerFactory<SCDiceDoneRet>(PACKET_TYPE.PT_SC_DICE_DONE_RET);
		registerFactory<SCNotifyGetStartMahjong>(PACKET_TYPE.PT_SC_NOTIFY_GET_START_MAHJONG);
		registerFactory<SCNotifyReorderMahjong>(PACKET_TYPE.PT_SC_NOTIFY_REORDER_MAHJONG);
		registerFactory<SCNotifyGetStartDone>(PACKET_TYPE.PT_SC_NOTIFY_GET_START_DONE);
		registerFactory<SCAskDrop>(PACKET_TYPE.PT_SC_ASK_DROP);
		registerFactory<SCNotifyGetMahjong>(PACKET_TYPE.PT_SC_NOTIFY_GET_MAHJONG);
		registerFactory<SCAskAction>(PACKET_TYPE.PT_SC_ASK_ACTION);
		registerFactory<SCOtherPlayerDrop>(PACKET_TYPE.PT_SC_OTHER_PLAYER_DROP);
		registerFactory<SCRequestDropRet>(PACKET_TYPE.PT_SC_REQUEST_DROP_RET);
		registerFactory<SCPlayerHu>(PACKET_TYPE.PT_SC_PLAYER_HU);
		registerFactory<SCPlayerGang>(PACKET_TYPE.PT_SC_PLAYER_GANG);
		registerFactory<SCPlayerPeng>(PACKET_TYPE.PT_SC_PLAYER_PENG);
		registerFactory<SCPlayerPass>(PACKET_TYPE.PT_SC_PLAYER_PASS);
		registerFactory<SCOtherPlayerGang>(PACKET_TYPE.PT_SC_OTHER_PLAYER_GANG);
		registerFactory<SCOtherPlayerPeng>(PACKET_TYPE.PT_SC_OTHER_PLAYER_PENG);
		registerFactory<SCOtherPlayerPass>(PACKET_TYPE.PT_SC_OTHER_PLAYER_PASS);
		registerFactory<SCOtherPlayerAskDrop>(PACKET_TYPE.PT_SC_OTHER_PLAYER_ASK_DROP);
		registerFactory<SCOtherPlayerAskAction>(PACKET_TYPE.PT_SC_OTHER_PLAYER_ASK_ACTION);
		registerFactory<SCNotifyMahjongEnd>(PACKET_TYPE.PT_SC_NOTIFY_MAHJONG_END);
		registerFactory<SCContinueGameRet>(PACKET_TYPE.PT_SC_CONTINUE_GAME_RET);
		registerFactory<SCOtherPlayerContinueGame>(PACKET_TYPE.PT_SC_OTHER_PLAYER_CONTINUE_GAME);
		registerFactory<SCBackToMahjongHallRet>(PACKET_TYPE.PT_SC_BACK_TO_MAHJONG_HALL_RET);
		registerFactory<SCOtherPlayerBackToMahjongHall>(PACKET_TYPE.PT_SC_OTHER_PLAYER_BACK_TO_MAHJONG_HALL);
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
	public SocketFactory registerFactory<T>(PACKET_TYPE type) where T : SocketPacket
	{
		SocketFactory factory = createFactory(typeof(T), type);
		mFactoryList.Add(factory.getType(), factory);
		return factory;
	}
}