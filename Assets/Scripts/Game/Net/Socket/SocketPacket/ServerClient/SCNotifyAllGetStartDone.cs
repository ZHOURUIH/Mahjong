using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyAllGetStartDone : SocketPacket
{
	public SCNotifyAllGetStartDone(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams(){ }
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START))
		{
			return;
		}
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GAMING;
		pushCommand(cmd, gameScene);
	}
}