using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomEnd : Command
{
	public Dictionary<Character, int> mMoneyDeltaList;
	public override void init()
	{
		base.init();
		mMoneyDeltaList = null;
	}
	public override void execute()
	{
		Room room = mReceiver as Room;
		// 先设置结果
		ScriptGameEnding gameEnding = mLayoutManager.getScript(LAYOUT_TYPE.LT_GAME_ENDING) as ScriptGameEnding;
		gameEnding.setResult(room.getResultInfoList().Count != 0);
		gameEnding.setDetail(room.getResultInfoList());
		gameEnding.setPlayerInfo(mMoneyDeltaList);

		// 切换流程
		CommandGameSceneChangeProcedure cmdProcedure = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>();
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_ENDING;
		mCommandSystem.pushCommand(cmdProcedure, mGameSceneManager.getCurScene());

		// 通知所有玩家本局结束
		Dictionary<int, Character> playerList = room.getPlayerList();
		foreach(var player in playerList)
		{
			CommandCharacterNotifyEnd cmdEnd = mCommandSystem.newCmd<CommandCharacterNotifyEnd>();
			mCommandSystem.pushCommand(cmdEnd, player.Value);
		}
	}
}