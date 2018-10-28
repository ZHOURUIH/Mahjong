using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunningDice : SceneProcedure
{
	public MahjongSceneRunningDice(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_DICE);
		// 通知全部角色信息布局全部准备完毕
		mScriptAllCharacterInfo.notifyStartGame();
		mScriptMahjongFrame.notifyStartGame();
		CommandGameScenePrepareChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START;
		cmd.mPrepareTime = mScriptDice.getDiceAnimTime() + 1.0f;
		pushCommand(cmd, mGameScene);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_DICE);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}