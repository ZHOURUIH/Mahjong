using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RoomItem : GameBase
{
	public ScriptRoomList mScript;
	public txUIObject mItemParent;
	public txNGUITexture mItemRoot;
	public txNGUIText mOwnerName;
	public txNGUIText mPlayerCount;
	public int mRoomID;
	public RoomItem(ScriptRoomList script)
	{
		mScript = script;
	}
	public void assignWindow(txUIObject parent)
	{
		mItemParent = parent;
		mScript.newObject(out mItemRoot, mItemParent, "RoomItem", 1);
		mScript.newObject(out mOwnerName, mItemRoot, "OwnerName");
		mScript.newObject(out mPlayerCount, mItemRoot, "PlayerCount");
	}
	public void init()
	{
		mScript.registeBoxColliderNGUI(mItemRoot, onItemClicked);
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
	public void setRoomID(int roomID)
	{
		mRoomID = roomID;
	}
	protected void onItemClicked(GameObject obj)
	{
		CSJoinRoom join = mSocketNetManager.createPacket<CSJoinRoom>();
		join.mRoomID.mValue = mRoomID;
		mSocketNetManager.sendMessage(join);
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
	protected txNGUIText mPageCountLabel;
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
		newObject(out mPageCountLabel, mBackground, "PageCountLabel");
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
	public override void onGameState()
	{
		base.onGameState();
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST))
		{
			MainSceneRoomList roomListProcedure = gameScene.getCurOrParentProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
			if (roomListProcedure != null)
			{
				confirmAutoRequest(roomListProcedure.getAutoRequestRoomList(), roomListProcedure.getCurRequestTime());
				setPageLabel(0, 0);
				showRoomList(null, 0, 0);
			}
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
	public void setRemainRequestTime(int time)
	{
		mTimeLabel.setLabel(time + "秒");
	}
	public void showRoomList(List<RoomInfo> roomList, int pageIndex, int allRoomCount)
	{
		// 显示当前页房间信息
		int maxCount = mRoomItemList.Length;
		int showCount = MathUtility.getMin(roomList != null ? roomList.Count : 0, maxCount);
		for(int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mRoomItemList[i].mItemParent, i < showCount);
			if(i < showCount)
			{
				mRoomItemList[i].setOwnerName(roomList[i].mOwnerName);
				mRoomItemList[i].setPlayerCount(roomList[i].mCurCount, roomList[i].mMaxCount);
				mRoomItemList[i].setRoomID(roomList[i].mID);
			}
		}
		// 计算总页数
		int pageCount = allRoomCount / GameDefine.ROOM_LIST_PAGE_ITEM_COUNT;
		if(allRoomCount % GameDefine.ROOM_LIST_PAGE_ITEM_COUNT > 0)
		{
			++pageCount;
		}
		setPageLabel(pageIndex, pageCount);
	}
	public void confirmAutoRequest(bool autoRequest, float autoRequestTime)
	{
		mAutoRefreshCheck.setChecked(autoRequest);
		MathUtility.clampMin(ref autoRequestTime, 0.0f);
		setRemainRequestTime((int)autoRequestTime);
	}
	public void setPageLabel(int curPage, int totalPage)
	{
		// 设置翻页按钮是否可点击
		mLastPage.setHandleInput(curPage > 0);
		mNextPage.setHandleInput(curPage + 1 < totalPage);
		mPageCountLabel.setLabel((curPage + 1) + "/" + totalPage);
	}
	//-----------------------------------------------------------------------------------
	protected void onLastPageClicked(GameObject obj)
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST))
		{
			MainSceneRoomList roomListProcedure = gameScene.getCurOrParentProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
			CommandMainSceneRequestRoomList cmd = newCmd(out cmd);
			cmd.mCurPageIndex = roomListProcedure.getCurPage() - 1;
			pushCommand(cmd, gameScene);
		}
	}
	protected void onNextPageClicked(GameObject obj)
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST))
		{
			MainSceneRoomList roomListProcedure = gameScene.getCurOrParentProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
			CommandMainSceneRequestRoomList cmd = newCmd(out cmd);
			cmd.mCurPageIndex = roomListProcedure.getCurPage() + 1;
			pushCommand(cmd, gameScene);
		}
	}
	protected void onManualRefreshClicked(GameObject obj)
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST))
		{
			MainSceneRoomList roomListProcedure = gameScene.getCurOrParentProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
			CommandMainSceneRequestRoomList cmd = newCmd(out cmd);
			cmd.mCurPageIndex = roomListProcedure.getCurPage();
			pushCommand(cmd, gameScene);
		}
	}
	protected void onAutoRefreshClicked(GameObject obj)
	{
		CommandMainSceneNotifyAutoRequest cmd = newCmd(out cmd);
		cmd.mAutoRequest = mAutoRefreshCheck.getChecked();
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onButtonPress(GameObject obj, bool press)
	{
		txUIObject window = mLayout.getUIObject(obj);
		LayoutTools.SCALE_WINDOW(window, window.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}