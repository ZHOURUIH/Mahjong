using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunningGetStart : SceneProcedure
{
	public MahjongSceneRunningGetStart()
	{ }
	public MahjongSceneRunningGetStart(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_HAND_IN);

		// 通知麻将系统开始拿牌
		CommandMahjongSystemState cmdState = new CommandMahjongSystemState();
		cmdState.mPlayState = MAHJONG_PLAY_STATE.MPS_GET_START;
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