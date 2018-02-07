using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class LayoutScriptAutoHide : LayoutScript
{
	protected bool mShowDone = false;
	protected bool mHideDone = true;
	protected bool mAutoHide = true;
	protected float mAutoHideTime = 3.0f;
	protected float mCurTime = 0.0f;
	public LayoutScriptAutoHide(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void onReset()
	{
		base.onReset();
		mCurTime = 0.0f;
		mShowDone = false;
		mHideDone = true;
	}
	public override void onShow(bool immediately, string param)
	{
		startShowOrHide();
		if (immediately)
		{
			showDone();
		}
	}
	public override void onHide(bool immediately, string param)
	{
		startShowOrHide();
		if (immediately)
		{
			hideDone();
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		// 已经显示完成,并且需要自动隐藏时才会计时
		if (mShowDone && mCurTime >= 0.0f)
		{
			mCurTime += elapsedTime;
			if (mCurTime >= mAutoHideTime)
			{
				LayoutTools.HIDE_LAYOUT(mLayout.getType());
			}
		}
	}
	public void setAutoHide(bool autoHide)
	{
		mCurTime = autoHide ? 0.0f : -1.0f;
	}
	public bool getShowDone()
	{
		return mShowDone;
	}
	public bool getHideDone()
	{
		return mHideDone;
	}
	public void resetHideTime()
	{
		// 只有当需要自动隐藏时,点击屏幕才会重置时间
		if (mCurTime >= 0.0f)
		{
			mCurTime = 0.0f;
		}
	}
	//-----------------------------------------------------------------------------------------------------------------------------
	protected void startShowOrHide()
	{
		mHideDone = false;
		mShowDone = false;
	}
	protected void showDone()
	{
		mHideDone = false;
		mShowDone = true;
	}
	protected void hideDone()
	{
		mHideDone = true;
		mShowDone = false;
		LayoutTools.HIDE_LAYOUT_FORCE(mType);
	}
}