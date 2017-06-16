using System;
using System.Collections;
using System.Collections.Generic;

// 加入房间的结果
public enum JOIN_ROOM_RESULT
{
	JRR_SUCC,           // 加入成功
	JRR_FULL,           // 房间已满
	JRR_ROOM_LOCKED,    // 房间已锁定,拒绝加入
	JRR_NO_ROOM,        // 房间不存在
	JRR_PLAYER_IN_ROOM, // 玩家已在房间中
}

public class SCJoinRoomRet : SocketPacket
{
	protected BYTE mResult = new BYTE();
	protected INT mRoomID = new INT();
	protected BYTE mServerPosition = new BYTE();
	protected BOOL mBanker = new BOOL();
	public SCJoinRoomRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mResult);
		pushParam(mRoomID);
		pushParam(mServerPosition);
		pushParam(mBanker);
	}
	public override void execute()
	{
		JOIN_ROOM_RESULT result = (JOIN_ROOM_RESULT)(mResult.mValue);
		if (result == JOIN_ROOM_RESULT.JRR_SUCC)
		{
			UnityUtility.logInfo("加入房间成功, 房间ID:" + mRoomID);
			// 设置房间号和服务器中的位置
			CharacterMyself myself = mCharacterManager.getMyself();
			CharacterData data = myself.getCharacterData();
			data.mRoomID = mRoomID.mValue;
			data.mServerPosition = (PLAYER_POSITION)mServerPosition.mValue;
			data.mBanker = mBanker.mValue;

			// 进入麻将场景
			CommandGameSceneManagerEnter cmd = new CommandGameSceneManagerEnter();
			cmd.mSceneType = GAME_SCENE_TYPE.GST_MAHJONG;
			mCommandSystem.pushCommand(cmd, mGameSceneManager);

			// 进入麻将场景后,创建房间
			MahjongScene mahjongScene = mGameSceneManager.getCurScene() as MahjongScene;
			Room room = mahjongScene.createRoom(myself.getCharacterData().mRoomID);
			// 将自己加入房间
			CommandRoomJoin cmdJoin = new CommandRoomJoin();
			cmdJoin.mCharacter = myself;
			mCommandSystem.pushCommand(cmdJoin, room);
		}
		else
		{
			UnityUtility.logInfo("加入房间失败, 原因:" + result);
		}
	}
}