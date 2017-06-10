using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneWaiting : SceneProcedure
{
	public MahjongSceneWaiting()
	{ }
	public MahjongSceneWaiting(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 显示布局
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME);

		// 通知麻将系统开始新的一局
		CommandMahjongSystemState cmdState = new CommandMahjongSystemState();
		cmdState.mPlayState = MAHJONG_PLAY_STATE.MPS_WAITING;
		mCommandSystem.pushCommand(cmdState, mMahjongSystem);

		// 通知房间开始等待玩家加入
		MahjongScene mahjongScene = mGameScene as MahjongScene;
		mahjongScene.getRoom().notifyStartWait();

		// 设置显示房间号
		CharacterMyself myself = mCharacterManager.getMyself();
		ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
		mahjongFrame.setRoomID(myself.getCharacterData().mRoomID);
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
	public void notifyAllPlayerReady()
	{
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE;
		mCommandSystem.pushCommand(cmd, mGameScene);
	}
}