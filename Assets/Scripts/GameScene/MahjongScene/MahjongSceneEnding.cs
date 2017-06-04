using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneEnding : SceneProcedure
{
	public MahjongSceneEnding()
	{ }
	public MahjongSceneEnding(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 设置本局结果
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_GAME_ENDING);
		// 通知麻将系统进入结束流程
		CommandMahjongSystemState cmdState = new CommandMahjongSystemState();
		cmdState.mPlayState = MAHJONG_PLAY_STATE.MPS_ENDING;
		mCommandSystem.pushCommand(cmdState, mMahjongSystem);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_HAND_IN);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_PLAYER_ACTION);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_GAME_ENDING);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	public void setResult(List<HU_TYPE> huList)
	{
		ScriptGameEnding gameEnding = mLayoutManager.getScript(LAYOUT_TYPE.LT_GAME_ENDING) as ScriptGameEnding;
		gameEnding.setResult(huList);
	}
}