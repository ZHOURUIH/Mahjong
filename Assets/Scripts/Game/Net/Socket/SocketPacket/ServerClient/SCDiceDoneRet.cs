using System;
using System.Collections;
using System.Collections.Generic;

public class SCDiceDoneRet : SocketPacket
{
	public SCDiceDoneRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams(){ }
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		// 进入到发牌流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START;
		pushCommand(cmd, gameScene);
	}
}