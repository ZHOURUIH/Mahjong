using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutLoadInfo
{
	public LAYOUT_TYPE mType;
	public GameLayout mLayout;
	public int mOrder;
	public LayoutLoadInfo(LAYOUT_TYPE type, int order)
	{
		mType = type;
		mOrder = order;
		mLayout = null;
	}
}

public class MainSceneLoading : SceneProcedure
{
	protected int mLastVSync;
	protected int mLastTargetFrameRate;
	protected Dictionary<LAYOUT_TYPE, LayoutLoadInfo> mLoadInfo;
	protected int mLoadedCount;
	public MainSceneLoading()
	{ }
	public MainSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		mLoadInfo = new Dictionary<LAYOUT_TYPE, LayoutLoadInfo>();
		mLoadInfo.Add(LAYOUT_TYPE.LT_MAIN_FRAME, new LayoutLoadInfo(LAYOUT_TYPE.LT_MAIN_FRAME, 0));
		mLoadInfo.Add(LAYOUT_TYPE.LT_CHARACTER, new LayoutLoadInfo(LAYOUT_TYPE.LT_CHARACTER, 1));
		mLoadInfo.Add(LAYOUT_TYPE.LT_BILLBOARD, new LayoutLoadInfo(LAYOUT_TYPE.LT_BILLBOARD, 1));
		mLoadInfo.Add(LAYOUT_TYPE.LT_ROOM_MENU, new LayoutLoadInfo(LAYOUT_TYPE.LT_ROOM_MENU, 1));
		mLoadInfo.Add(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG, new LayoutLoadInfo(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG, 2));
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

		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_RUNNING;
		mCommandSystem.pushDelayCommand(cmd, mGameScene);
	}
}