using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunningGaming : SceneProcedure
{
	public MahjongSceneRunningGaming()
	{ }
	public MahjongSceneRunningGaming(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_PLAYER_ACTION);

		// 通知麻将系统进入正常麻将游戏
		CommandMahjongSystemState cmdState = new CommandMahjongSystemState();
		cmdState.mPlayState = MAHJONG_PLAY_STATE.MPS_NORMAL_GAMING;
		mCommandSystem.pushCommand(cmdState, mMahjongSystem);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}