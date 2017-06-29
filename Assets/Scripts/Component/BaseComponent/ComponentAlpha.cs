using UnityEngine;
using System;
using System.Collections;

public class ComponentAlpha : ComponentLinear
{
	public AlphaFadeCallback mFadeDoneCallback;	// 当透明度变化到目标值时的回调函数
	public AlphaFadeCallback mFadingCallback;	// 当透明度变化到目标值时的回调函数
	public object mFadeDoneUserData;
	public object mFadingUserData;
	public ComponentAlpha(Type type, string name)
		:
		base(type, name)
	{
		clearCallback();
	}
	public void setFadeDoneCallback(AlphaFadeCallback callback, object userData)
	{
		setAlphaCallback(callback, userData, ref mFadeDoneCallback, ref mFadeDoneUserData, this);
	}
	public void setFadingCallback(AlphaFadeCallback callback, object userData)
	{
		setAlphaCallback(callback, userData, ref mFadingCallback, ref mFadingUserData, this);
	}
	//--------------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType(){mBaseType = typeof(ComponentAlpha);}
	protected override bool isType(Type type){return type == typeof(ComponentAlpha);}
	protected void doneAlphaCallback(AlphaFadeCallback curDoneCallback, object curDoneUserData, ComponentAlpha component)
	{
		// 先保存回调,然后再调用回调之前就清空回调,确保在回调函数执行时已经完全完成
		AlphaFadeCallback tempCallback = curDoneCallback;
		object tempUserData = curDoneUserData;
		clearCallback();
		if (tempCallback != null)
		{
			tempCallback(component, tempUserData, false, true);
		}
	}
	protected static void setAlphaCallback(AlphaFadeCallback callback, object userData, ref AlphaFadeCallback curCallback, ref object curUserData, ComponentAlpha component)
	{
		AlphaFadeCallback tempCallback = curCallback;
		object tempUserData = curUserData;
		curCallback = null;
		curUserData = null;
		// 如果回调函数当前不为空,则是中断了正在进行的变化
		if (tempCallback != null)
		{
			tempCallback(component, tempUserData, true, false);
		}
		curCallback = callback;
		curUserData = userData;
	}
	protected void clearCallback()
	{
		mFadingCallback = null;
		mFadeDoneCallback = null;
		mFadingUserData = null;
		mFadeDoneUserData = null;
	}
	protected override void afterApplyValue(bool done = false)
	{
		if (mFadingCallback != null)
		{
			mFadingCallback(this, mFadingUserData, false, done);
		}
		base.afterApplyValue(done);
		if (done)
		{
			doneAlphaCallback(mFadeDoneCallback, mFadeDoneUserData, this);
		}
	}
	protected override void applyValue(float value, bool done = false)
	{
		applyAlpha(value, done);
	}
	public virtual void applyAlpha(float alpha, bool done = false) { }
}
