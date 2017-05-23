using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShowMahjong
{
	public ScriptMahjongHandIn mScript;
	public string mMahjongPreName;
	public ACTION_TYPE mActionType;
	public MAHJONG mMahjong;
	public txUIObject mShowSingleRoot;
	public txUIStaticSprite[] mMahjongWindows;
	public ShowMahjong(ScriptMahjongHandIn script, string preName)
	{
		mScript = script;
		mActionType = ACTION_TYPE.AT_MAX;
		mMahjongPreName = preName;
		mMahjongWindows = new txUIStaticSprite[CommonDefine.MAX_SINGLE_COUNT];
	}
	public void assignWindow(txUIObject parent, string rootName)
	{
		mShowSingleRoot = mScript.newObject<txUIObject>(parent, rootName);
		int count = mMahjongWindows.Length;
		for(int i = 0; i < count; ++i)
		{
			mMahjongWindows[i] = mScript.newObject<txUIStaticSprite>(mShowSingleRoot, "Mahjong" + i, 0);
		}
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		mActionType = ACTION_TYPE.AT_MAX;
		int count = mMahjongWindows.Length;
		for(int i = 0; i < count; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mMahjongWindows[i], false);
		}
	}
	public void showMahjong(ACTION_TYPE type, ref MAHJONG mah)
	{
		mActionType = type;
		mMahjong = mah;
		if(mActionType == ACTION_TYPE.AT_PENG || mActionType == ACTION_TYPE.AT_GANG)
		{
			int count = 0;
			if(mActionType == ACTION_TYPE.AT_PENG)
			{
				count = 3;
			}
			else if(mActionType == ACTION_TYPE.AT_GANG)
			{
				count = 4;
			}
			int maxCount = mMahjongWindows.Length;
			string mahjongSpriteName = mMahjongPreName + CommonDefine.MAHJONG_NAME[(int)mMahjong];
			for (int i = 0; i < maxCount; ++i)
			{
				LayoutTools.ACTIVE_WINDOW(mMahjongWindows[i], i < count);
				if (i < count)
				{
					mMahjongWindows[i].setSpriteName(mahjongSpriteName);
				}
			}
		}
	}
}

public enum HANDIN_STATE
{
	HS_SAVED,
	HS_PREPARE_DROP,
	HS_MAX,
}

public class HandInMahjongInfo
{
	public txUIButton mWindow;
	public MAHJONG mMahjong;
	public HANDIN_STATE mState;
}

public class HandInMahjong : GameBase
{
	public txUIObject mHandInRoot;
	public txUIObject mShowRoot;
	public List<Vector3> mHandInPosition;
	public List<Vector3> mHandInTargetPosition;
	public List<ShowMahjong> mShowMahjongList;
	public List<HandInMahjongInfo> mHandInMahjong;
	public int mCurHandInCount;
	public PLAYER_POSITION mPosition;
	public bool mCanDrop;
	public string mShowPreName;
	public ScriptMahjongHandIn mScript;
	public HandInMahjong(ScriptMahjongHandIn script, PLAYER_POSITION position, string showPreName)
	{
		mScript = script;
		mPosition = position;
		mCanDrop = false;
		mShowPreName = showPreName;
		mHandInMahjong = new List<HandInMahjongInfo>();
		mHandInPosition = new List<Vector3>();
		mHandInTargetPosition = new List<Vector3>();
		mShowMahjongList = new List<ShowMahjong>();
		for (int i = 0; i < CommonDefine.MAX_PENG_TIMES; ++i)
		{
			mShowMahjongList.Add(new ShowMahjong(mScript, mShowPreName));
		}
		for (int i = 0; i < CommonDefine.MAX_HAND_IN_COUNT; ++i)
		{
			HandInMahjongInfo info = new HandInMahjongInfo();
			info.mState = HANDIN_STATE.HS_MAX;
			info.mMahjong = MAHJONG.M_MAX;
			mHandInMahjong.Add(info);
		}
	}
	public void assignWindow(string handInRoot, string showRoot)
	{
		mHandInRoot = mScript.newObject<txUIObject>(handInRoot);
		mShowRoot = mScript.newObject<txUIObject>(showRoot);
		int handInCount = mHandInMahjong.Count;
		for (int i = 0; i < handInCount; ++i)
		{
			mHandInMahjong[i].mWindow = mScript.newObject<txUIButton>(mHandInRoot, "Mahjong" + i);
		}
		// 碰或者杠了的麻将
		int showCount = mShowMahjongList.Count;
		for (int i = 0; i < showCount; ++i)
		{
			mShowMahjongList[i].assignWindow(mShowRoot, "ShowOutRoot" + i);
		}
	}
	public void init()
	{
		// 手里的麻将
		if(mPosition == PLAYER_POSITION.PP_MYSELF)
		{
			Vector3 targetOffset = new Vector3(0.0f, 30.0f, 0.0f);
			int count = mHandInMahjong.Count;
			for (int i = 0; i < count; ++i)
			{
				mHandInPosition.Add(mHandInMahjong[i].mWindow.getPosition());
				mHandInTargetPosition.Add(mHandInPosition[i] + targetOffset);
				mGlobalTouchSystem.registerBoxCollider(mHandInMahjong[i].mWindow, onMahjongClicked, null, null);
			}
		}
		else
		{
			int count = mHandInMahjong.Count;
			for (int i = 0; i < count; ++i)
			{
				mHandInMahjong[i].mWindow.setHandleInput(false);
			}
		}
		// 碰或者杠了的麻将
		for (int i = 0; i < CommonDefine.MAX_PENG_TIMES; ++i)
		{
			mShowMahjongList[i].init();
		}
	}
	public void onReset()
	{
		refreshMahjongCount(0);
		int lineLength = mShowMahjongList.Count;
		for(int i = 0; i < lineLength; ++i)
		{
			mShowMahjongList[i].onReset();
		}
	}
	// 通知开局时的拿牌
	public void notifyGetStart(MAHJONG mah)
	{
		// 开局拿了一张牌
		HandInMahjongInfo info = mHandInMahjong[mCurHandInCount];
		if (mPosition == PLAYER_POSITION.PP_MYSELF)
		{
			info.mMahjong = mah;
			info.mState = HANDIN_STATE.HS_SAVED;
			info.mWindow.setSpriteName(CommonDefine.MAHJONG_NAME[(int)mah]);
		}
		LayoutTools.ACTIVE_WINDOW(info.mWindow);
		++mCurHandInCount;
	}
	// 摸牌
	public void notifyGet(MAHJONG mah)
	{
		HandInMahjongInfo info = mHandInMahjong[mCurHandInCount];
		LayoutTools.ACTIVE_WINDOW(info.mWindow);
		if (mPosition == PLAYER_POSITION.PP_MYSELF)
		{
			// 放到末尾,并且设置为准备打出的状态
			info.mMahjong = mah;
			info.mState = HANDIN_STATE.HS_SAVED;
			info.mWindow.setSpriteName(CommonDefine.MAHJONG_NAME[(int)mah]);
			prepareDropMahjong(mCurHandInCount);
		}
		++mCurHandInCount;
	}
	// 打出一张牌
	public void notifyDrop(MAHJONG mah, int index)
	{
		HandInMahjongInfo info = mHandInMahjong[index];
		if (mPosition == PLAYER_POSITION.PP_MYSELF)
		{
			info.mMahjong = MAHJONG.M_MAX;
			info.mState = HANDIN_STATE.HS_MAX;
			info.mWindow.setSpriteName("");
		}
		LayoutTools.ACTIVE_WINDOW(info.mWindow, false);
	}
	// 碰牌或者杠牌
	public void notifyPengOrGang(PengGangInfo[] infoList)
	{
		int infoCount = infoList.Length;
		int showIndex = 0;
		for (int i = 0; i < infoCount; ++i)
		{
			if(infoList[i].mType != ACTION_TYPE.AT_MAX)
			{
				mShowMahjongList[showIndex++].showMahjong(infoList[i].mType, ref infoList[i].mMahjong);
			}
		}
	}
	//通知重新排列麻将
	public void notifyReorder(List<MAHJONG> handIn)
	{
		if(mPosition == PLAYER_POSITION.PP_MYSELF)
		{
			refreshMahjong(handIn);
		}
		else
		{
			refreshMahjongCount(handIn.Count);
		}
	}
	public void notifyCanDrop(bool wait)
	{
		mCanDrop = wait;
	}
	//------------------------------------------------------------------------------------------------------
	// 刷新麻将的数量
	protected void refreshMahjongCount(int count)
	{
		mCurHandInCount = count;
		int maxCount = mHandInMahjong.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mHandInMahjong[i].mWindow, i < mCurHandInCount);
		}
	}
	// 刷新麻将的数量和显示
	protected void refreshMahjong(List<MAHJONG> handIn)
	{
		mCurHandInCount = handIn.Count;
		int maxCount = mHandInMahjong.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mHandInMahjong[i].mWindow, i < mCurHandInCount);
			if (i < mCurHandInCount)
			{
				mHandInMahjong[i].mMahjong = handIn[i];
				mHandInMahjong[i].mWindow.setSpriteName(CommonDefine.MAHJONG_NAME[(int)handIn[i]]);
				setState(HANDIN_STATE.HS_SAVED, i);
			}
			else
			{
				mHandInMahjong[i].mMahjong = MAHJONG.M_MAX;
				setState(HANDIN_STATE.HS_MAX, i);
			}
		}
	}
	protected void onMahjongClicked(txUIButton button)
	{
		if(!mCanDrop)
		{
			return;
		}
		int index = StringUtility.getLastNumber(button.getName());
		// 点击手里的牌,则将牌设置为准备打出的状态
		if(mHandInMahjong[index].mState == HANDIN_STATE.HS_SAVED)
		{
			prepareDropMahjong(index);
		}
		// 点击准备打出的牌,则将牌打出
		else if(mHandInMahjong[index].mState == HANDIN_STATE.HS_PREPARE_DROP)
		{
			CommandCharacterDrop cmd = new CommandCharacterDrop();
			cmd.mMah = mHandInMahjong[index].mMahjong;
			cmd.mIndex = index;
			mCommandSystem.pushCommand(cmd, mMahjongSystem.getCharacterByPosition(mPosition));
		}
	}
	protected void prepareDropMahjong(int index)
	{
		setState(HANDIN_STATE.HS_PREPARE_DROP, index);
		// 将其他的牌的状态重置为手里的牌
		int count = mHandInMahjong.Count;
		for (int i = 0; i < count; ++i)
		{
			if (i != index && mHandInMahjong[i].mState != HANDIN_STATE.HS_SAVED)
			{
				setState(HANDIN_STATE.HS_SAVED, i);
			}
		}
	}
	protected void setState(HANDIN_STATE state, int index)
	{
		mHandInMahjong[index].mState = state;
		if(state == HANDIN_STATE.HS_PREPARE_DROP)
		{
			LayoutTools.MOVE_WINDOW(mHandInMahjong[index].mWindow, mHandInMahjong[index].mWindow.getPosition(), mHandInTargetPosition[index], 0.1f);
		}
		else
		{
			LayoutTools.MOVE_WINDOW(mHandInMahjong[index].mWindow, mHandInMahjong[index].mWindow.getPosition(), mHandInPosition[index], 0.1f);
		}
	}
}

public class ScriptMahjongHandIn : LayoutScript
{
	HandInMahjong[] mHandInList;
	public ScriptMahjongHandIn(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mHandInList = new HandInMahjong[CommonDefine.MAX_PLAYER_COUNT];
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mHandInList[i] = new HandInMahjong(this, (PLAYER_POSITION)i, CommonDefine.mShowPreName[i]);
		}
	}
	public override void assignWindow()
	{
		int length = mHandInList.Length;
		for (int i = 0; i < length; ++i)
		{
			mHandInList[i].assignWindow(CommonDefine.mHandInRootName[i], CommonDefine.mShowRootName[i]);
		}
	}
	public override void init()
	{
		int length = mHandInList.Length;
		for (int i = 0; i < length; ++i)
		{
			mHandInList[i].init();
		}
	}
	public override void onReset()
	{
		// 隐藏所有的碰牌
		int length = mHandInList.Length;
		for(int i = 0; i < length; ++i)
		{
			mHandInList[i].onReset();
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
	// 开局时拿牌
	public void notifyGetMahjongStart(PLAYER_POSITION pos, MAHJONG mah)
	{
		mHandInList[(int)pos].notifyGetStart(mah);
	}
	// 摸牌
	public void notifyGetMahjong(PLAYER_POSITION pos, MAHJONG mah)
	{
		mHandInList[(int)pos].notifyGet(mah);
	}
	// 打牌
	public void notifyDropMahjong(PLAYER_POSITION pos, MAHJONG mah, int index)
	{
		mHandInList[(int)pos].notifyDrop(mah, index);
	}
	// 碰牌或者杠牌
	public void notifyPengOrGang(PLAYER_POSITION pos, PengGangInfo[] infoList)
	{
		mHandInList[(int)pos].notifyPengOrGang(infoList);
	}
	// 刷新牌
	public void notifyReorder(PLAYER_POSITION pos, List<MAHJONG> handIn)
	{
		mHandInList[(int)pos].notifyReorder(handIn);
	}
	public void notifyCanDrop(bool canDrop)
	{
		int count = mHandInList.Length;
		for(int i = 0; i < count; ++i)
		{
			mHandInList[i].notifyCanDrop(canDrop);
		}
	}
}