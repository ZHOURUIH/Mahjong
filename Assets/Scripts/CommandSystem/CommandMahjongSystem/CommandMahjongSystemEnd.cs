using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemEnd : Command
{
	public Character mHuPlayer;
	public MAHJONG mMahjong;
	public List<HU_TYPE> mHuList;
	public CommandMahjongSystemEnd(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		// 先设置结果
		GameScene gameScene = mGameSceneManager.getCurScene();
		MahjongSceneEnding ending = gameScene.getSceneProcedure(PROCEDURE_TYPE.PT_MAHJONG_ENDING) as MahjongSceneEnding;
		ending.setResult(mHuList);
		// 切换流程
		CommandGameSceneChangeProcedure cmdProcedure = new CommandGameSceneChangeProcedure();
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_ENDING;
		mCommandSystem.pushCommand(cmdProcedure, gameScene);
		// 通知所有玩家本局结束
		Dictionary<int, Character> playerList = mahjongSystem.getAllPlayer();
		foreach(var player in playerList)
		{
			CommandCharacterNotifyEnd cmdEnd = new CommandCharacterNotifyEnd();
			mCommandSystem.pushCommand(cmdEnd, player.Value);
		}
	}
	public override string showDebugInfo()
	{
		string name = mHuPlayer != null ? mHuPlayer.getName() : "";
		string huStr = "";
		if(mHuList != null)
		{
			int huCount = mHuList.Count;
			for (int i = 0; i < huCount; ++i)
			{
				huStr += mHuList[i];
				huStr += ", ";
			}
			StringUtility.removeLastComma(ref huStr);
		}
		return base.showDebugInfo() + " : mahjong : " + mMahjong + ", player : " + name + ", hu type : " + huStr;
	}
}