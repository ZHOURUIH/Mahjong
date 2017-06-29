using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class ComponentHSL : ComponentLinear
{
	protected Vector3 mStartHSL;
	protected Vector3 mTargetHSL;
	protected HSLCallback mHSLFadingCallback;
	protected object mHSLFadingUserData;
	protected HSLCallback mHSLDoneCallback;
	protected object mHSLDoneUserData;
	public ComponentHSL(Type type, string name)
		:
		base(type, name)
	{
		clearCallback();
	}
	public void start(float time, Vector3 startHSL, Vector3 targetHSL, float timeOffset)
	{
		mStartHSL = startHSL;
		mTargetHSL = targetHSL;
		start(0.0f, 1.0f, time, timeOffset);
	}
	public void setStartHSL(Vector3 startHSL) { mStartHSL = startHSL; }
	public void setTargetHSL(Vector3 targetHSL) { mTargetHSL = targetHSL; }
	public Vector3 getStartHSL() { return mStartHSL; }
	public Vector3 getTargetHSL() { return mTargetHSL; }
	public void setHSLFadingCallback(HSLCallback fading, object fadingUserData)
	{
		setCallback(fading, fadingUserData, ref mHSLFadingCallback, ref mHSLFadingUserData, this);
	}
	public void setHSLDoneCallback(HSLCallback hslDone, object doneUserData)
	{
		setCallback(hslDone, doneUserData, ref mHSLDoneCallback, ref mHSLDoneUserData, this);
	}
	//---------------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType(){mBaseType = typeof(ComponentHSL);}
	protected override bool isType(Type type){return type == typeof(ComponentHSL);}
	protected virtual void clearCallback()
	{
		mHSLFadingUserData = null;
		mHSLFadingUserData = null;
		mHSLDoneCallback = null;
		mHSLDoneUserData = null;
	}
	protected static void setCallback(HSLCallback callback, object userData, ref HSLCallback curCallback, ref object curUserData, ComponentHSL component)
	{
		HSLCallback tempCallback = curCallback;
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
	protected void doneCallback(HSLCallback curDoneCallback, object curDoneUserData, ComponentHSL component)
	{
		// 先保存回调,然后再调用回调之前就清空回调,确保在回调函数执行时已经完全完成
		HSLCallback tempCallback = curDoneCallback;
		object tempUserData = curDoneUserData;
		clearCallback();
		if (tempCallback != null)
		{
			tempCallback(component, tempUserData, false, true);
		}
	}
	protected override void applyValue(float value, bool done = false)
	{
		applyHSL(mStartHSL + (mTargetHSL - mStartHSL) * value, done);
	}
	protected virtual void applyHSL(Vector3 hsl, bool done = false) { }
	protected override void afterApplyValue(bool done = false)
	{
		// 无论是否完成,都应该调用正在执行的回调,确保数据正确
		if (mHSLFadingCallback != null)
		{
			mHSLFadingCallback(this, mHSLFadingUserData, false, done);
		}
		base.afterApplyValue(done);
		if (done)
		{
			doneCallback(mHSLDoneCallback, mHSLDoneUserData, this);
		}
	}
}