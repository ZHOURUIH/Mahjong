using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneLoading : SceneProcedure
{
	protected int mLastVSync;
	protected int mLastTargetFrameRate;
	protected Dictionary<LAYOUT_TYPE, LayoutLoadInfo> mLoadInfo;
	protected int mLoadedCount;
	public MahjongSceneLoading()
	{ }
	public MahjongSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		mLoadInfo = new Dictionary<LAYOUT_TYPE, LayoutLoadInfo>();
		mLoadInfo.Add(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, new LayoutLoadInfo(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, 2));
		mLoadInfo.Add(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME, new LayoutLoadInfo(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME, 0));
		mLoadInfo.Add(LAYOUT_TYPE.LT_MAHJONG_DROP, new LayoutLoadInfo(LAYOUT_TYPE.LT_MAHJONG_DROP, 1));
		mLoadInfo.Add(LAYOUT_TYPE.LT_MAHJONG_HAND_IN, new LayoutLoadInfo(LAYOUT_TYPE.LT_MAHJONG_HAND_IN, 1));
		mLoadInfo.Add(LAYOUT_TYPE.LT_DICE, new LayoutLoadInfo(LAYOUT_TYPE.LT_DICE, 3));
		mLoadInfo.Add(LAYOUT_TYPE.LT_PLAYER_ACTION, new LayoutLoadInfo(LAYOUT_TYPE.LT_PLAYER_ACTION, 2));
		mLoadInfo.Add(LAYOUT_TYPE.LT_GAME_ENDING, new LayoutLoadInfo(LAYOUT_TYPE.LT_GAME_ENDING, 2));
		mLoadInfo.Add(LAYOUT_TYPE.LT_ADD_PLAYER, new LayoutLoadInfo(LAYOUT_TYPE.LT_ADD_PLAYER, 4));
		mLoadInfo.Add(LAYOUT_TYPE.LT_MAHJONG_FRAME, new LayoutLoadInfo(LAYOUT_TYPE.LT_MAHJONG_FRAME, 4));
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		mLastVSync = QualitySettings.vSyncCount;
		mLastTargetFrameRate = Application.targetFrameRate;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
		mLoadedCount = 0;
		foreach (var item in mLoadInfo)
		{
			LayoutTools.LOAD_LAYOUT_ASYNC(item.Key, item.Value.mOrder, onLayoutLoaded);
		}
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
	//-----------------------------------------------------------------------------------------------------------
	protected void onLayoutLoaded(GameLayout layout)
	{
		mLoadInfo[layout.getType()].mLayout = layout;
		if (++mLoadedCount == mLoadInfo.Count)
		{
			allLayoutLoaded();
		}
	}
	protected void allLayoutLoaded()
	{
		QualitySettings.vSyncCount = mLastVSync;
		Application.targetFrameRate = mLastTargetFrameRate;
		CommandGameSceneChangeProcedure cmd = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>(true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_WAITING;
		mCommandSystem.pushDelayCommand(cmd, mGameScene);
	}
}