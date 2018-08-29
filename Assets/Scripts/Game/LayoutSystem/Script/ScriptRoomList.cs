using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RoomItem : GameBase
{
	protected ScriptRoomList mScript;
	protected txUIObject mItemParent;
	protected txNGUITexture mItemRoot;
	protected txNGUIText mOwnerName;
	protected txNGUIText mPlayerCount;
	public RoomItem(ScriptRoomList script)
	{
		mScript = script;
	}
	public void assignWindow(txUIObject parent)
	{
		mItemParent = parent;
		mScript.newObject(out mItemRoot, mItemParent, "RoomItem");
		mScript.newObject(out mOwnerName, mItemRoot, "OwnerName");
		mScript.newObject(out mPlayerCount, mItemRoot, "PlayerCount");
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		;
	}
	public void setOwnerName(string name)
	{
		mOwnerName.setLabel(name);
	}
	public void setPlayerCount(int curCount, int maxCount)
	{
		mPlayerCount.setLabel("" + curCount + "/" + maxCount);
	}
}

public class ScriptRoomList : LayoutScript
{
	protected txNGUITexture	mBackground;
	protected txNGUIText mRoomListTitle;
	protected txUIObject mRoomGrid;
	protected txUIObject[] mItemRootList;
	protected RoomItem[] mRoomItemList;
	protected txNGUIButton mLastPage;
	protected txNGUIButton mNextPage;
	protected txNGUIButton mManualRefresh;
	protected txNGUIText mRefreshLabel;
	protected txNGUIText mTimeLabel;
	protected txNGUICheckBox mAutoRefreshCheck;
	public ScriptRoomList(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mItemRootList = new txUIObject[GameDefine.ROOM_LIST_PAGE_ITEM_COUNT];
		mRoomItemList = new RoomItem[GameDefine.ROOM_LIST_PAGE_ITEM_COUNT];
	}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
		newObject(out mRoomListTitle, mBackground, "RoomListTitle");
		newObject(out mRoomGrid, mBackground, "RoomGrid");
		int count = mItemRootList.Length;
		for (int i = 0; i < count; ++i)
		{
			newObject(out mItemRootList[i], mRoomGrid, "Item" + i);
			instantiateObject(mItemRootList[i], "RoomItem");
			mRoomItemList[i] = new RoomItem(this);
			mRoomItemList[i].assignWindow(mItemRootList[i]);
		}
		newObject(out mLastPage, mBackground, "LastPage");
		newObject(out mNextPage, mBackground, "NextPage");
		newObject(out mManualRefresh, mBackground, "ManualRefresh");
		newObject(out mRefreshLabel, mBackground, "RefreshLabel");
		newObject(out mTimeLabel, mRefreshLabel, "TimeLabel");
		newObject(out mAutoRefreshCheck, mRefreshLabel, "AutoRefreshCheck");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mLastPage, onLastPageClicked, onButtonPress);
		registeBoxColliderNGUI(mNextPage, onNextPageClicked, onButtonPress);
		registeBoxColliderNGUI(mManualRefresh, onManualRefreshClicked, onButtonPress);
		registeBoxColliderNGUI(mAutoRefreshCheck, onAutoRefreshClicked, onButtonPress);
		int count = mRoomItemList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRoomItemList[i].init();
		}
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mLastPage);
		LayoutTools.SCALE_WINDOW(mNextPage);
		LayoutTools.SCALE_WINDOW(mManualRefresh);
		int count = mRoomItemList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRoomItemList[i].onReset();
		}
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	//-----------------------------------------------------------------------------------
	protected void onLastPageClicked(GameObject obj)
	{
		;
	}
	protected void onNextPageClicked(GameObject obj)
	{
		;
	}
	protected void onManualRefreshClicked(GameObject obj)
	{
		;
	}
	protected void onAutoRefreshClicked(GameObject obj)
	{
		;
	}
	protected void onButtonPress(GameObject obj, bool press)
	{
		txUIObject window = mLayout.getUIObject(obj);
		LayoutTools.SCALE_WINDOW(window, window.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}