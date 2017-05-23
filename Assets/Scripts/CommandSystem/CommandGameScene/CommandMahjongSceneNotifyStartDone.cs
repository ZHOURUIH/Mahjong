using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyStartDone : Command
{
	public CommandMahjongSceneNotifyStartDone(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START))
		{
			return;
		}
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GAMING;
		mCommandSystem.pushCommand(cmd, gameScene);
	}
}