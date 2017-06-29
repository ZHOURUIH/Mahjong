using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomEnd : Command
{
	public CommandRoomEnd(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Room room = mReceiver as Room;
		// 先设置结果
		GameScene gameScene = mGameSceneManager.getCurScene();
		MahjongSceneEnding ending = gameScene.getSceneProcedure(PROCEDURE_TYPE.PT_MAHJONG_ENDING) as MahjongSceneEnding;
		ending.setResult(room.getResultInfoList());
		// 切换流程
		CommandGameSceneChangeProcedure cmdProcedure = new CommandGameSceneChangeProcedure();
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_ENDING;
		mCommandSystem.pushCommand(cmdProcedure, gameScene);
		// 通知所有玩家本局结束
		Dictionary<int, Character> playerList = room.getPlayerList();
		foreach(var player in playerList)
		{
			CommandCharacterNotifyEnd cmdEnd = new CommandCharacterNotifyEnd();
			mCommandSystem.pushCommand(cmdEnd, player.Value);
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo();
	}
}