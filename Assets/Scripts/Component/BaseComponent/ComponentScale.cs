using UnityEngine;
using System;
using System.Collections;

public class ComponentScale : ComponentLinear 
{
	protected Vector3 mStartScale;		// 起始缩放值
	protected Vector3 mTargetScale;		// 目标缩放值
	protected ScaleCallback mScalingCallback;
	protected object mScalingUserData;
	protected ScaleCallback mScaleDoneCallback;
	protected object mScaleDoneUserData;
	public ComponentScale(Type type, string name)
		:
		base(type, name)
	{}
	public void start(Vector3 startScale, Vector3 targetScale, float scaleTime, float timeOffset)
	{
		mStartScale = startScale;
		mTargetScale = targetScale;
		start(0.0f, 1.0f, scaleTime, timeOffset);
	}
	public void setStartScale( Vector3 startScale){ mStartScale = startScale;}
	public void setTargetScale( Vector3 targetScale) {mTargetScale = targetScale; }
	public  Vector3 getStartScale(){ return mStartScale; }
	public Vector3 getTargetScale() { return mTargetScale; }
	public void setScalingCallback(ScaleCallback callback, object userData)
	{
		setCallback(callback, userData, ref mScalingCallback, ref mScalingUserData, this);
	}
	public void setScaleDoneCallback(ScaleCallback callback, object userData)
	{
		setCallback(callback, userData, ref mScaleDoneCallback, ref mScaleDoneUserData, this);
	}
	//----------------------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(ComponentScale);}
	protected override void setBaseType(){mBaseType = typeof(ComponentScale);}
	protected override void applyValue(float value, bool done = false)
	{
		applyScale(mStartScale + (mTargetScale - mStartScale) * value, done);
	}
	protected virtual void applyScale(Vector3 scale, bool refreshNow, bool done = false) { }
	protected override void afterApplyValue(bool done = false)
	{
		// 无论是否完成,都应该调用正在执行的回调,确保数据正确
		if (mScalingCallback != null)
		{
			mScalingCallback(this, mScalingUserData, false, done);
		}
		base.afterApplyValue(done);
		if (done)
		{
			doneCallback(mScaleDoneCallback, mScaleDoneUserData, this);
		}
	}
	protected void clearCallback()
	{
		mScalingCallback = null;
		mScalingUserData = null;
		mScaleDoneCallback = null;
		mScaleDoneUserData = null;
	}
	protected static void setCallback(ScaleCallback callback, object userData, ref ScaleCallback curCallback, ref object curUserData, ComponentScale component)
	{
		ScaleCallback tempCallback = curCallback;
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
	protected void doneCallback(ScaleCallback curDoneCallback, object curDoneUserData, ComponentScale component)
	{
		// 先保存回调,然后再调用回调之前就清空回调,确保在回调函数执行时已经完全完成
		ScaleCallback tempCallback = curDoneCallback;
		object tempUserData = curDoneUserData;
		clearCallback();
		if (tempCallback != null)
		{
			tempCallback(component, tempUserData, false, true);
		}
	}
}
