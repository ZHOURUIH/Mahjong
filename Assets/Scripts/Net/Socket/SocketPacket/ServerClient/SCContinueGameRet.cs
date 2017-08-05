using System;
using System.Collections;
using System.Collections.Generic;

public class SCContinueGameRet : SocketPacket
{
	public SCContinueGameRet(PACKET_TYPE type)
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

		// 跳转到等待流程
		CommandGameSceneChangeProcedure cmdProcedure = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>();
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_WAITING;
		mCommandSystem.pushCommand(cmdProcedure, mahjongScene);

		// 将自己加入房间
		CharacterMyself myself = mCharacterManager.getMyself();
		CommandRoomJoin cmdJoin = mCommandSystem.newCmd<CommandRoomJoin>();
		cmdJoin.mCharacter = myself;
		mCommandSystem.pushCommand(cmdJoin, mahjongScene.getRoom());
	}
}