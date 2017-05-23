using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunningDice : SceneProcedure
{
	public MahjongSceneRunningDice()
	{ }
	public MahjongSceneRunningDice(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_DICE);

		// 通知麻将系统开始掷骰子
		CommandMahjongSystemState cmdState = new CommandMahjongSystemState();
		cmdState.mPlayState = MAHJONG_PLAY_STATE.MPS_DICE;
		mCommandSystem.pushCommand(cmdState, mMahjongSystem);

		ScriptDice diceScript = mLayoutManager.getScript(LAYOUT_TYPE.LT_DICE) as ScriptDice;
		diceScript.setDiceResult(mMahjongSystem.getDice());
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_DICE);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}