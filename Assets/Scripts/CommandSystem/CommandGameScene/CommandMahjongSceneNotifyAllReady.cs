using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyAllReady : Command
{
	public CommandMahjongSceneNotifyAllReady(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_WAITING))
		{
			return;
		}
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE;
		mCommandSystem.pushCommand(cmd, gameScene);
		// 通知全部角色信息布局全部准备完毕
		ScriptAllCharacterInfo allCharacterInfo = mLayoutManager.getScript(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO) as ScriptAllCharacterInfo;
		allCharacterInfo.notifyStartGame();
	}
}