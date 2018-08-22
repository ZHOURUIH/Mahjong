using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PacketInfo
{
	public PACKET_TYPE mType;
	public Type mClassType;
	public int mPacketSize;
}

public class SocketFactory
{
	protected Dictionary<PACKET_TYPE, PacketInfo> mPacketTypeList;
	protected Dictionary<Type, PacketInfo> mClassTypeList;
	public SocketFactory()
	{
		mPacketTypeList = new Dictionary<PACKET_TYPE, PacketInfo>();
		mClassTypeList = new Dictionary<Type, PacketInfo>();
	}
	public void init()
	{
		// 注册所有消息
		// 客户端->服务器
		registerPacket<CSHeartBeat>(PACKET_TYPE.PT_CS_HEART_BEAT);
		registerPacket<CSLogin>(PACKET_TYPE.PT_CS_LOGIN);
		registerPacket<CSCreateRoom>(PACKET_TYPE.PT_CS_CREATE_ROOM);
		registerPacket<CSJoinRoom>(PACKET_TYPE.PT_CS_JOIN_ROOM);
		registerPacket<CSRegister>(PACKET_TYPE.PT_CS_REGISTER);
		registerPacket<CSCheckName>(PACKET_TYPE.PT_CS_CHECK_NAME);
		registerPacket<CSCheckAccount>(PACKET_TYPE.PT_CS_CHECK_ACCOUNT);
		registerPacket<CSReady>(PACKET_TYPE.PT_CS_READY);
		registerPacket<CSLeaveRoom>(PACKET_TYPE.PT_CS_LEAVE_ROOM);
		registerPacket<CSDiceDone>(PACKET_TYPE.PT_CS_DICE_DONE);
		registerPacket<CSRequestDrop>(PACKET_TYPE.PT_CS_REQUEST_DROP);
		registerPacket<CSConfirmAction>(PACKET_TYPE.PT_CS_CONFIRM_ACTION);
		registerPacket<CSContinueGame>(PACKET_TYPE.PT_CS_CONTINUE_GAME);
		registerPacket<CSBackToMahjongHall>(PACKET_TYPE.PT_CS_BACK_TO_MAHJONG_HALL);
		registerPacket<CSAddMahjongRobot>(PACKET_TYPE.PT_CS_ADD_MAHJONG_ROBOT);
		registerPacket<CSCancelLogin>(PACKET_TYPE.PT_CS_CANCEL_LOGIN);
		int needCSCount = PACKET_TYPE.PT_CS_MAX - PACKET_TYPE.PT_CS_MIN - 1;
		if (mPacketTypeList.Count != needCSCount)
		{
			UnityUtility.logError("not all CS packet registered! cur count : " + mPacketTypeList.Count + ", need count : " + needCSCount);
		}
		// 服务器->客户端
		registerPacket<SCHeartBeatRet>(PACKET_TYPE.PT_SC_HEART_BEAT_RET);
		registerPacket<SCLoginRet>(PACKET_TYPE.PT_SC_LOGIN_RET);
		registerPacket<SCCreateRoomRet>(PACKET_TYPE.PT_SC_CREATE_ROOM_RET);
		registerPacket<SCNotifyBanker>(PACKET_TYPE.PT_SC_NOTIFY_BANKER);
		registerPacket<SCJoinRoomRet>(PACKET_TYPE.PT_SC_JOIN_ROOM_RET);
		registerPacket<SCOtherPlayerJoinRoom>(PACKET_TYPE.PT_SC_OTHER_PLAYER_JOIN_ROOM);
		registerPacket<SCOtherPlayerLeaveRoom>(PACKET_TYPE.PT_SC_OTHER_PLAYER_LEAVE_ROOM);
		registerPacket<SCOtherPlayerOffline>(PACKET_TYPE.PT_SC_OTHER_PLAYER_OFFLINE);
		registerPacket<SCStartGame>(PACKET_TYPE.PT_SC_START_GAME);
		registerPacket<SCRegisterRet>(PACKET_TYPE.PT_SC_REGISTER_RET);
		registerPacket<SCCheckNameRet>(PACKET_TYPE.PT_SC_CHECK_NAME_RET);
		registerPacket<SCCheckAccountRet>(PACKET_TYPE.PT_SC_CHECK_ACCOUNT_RET);
		registerPacket<SCReadyRet>(PACKET_TYPE.PT_SC_READY_RET);
		registerPacket<SCOtherPlayerReady>(PACKET_TYPE.PT_SC_OTHER_PLAYER_READY);
		registerPacket<SCLeaveRoomRet>(PACKET_TYPE.PT_SC_LEAVE_ROOM_RET);
		registerPacket<SCDiceDoneRet>(PACKET_TYPE.PT_SC_DICE_DONE_RET);
		registerPacket<SCNotifyGetStartMahjong>(PACKET_TYPE.PT_SC_NOTIFY_GET_START_MAHJONG);
		registerPacket<SCNotifyReorderMahjong>(PACKET_TYPE.PT_SC_NOTIFY_REORDER_MAHJONG);
		registerPacket<SCNotifyGetStartDone>(PACKET_TYPE.PT_SC_NOTIFY_GET_START_DONE);
		registerPacket<SCAskDrop>(PACKET_TYPE.PT_SC_ASK_DROP);
		registerPacket<SCNotifyGetMahjong>(PACKET_TYPE.PT_SC_NOTIFY_GET_MAHJONG);
		registerPacket<SCAskAction>(PACKET_TYPE.PT_SC_ASK_ACTION);
		registerPacket<SCOtherPlayerDrop>(PACKET_TYPE.PT_SC_OTHER_PLAYER_DROP);
		registerPacket<SCRequestDropRet>(PACKET_TYPE.PT_SC_REQUEST_DROP_RET);
		registerPacket<SCPlayerHu>(PACKET_TYPE.PT_SC_PLAYER_HU);
		registerPacket<SCPlayerGang>(PACKET_TYPE.PT_SC_PLAYER_GANG);
		registerPacket<SCPlayerPeng>(PACKET_TYPE.PT_SC_PLAYER_PENG);
		registerPacket<SCPlayerPass>(PACKET_TYPE.PT_SC_PLAYER_PASS);
		registerPacket<SCOtherPlayerGang>(PACKET_TYPE.PT_SC_OTHER_PLAYER_GANG);
		registerPacket<SCOtherPlayerPeng>(PACKET_TYPE.PT_SC_OTHER_PLAYER_PENG);
		registerPacket<SCOtherPlayerPass>(PACKET_TYPE.PT_SC_OTHER_PLAYER_PASS);
		registerPacket<SCOtherPlayerAskDrop>(PACKET_TYPE.PT_SC_OTHER_PLAYER_ASK_DROP);
		registerPacket<SCOtherPlayerAskAction>(PACKET_TYPE.PT_SC_OTHER_PLAYER_ASK_ACTION);
		registerPacket<SCNotifyMahjongEnd>(PACKET_TYPE.PT_SC_NOTIFY_MAHJONG_END);
		registerPacket<SCContinueGameRet>(PACKET_TYPE.PT_SC_CONTINUE_GAME_RET);
		registerPacket<SCOtherPlayerContinueGame>(PACKET_TYPE.PT_SC_OTHER_PLAYER_CONTINUE_GAME);
		registerPacket<SCBackToMahjongHallRet>(PACKET_TYPE.PT_SC_BACK_TO_MAHJONG_HALL_RET);
		registerPacket<SCOtherPlayerBackToMahjongHall>(PACKET_TYPE.PT_SC_OTHER_PLAYER_BACK_TO_MAHJONG_HALL);
		registerPacket<SCShowHua>(PACKET_TYPE.PT_SC_SHOW_HUA);
		registerPacket<SCOtherPlayerShowHua>(PACKET_TYPE.PT_SC_OTHER_PLAYER_SHOW_HUA);
		registerPacket<SCAddMahjongRobotRet>(PACKET_TYPE.PT_SC_ADD_MAHJONG_ROBOT_RET);
		int needSCCount = PACKET_TYPE.PT_SC_MAX - PACKET_TYPE.PT_SC_MIN - 1;
		if (mPacketTypeList.Count - needCSCount != needSCCount)
		{
			UnityUtility.logError("not all SC packet registered! cur count : " + (mPacketTypeList.Count - needCSCount) + ", need count : " + needSCCount);
		}
	}
	public int getPacketSize(PACKET_TYPE type)
	{
		if(mPacketTypeList.ContainsKey(type))
		{
			return mPacketTypeList[type].mPacketSize;
		}
		return 0;
	}
	public SocketPacket createPacket(PACKET_TYPE type)
	{
		if(mPacketTypeList.ContainsKey(type))
		{
			return UnityUtility.createInstance<SocketPacket>(mPacketTypeList[type].mClassType, type);
		}
		return null;
	}
	public SocketPacket createPacket(Type type)
	{
		if(mClassTypeList.ContainsKey(type))
		{
			return UnityUtility.createInstance<SocketPacket>(type, mClassTypeList[type].mType);
		}
		return null;
	}
	//------------------------------------------------------------------------------------------------------------
	public void registerPacket<T>(PACKET_TYPE type) where T : SocketPacket
	{
		PacketInfo info = new PacketInfo();
		info.mClassType = typeof(T);
		info.mType = type;
		SocketPacket packet = UnityUtility.createInstance<T>(typeof(T), type);
		info.mPacketSize = packet.getSize();
		mPacketTypeList.Add(type, info);
		mClassTypeList.Add(info.mClassType, info);
	}
}