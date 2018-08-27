using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HandInMahjongMyself : HandInMahjong
{
	public HandInMahjongMyself(ScriptMahjongHandIn script, PLAYER_POSITION position)
		: base(script, position)
	{ }
	public override void assignWindow(string handInRoot)
	{
		base.assignWindow(handInRoot);
		int handInCount = mHandInMahjong.Count;
		for (int i = 0; i < handInCount; ++i)
		{
			mScript.newObject(out mHandInMahjong[i].mSelectMask, mHandInMahjong[i].mMahjongWindow, "Select", 0);
		}
	}
	public override void init()
	{
		base.init();
		Vector3 targetOffset = new Vector3(0.0f, 30.0f, 0.0f);
		int count = mHandInMahjong.Count;
		for (int i = 0; i < count; ++i)
		{
			mHandInPosition.Add(mHandInMahjong[i].mMahjongWindow.getPosition());
			mHandInTargetPosition.Add(mHandInPosition[i] + targetOffset);
			mScript.registeBoxColliderNGUI(mHandInMahjong[i].mMahjongWindow, onMahjongClicked);
		}
	}
	public override void onReset()
	{
		base.onReset();
		refreshMahjong(new List<MAHJONG>());
	}
	public override void notifyGetStart(MAHJONG mah)
	{
		HandInMahjongInfo info = mHandInMahjong[mCurHandInCount];
		info.mMahjong = mah;
		info.mState = HANDIN_STATE.HS_SAVED;
		info.mMahjongWindow.setSpriteName(GameDefine.MAHJONG_NAME[(int)mah]);
		// 最后才调用基类函数
		base.notifyGetStart(mah);
	}
	public override void notifyGet(MAHJONG mah)
	{
		// 放到末尾,并且设置为准备打出的状态
		HandInMahjongInfo info = mHandInMahjong[mCurHandInCount];
		info.mMahjong = mah;
		info.mState = HANDIN_STATE.HS_SAVED;
		info.mMahjongWindow.setSpriteName(GameDefine.MAHJONG_NAME[(int)mah]);
		prepareDropMahjong(mCurHandInCount);
		// 最后才调用基类函数
		base.notifyGet(mah);
	}
	public override void notifyDrop(MAHJONG mah, int index)
	{
		base.notifyDrop(mah, index);
		HandInMahjongInfo info = mHandInMahjong[index];
		info.mMahjong = MAHJONG.M_MAX;
		info.mState = HANDIN_STATE.HS_MAX;
		info.mMahjongWindow.setSpriteName("");
	}
	public override void notifyReorder(List<MAHJONG> handIn)
	{
		base.notifyReorder(handIn);
		refreshMahjong(handIn);
	}
	public override void markAbleToPengOrGang(MAHJONG mah)
	{
		for (int i = 0; i < mHandInMahjong.Count; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mHandInMahjong[i].mSelectMask, mah == mHandInMahjong[i].mMahjong);
		}
	}
}