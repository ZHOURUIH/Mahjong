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
	protected Dictionary<LAYOUT_TYPE, LayoutLoadInfo> mLoadInfo;
	protected int mLoadedCount;
	public MainSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		mLoadInfo = new Dictionary<LAYOUT_TYPE, LayoutLoadInfo>();
		addLoadInfo(LAYOUT_TYPE.LT_MAIN_FRAME, 0);
		addLoadInfo(LAYOUT_TYPE.LT_CHARACTER, 1);
		addLoadInfo(LAYOUT_TYPE.LT_BILLBOARD, 1);
		addLoadInfo(LAYOUT_TYPE.LT_ROOM_MENU, 1);
		addLoadInfo(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG, 2);
		addLoadInfo(LAYOUT_TYPE.LT_FREE_MATCH_TIP, 5);
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_MAIN_LOADING, 0);
		mLoadedCount = 0;
		foreach (var item in mLoadInfo)
		{
			LayoutTools.LOAD_NGUI_ASYNC(item.Key, item.Value.mOrder, onLayoutLoaded);
		}
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_LOADING);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	protected void onLayoutLoaded(GameLayout layout)
	{
		mScriptMainLoading.setProgress((float)mLoadedCount / mLoadInfo.Count);
		mLoadInfo[layout.getType()].mLayout = layout;
		if (++mLoadedCount == mLoadInfo.Count)
		{
			allLayoutLoaded();
		}
	}
	protected void allLayoutLoaded()
	{
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_MAIN_HALL;
		pushDelayCommand(cmd, mGameScene);
	}
	protected void addLoadInfo(LAYOUT_TYPE type, int order)
	{
		mLoadInfo.Add(type, new LayoutLoadInfo(type, order));
	}
}