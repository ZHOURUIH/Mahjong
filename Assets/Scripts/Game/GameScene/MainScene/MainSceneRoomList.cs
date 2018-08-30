using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneRoomList : SceneProcedure
{
	protected bool mAutoRequest = true;
	protected float mCurRequestTime = 0.0f;	// 自动请求房间列表的剩余时间,-1表示不自动请求
	protected int mCurPage = 0;
	protected float mRequestCD = -1.0f;		// 当前请求房间列表的剩余CD时间,-1表示已冷却,大于等于0表示未冷却
	public MainSceneRoomList(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME_BACK);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_BACK_TO_MAIN_HALL);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ROOM_LIST);
		// 只重置当前页的下标
		mCurPage = 0;
	}
	protected override void onUpdate(float elapsedTime)
	{
		if(mAutoRequest && mCurRequestTime >= 0.0f)
		{
			int beginIntTime = (int)mCurRequestTime;
			mCurRequestTime -= elapsedTime;
			int endIntTime = (int)mCurRequestTime;
			if(beginIntTime != endIntTime)
			{
				// 剩余时间改变时通知布局
				CommandMainSceneAutoRequestTimeChanged cmd = newCmd(out cmd);
				cmd.mTime = endIntTime;
				pushCommand(cmd, mGameScene);
			}
			if (mCurRequestTime < 0.0f)
			{
				// 每隔一定时间自动请求房间列表
				CommandMainSceneRequestRoomList cmd = newCmd(out cmd);
				cmd.mCurPageIndex = mCurPage;
				pushCommand(cmd, mGameScene);
			}
		}
		if(mRequestCD >= 0.0f)
		{
			mRequestCD -= elapsedTime;
			if(mRequestCD < 0.0f)
			{
				mRequestCD = -1.0f;
			}
		}
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME_BACK);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BACK_TO_MAIN_HALL);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ROOM_LIST);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	public bool canRequestRoomList() { return mRequestCD < 0.0f; }
	public void setAutoRequestRoomList(bool autoRequest)
	{
		mAutoRequest = autoRequest;
	}
	public void notifyRoomList()
	{
		// 获得服务器返回的房间列表后重置请求计时
		mCurRequestTime = GameDefine.AUTO_UPDATE_ROOM_TIME;
	}
	public bool getAutoRequestRoomList()
	{
		return mAutoRequest;
	}
	public float getCurRequestTime()
	{
		return mCurRequestTime;
	}
	public int getCurPage() { return mCurPage; }
}