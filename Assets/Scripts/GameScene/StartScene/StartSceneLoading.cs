using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneLoading : SceneProcedure
{
	protected Dictionary<LAYOUT_TYPE, LayoutLoadInfo> mLoadInfo;
	protected int mLoadedCount;
	public LogoSceneLoading()
	{ }
	public LogoSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		mLoadInfo = new Dictionary<LAYOUT_TYPE, LayoutLoadInfo>();
		mLoadInfo.Add(LAYOUT_TYPE.LT_START, new LayoutLoadInfo(LAYOUT_TYPE.LT_START, 0));
		mLoadInfo.Add(LAYOUT_TYPE.LT_GLOBAL_TOUCH, new LayoutLoadInfo(LAYOUT_TYPE.LT_GLOBAL_TOUCH, 100));
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		mLoadedCount = 0;
		// 加载所有布局
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
	//----------------------------------------------------------------------------------------------------------------------
	protected void onLayoutLoaded(GameLayout layout)
	{
		mLoadInfo[layout.getType()].mLayout = layout;
		LayoutTools.HIDE_LAYOUT(layout.getType());
		++mLoadedCount;
		if (mLoadedCount == mLoadInfo.Count)
		{
			allLayoutLoaded();
		}
	}
	protected void allLayoutLoaded()
	{
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_RUNNING;
		mCommandSystem.pushDelayCommand(cmd, mGameScene);
	}
}