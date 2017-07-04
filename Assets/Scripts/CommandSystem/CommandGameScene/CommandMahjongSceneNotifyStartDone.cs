using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyStartDone : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START))
		{
			return;
		}
		CommandGameSceneChangeProcedure cmd = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GAMING;
		mCommandSystem.pushCommand(cmd, gameScene);
	}
}