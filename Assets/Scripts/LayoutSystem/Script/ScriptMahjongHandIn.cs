using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PengGangMahjong
{
	protected ScriptMahjongHandIn mScript;
	protected string mMahjongPreName;
	protected txUIObject mPengGangRoot;
	protected List<txUIObject> mPengGangSingleRoot;
	protected List<List<txUIStaticSprite>> mMahjongWindows;
	public PengGangMahjong(ScriptMahjongHandIn script, string preName)
	{
		mScript = script;
		mMahjongPreName = preName;
		mPengGangSingleRoot = new List<txUIObject>();
		mMahjongWindows = new List<List<txUIStaticSprite>>();
		for (int i = 0; i < CommonDefine.MAX_PENG_TIMES; ++i)
		{
			mMahjongWindows.Add(new List<txUIStaticSprite>());
		}
	}
	public void assignWindow(string rootName)
	{
		mPengGangRoot = mScript.newObject<txUIObject>(rootName);
		for (int i = 0; i < CommonDefine.MAX_PENG_TIMES; ++i)
		{
			mPengGangSingleRoot.Add(mScript.newObject<txUIObject>(mPengGangRoot, "PengGang" + i));
		}
		int pengTimes = mMahjongWindows.Count;
		for (int i = 0; i < pengTimes; ++i)
		{
			for (int j = 0; j < CommonDefine.MAX_SINGLE_COUNT; ++j)
			{
				mMahjongWindows[i].Add(mScript.newObject<txUIStaticSprite>(mPengGangSingleRoot[i], "Mahjong" + j, 0));
			}
		}
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		int count = mPengGangSingleRoot.Count;
		for(int i = 0; i < count; ++i)
		{
			resetPengGang(i);
		}
	}
	public void showMahjong(PengGangInfo[] infoList)
	{
		int maxCount = mPengGangSingleRoot.Count;
		if(maxCount != infoList.Length)
		{
			UnityUtility.logError("count not match!");
			return;
		}
		int showIndex = 0;
		for (int i = 0; i < maxCount; ++i)
		{
			resetPengGang(i);
			PengGangInfo curInfo = infoList[i];
			if(curInfo.mMahjong != MAHJONG.M_MAX)
			{
				showPengGang(showIndex++, curInfo.mType, curInfo.mMahjong);
			}
		}
	}
	//----------------------------------------------------------------------------------------------------------
	protected void resetPengGang(int index)
	{
		LayoutTools.ACTIVE_WINDOW(mPengGangSingleRoot[index], false);
		int maxCount = mMahjongWindows[index].Count;
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mMahjongWindows[index][i], false);
		}
	}
	protected void showPengGang(int index, ACTION_TYPE type, MAHJONG mah)
	{
		if (type == ACTION_TYPE.AT_PENG || type == ACTION_TYPE.AT_GANG)
		{
			int count = 0;
			if (type == ACTION_TYPE.AT_PENG)
			{
				count = 3;
			}
			else if (type == ACTION_TYPE.AT_GANG)
			{
				count = 4;
			}
			else
			{
				return;
			}
			LayoutTools.ACTIVE_WINDOW(mPengGangSingleRoot[index]);
			int maxCount = mMahjongWindows[index].Count;
			string mahjongSpriteName = mMahjongPreName + CommonDefine.MAHJONG_NAME[(int)mah];
			for (int i = 0; i < maxCount; ++i)
			{
				LayoutTools.ACTIVE_WINDOW(mMahjongWindows[index][i], i < count);
				if (i < count)
				{
					mMahjongWindows[index][i].setSpriteName(mahjongSpriteName);
				}
			}
		}
	}
}

public class ShowMahjong
{
	protected ScriptMahjongHandIn mScript;
	protected string mMahjongPreName;
	protected txUIObject mShowRoot;
	protected List<txUIStaticSprite> mShowMahjong;
	public ShowMahjong(ScriptMahjongHandIn script, string mahjongPreName)
	{
		mScript = script;
		mMahjongPreName = mahjongPreName;
		mShowMahjong = new List<txUIStaticSprite>();
	}
	public void assignWindow(string showRoot)
	{
		mShowRoot = mScript.newObject<txUIObject>(showRoot, 0);
		for(int i = 0; i < CommonDefine.MAX_HAND_IN_COUNT; ++i)
		{
			mShowMahjong.Add(mScript.newObject<txUIStaticSprite>(mShowRoot, "Mahjong" + i));
		}
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		;
	}
	public void showCurMahjong(List<MAHJONG> mahList)
	{
		LayoutTools.ACTIVE_WINDOW(mShowRoot);
		int curCount = mahList.Count;
		int maxCount = mShowMahjong.Count;
		for(int i = 0; i < maxCount; ++i)
		{
			bool show = i < curCount;
			LayoutTools.ACTIVE_WINDOW(mShowMahjong[i], show);
			if(show)
			{
				mShowMahjong[i].setSpriteName(mMahjongPreName + CommonDefine.MAHJONG_NAME[(int)mahList[i]]);
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
	protected txUIObject mHandInRoot;
	protected List<HandInMahjongInfo> mHandInMahjong;
	protected List<Vector3> mHandInPosition;
	protected List<Vector3> mHandInTargetPosition;
	protected int mCurHandInCount;
	protected PLAYER_POSITION mPosition;
	protected bool mCanDrop;
	protected ScriptMahjongHandIn mScript;
	public HandInMahjong(ScriptMahjongHandIn script, PLAYER_POSITION position)
	{
		mScript = script;
		mPosition = position;
		mCanDrop = false;
		mHandInMahjong = new List<HandInMahjongInfo>();
		mHandInPosition = new List<Vector3>();
		mHandInTargetPosition = new List<Vector3>();
		for (int i = 0; i < CommonDefine.MAX_HAND_IN_COUNT; ++i)
		{
			HandInMahjongInfo info = new HandInMahjongInfo();
			info.mState = HANDIN_STATE.HS_MAX;
			info.mMahjong = MAHJONG.M_MAX;
			mHandInMahjong.Add(info);
		}
	}
	public void assignWindow(string handInRoot)
	{
		mHandInRoot = mScript.newObject<txUIObject>(handInRoot);
		int handInCount = mHandInMahjong.Count;
		for (int i = 0; i < handInCount; ++i)
		{
			mHandInMahjong[i].mWindow = mScript.newObject<txUIButton>(mHandInRoot, "Mahjong" + i);
		}
		// 碰或者杠了的麻将
		
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
				mHandInMahjong[i].mWindow.setClickCallback(onMahjongClicked);
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
		
	}
	public void onReset()
	{
		refreshMahjongCount(0);
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
	public void notifyCanDrop(bool canDrop)
	{
		mCanDrop = canDrop;
	}
	public void notifyEnd()
	{
		refreshMahjongCount(0);
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
	protected void onMahjongClicked(GameObject go)
	{
		if(mPosition != PLAYER_POSITION.PP_MYSELF)
		{
			return;
		}
		if(!mCanDrop)
		{
			return;
		}
		txUIObject button = mScript.getLayout().getUIObject(go);
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
			mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
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

// 玩家手里所有的麻将,手牌,碰牌,胡牌后展示的牌
public class PlayerMahjong : GameBase
{
	protected ShowMahjong mShowMahjong;
	protected PengGangMahjong mPengGangMahjong;
	protected HandInMahjong mHandInMahjong;
	protected PLAYER_POSITION mPosition;
	protected ScriptMahjongHandIn mScript;
	public PlayerMahjong(ScriptMahjongHandIn script, PLAYER_POSITION position)
	{
		mScript = script;
		mPosition = position;
		mHandInMahjong = new HandInMahjong(mScript, mPosition);
		mShowMahjong = new ShowMahjong(mScript, CommonDefine.mDropMahjongPreName[(int)mPosition]);
		mPengGangMahjong = new PengGangMahjong(mScript, CommonDefine.mDropMahjongPreName[(int)mPosition]);
	}
	public void assignWindow(string handInRoot, string pengGangRoot, string showRoot)
	{
		mHandInMahjong.assignWindow(handInRoot);
		mPengGangMahjong.assignWindow(pengGangRoot);
		mShowMahjong.assignWindow(showRoot);
	}
	public void init()
	{
		mHandInMahjong.init();
		mPengGangMahjong.init();
		mShowMahjong.init();
	}
	public void onReset()
	{
		mHandInMahjong.onReset();
		mPengGangMahjong.onReset();
		mShowMahjong.onReset();
	}
	public void notifyGetStart(MAHJONG mah)
	{
		mHandInMahjong.notifyGetStart(mah);
	}
	public void notifyGet(MAHJONG mah)
	{
		mHandInMahjong.notifyGet(mah);
	}
	public void notifyDrop(MAHJONG mah, int index)
	{
		mHandInMahjong.notifyDrop(mah, index);
	}
	public void notifyReorder(List<MAHJONG> handIn)
	{
		mHandInMahjong.notifyReorder(handIn);
	}
	public void notifyCanDrop(bool canDrop)
	{
		mHandInMahjong.notifyCanDrop(canDrop);
	}
	// 碰牌或者杠牌
	public void notifyPengOrGang(PengGangInfo[] infoList)
	{
		mPengGangMahjong.showMahjong(infoList);
	}
	// 通知游戏结束,显示自己的牌
	public void notifyEnd(List<MAHJONG> handIn)
	{
		mHandInMahjong.notifyEnd();
		mShowMahjong.showCurMahjong(handIn);
	}
}

public class ScriptMahjongHandIn : LayoutScript
{
	protected List<PlayerMahjong> mPlayerMahjong;
	public ScriptMahjongHandIn(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mPlayerMahjong = new List<PlayerMahjong>();
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mPlayerMahjong.Add(new PlayerMahjong(this, (PLAYER_POSITION)i));
		}
	}
	public override void assignWindow()
	{
		int length = mPlayerMahjong.Count;
		for (int i = 0; i < length; ++i)
		{
			mPlayerMahjong[i].assignWindow(CommonDefine.mHandInRootName[i], CommonDefine.mPengGangRootName[i], CommonDefine.mShowRootName[i]);
		}
	}
	public override void init()
	{
		int length = mPlayerMahjong.Count;
		for (int i = 0; i < length; ++i)
		{
			mPlayerMahjong[i].init();
		}
	}
	public override void onReset()
	{
		// 隐藏所有的碰牌
		int length = mPlayerMahjong.Count;
		for (int i = 0; i < length; ++i)
		{
			mPlayerMahjong[i].onReset();
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
		mPlayerMahjong[(int)pos].notifyGetStart(mah);
	}
	// 摸牌
	public void notifyGetMahjong(PLAYER_POSITION pos, MAHJONG mah)
	{
		mPlayerMahjong[(int)pos].notifyGet(mah);
	}
	// 打牌
	public void notifyDropMahjong(PLAYER_POSITION pos, MAHJONG mah, int index)
	{
		mPlayerMahjong[(int)pos].notifyDrop(mah, index);
	}
	// 碰牌或者杠牌
	public void notifyPengOrGang(PLAYER_POSITION pos, PengGangInfo[] infoList)
	{
		mPlayerMahjong[(int)pos].notifyPengOrGang(infoList);
	}
	// 刷新牌
	public void notifyReorder(PLAYER_POSITION pos, List<MAHJONG> handIn)
	{
		mPlayerMahjong[(int)pos].notifyReorder(handIn);
	}
	public void notifyCanDrop(bool canDrop)
	{
		mPlayerMahjong[(int)PLAYER_POSITION.PP_MYSELF].notifyCanDrop(canDrop);
	}
	public void notifyEnd(PLAYER_POSITION pos, List<MAHJONG> handIn)
	{
		mPlayerMahjong[(int)pos].notifyEnd(handIn);
	}
}