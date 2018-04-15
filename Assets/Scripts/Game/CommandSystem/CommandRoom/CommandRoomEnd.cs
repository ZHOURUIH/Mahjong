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
		mScriptGameEnding.setResult(room.getResultInfoList().Count != 0);
		mScriptGameEnding.setDetail(room.getResultInfoList());
		mScriptGameEnding.setPlayerInfo(mMoneyDeltaList);

		// 切换流程
		CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_ENDING;
		pushCommand(cmdProcedure, mGameSceneManager.getCurScene());

		// 通知所有玩家本局结束
		Dictionary<int, Character> playerList = room.getPlayerList();
		foreach(var player in playerList)
		{
			pushCommand<CommandCharacterNotifyEnd>(player.Value);
		}
	}
}