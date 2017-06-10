using System;
using System.Collections;
using System.Collections.Generic;


public class SCLeaveRoomRet : SocketPacket
{
	protected bool mResult;
	public SCLeaveRoomRet(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mResult = BinaryUtility.readBool(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBool(data, ref index, mResult);
	}
	public override int getSize()
	{
		return sizeof(bool);
	}
	public override void execute()
	{
		if (mResult)
		{
			UnityUtility.logInfo("离开房间成功");

			// 销毁房间中的所有其他玩家和房间
			Room room = (mGameSceneManager.getCurScene() as MahjongScene).getRoom();
			room.leaveAllRoomPlayer();
			// 进入到上一个场景
			CommandGameSceneManagerEnter cmdEnter = new CommandGameSceneManagerEnter();
			cmdEnter.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			mCommandSystem.pushCommand(cmdEnter, mGameSceneManager);
		}
		else
		{
			UnityUtility.logInfo("离开房间失败");
		}
	}
}