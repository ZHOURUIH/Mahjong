using System;
using System.Collections;
using System.Collections.Generic;


public class SCLeaveRoomRet : SocketPacket
{
	public BOOL mResult = new BOOL();
	public SCLeaveRoomRet(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mResult);
	}
	public override void execute()
	{
		if (mResult.mValue)
		{
			UnityUtility.logInfo("离开房间成功");

			// 销毁房间中的所有其他玩家和房间
			Room room = (mGameSceneManager.getCurScene() as MahjongScene).getRoom();
			room.leaveAllRoomPlayer();
			// 进入到上一个场景
			CommandGameSceneManagerEnter cmdEnter = newCmd(out cmdEnter);
			cmdEnter.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			pushCommand(cmdEnter, mGameSceneManager);
		}
		else
		{
			string info = "离开房间失败";
			GameUtility.messageOK(info);
			UnityUtility.logInfo(info);
		}
	}
}