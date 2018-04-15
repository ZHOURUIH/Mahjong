using System;
using System.Collections;
using System.Collections.Generic;

public class SCBackToMahjongHallRet : SocketPacket
{
	public SCBackToMahjongHallRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{ }
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;

		// 销毁房间中的所有其他玩家和房间
		Room room = mahjongScene.getRoom();
		room.leaveAllRoomPlayer();
		// 进入到上一个场景
		CommandGameSceneManagerEnter cmdEnter = newCmd(out cmdEnter);
		cmdEnter.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
		pushCommand(cmdEnter, mGameSceneManager);
	}
}