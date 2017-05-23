using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyDiceDone : Command
{
	public CommandMahjongSceneNotifyDiceDone(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE))
		{
			return;
		}
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START;
		mCommandSystem.pushCommand(cmd, gameScene);
	}
}