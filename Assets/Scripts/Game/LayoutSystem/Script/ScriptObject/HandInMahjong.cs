using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum HANDIN_STATE
{
	HS_SAVED,
	HS_PREPARE_DROP,
	HS_MAX,
}

public class HandInMahjongInfo
{
	public txNGUISprite mMahjongWindow;
	public txNGUITexture mSelectMask;
	public MAHJONG mMahjong;
	public HANDIN_STATE mState;
}
// 手里的牌
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
		for (int i = 0; i < GameDefine.MAX_HAND_IN_COUNT; ++i)
		{
			HandInMahjongInfo info = new HandInMahjongInfo();
			info.mState = HANDIN_STATE.HS_MAX;
			info.mMahjong = MAHJONG.M_MAX;
			mHandInMahjong.Add(info);
		}
	}
	public virtual void assignWindow(string handInRoot)
	{
		mScript.newObject(out mHandInRoot, handInRoot);
		int handInCount = mHandInMahjong.Count;
		for (int i = 0; i < handInCount; ++i)
		{
			mScript.newObject(out mHandInMahjong[i].mMahjongWindow, mHandInRoot, "Mahjong" + i);
		}
	}
	public virtual void init() { }
	public virtual void onReset() { }
	// 通知开局时的拿牌
	public virtual void notifyGetStart(MAHJONG mah)
	{
		// 开局拿了一张牌
		HandInMahjongInfo info = mHandInMahjong[mCurHandInCount];
		LayoutTools.ACTIVE_WINDOW(info.mMahjongWindow);
		++mCurHandInCount;
	}
	// 摸牌
	public virtual void notifyGet(MAHJONG mah)
	{
		LayoutTools.ACTIVE_WINDOW(mHandInMahjong[mCurHandInCount].mMahjongWindow);
		++mCurHandInCount;
	}
	// 拿出一张花牌
	public virtual void notifyShowHua(MAHJONG mah, int index)
	{
		LayoutTools.ACTIVE_WINDOW(mHandInMahjong[index].mMahjongWindow, false);
	}
	// 打出一张牌
	public virtual void notifyDrop(MAHJONG mah, int index)
	{
		LayoutTools.ACTIVE_WINDOW(mHandInMahjong[index].mMahjongWindow, false);
	}
	//通知重新排列麻将
	public virtual void notifyReorder(List<MAHJONG> handIn) { }
	public void notifyCanDrop(bool canDrop)
	{
		mCanDrop = canDrop;
	}
	public void notifyEnd()
	{
		refreshMahjongCount(0);
	}
	public virtual void markAbleToPengOrGang(MAHJONG mah){}
	//------------------------------------------------------------------------------------------------------
	// 刷新麻将的数量
	protected void refreshMahjongCount(int count)
	{
		mCurHandInCount = count;
		int maxCount = mHandInMahjong.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mHandInMahjong[i].mMahjongWindow, i < mCurHandInCount);
		}
	}
	// 刷新麻将的数量和显示
	protected void refreshMahjong(List<MAHJONG> handIn)
	{
		mCurHandInCount = handIn.Count;
		int maxCount = mHandInMahjong.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mHandInMahjong[i].mMahjongWindow, i < mCurHandInCount);
			if (i < mCurHandInCount)
			{
				mHandInMahjong[i].mMahjong = handIn[i];
				mHandInMahjong[i].mMahjongWindow.setSpriteName(GameDefine.MAHJONG_NAME[(int)handIn[i]]);
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
		if (mPosition != PLAYER_POSITION.PP_MYSELF)
		{
			return;
		}
		if (!mCanDrop)
		{
			return;
		}
		txUIObject button = mScript.getLayout().getUIObject(go);
		int index = StringUtility.getLastNumber(button.getName());
		// 点击手里的牌,则将牌设置为准备打出的状态
		if (mHandInMahjong[index].mState == HANDIN_STATE.HS_SAVED)
		{
			prepareDropMahjong(index);
		}
		// 点击准备打出的牌,则请求将牌打出
		else if (mHandInMahjong[index].mState == HANDIN_STATE.HS_PREPARE_DROP)
		{
			CSRequestDrop requestDrop = mSocketNetManager.createPacket<CSRequestDrop>();
			requestDrop.mIndex.mValue = (byte)index;
			mSocketNetManager.sendMessage(requestDrop);
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
		if (state == HANDIN_STATE.HS_PREPARE_DROP)
		{
			LayoutTools.MOVE_WINDOW(mHandInMahjong[index].mMahjongWindow, mHandInMahjong[index].mMahjongWindow.getPosition(), mHandInTargetPosition[index], 0.1f);
		}
		else
		{
			LayoutTools.MOVE_WINDOW(mHandInMahjong[index].mMahjongWindow, mHandInMahjong[index].mMahjongWindow.getPosition(), mHandInPosition[index], 0.1f);
		}
	}
}