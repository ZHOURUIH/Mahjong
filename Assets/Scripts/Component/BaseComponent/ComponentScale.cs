using UnityEngine;
using System;
using System.Collections;

public class ComponentScale : GameComponent 
{
	protected PLAY_STATE mPlayState;
	protected float mCurScaleTime;		// 当前缩放计时
	protected float mScaleTime;			// 缩放到目标值需要的时间
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
	public override void setBaseType()
	{
		mBaseType = typeof(ComponentScale); 
	}
	public override void update(float elapsedTime) 
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && !MathUtility.isFloatZero(mScaleTime))
		{
			mCurScaleTime += elapsedTime;
			bool done = false;
			Vector3 curScale;
			if (mCurScaleTime < mScaleTime)
			{
				curScale = mStartScale + (mTargetScale - mStartScale) * (mCurScaleTime / mScaleTime);
			}
			else
			{
				curScale = mTargetScale;
				done = true;
			}
				applyScale(curScale, done);
				afterApplyScale(done);
			}
	}
	public override bool isType(Type type)
	{
		return type == typeof(ComponentScale);
	}
	public override void setActive( bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public void stop()
	{
		mPlayState =PLAY_STATE.PS_STOP;
		mCurScaleTime = 0.0f;
	}
	public  void pause(bool pause){	mPlayState = pause ? PLAY_STATE.PS_PAUSE : PLAY_STATE.PS_PLAY;}
	public void setPlayState(PLAY_STATE state)
	{
		if (state == PLAY_STATE.PS_STOP)
		{
			stop();
		}
		else if (state == PLAY_STATE.PS_PAUSE)
		{
			pause(true);
		}
		else if (state == PLAY_STATE.PS_PLAY)
		{
			pause(false);
		}
	}
	public  PLAY_STATE getPlayState() { return mPlayState; }
	public void start( Vector3 startScale,  Vector3 targetScale, float scaleTime, float timeOffset)
	{
		pause(false);
		mScaleTime = scaleTime;
		mStartScale = startScale;
		mTargetScale = targetScale;
		mCurScaleTime = timeOffset;
		bool isStop = MathUtility.isFloatZero(mScaleTime);
		if (!isStop)
		{
			applyScale(mStartScale + (targetScale - startScale) * mCurScaleTime / mScaleTime, true);
		}
		else 
		{
			applyScale(mTargetScale, true);
		}
		// 如果缩放时间为0,则停止并禁用组件
		afterApplyScale(isStop);
	}
	public void setStartScale( Vector3 startScale) 
	{ 
		mStartScale = startScale;
	}
	public void setTargetScale( Vector3 targetScale) 
	{
		mTargetScale = targetScale; 
	}
	public void setScaleTime(float scaleTime) 
	{ 
		mScaleTime = scaleTime; 
	}
	public  Vector3 getStartScale()
	{ 
		return mStartScale; 
	}
	public Vector3 getTargetScale() 
	{ 
		return mTargetScale; 
	}
	public float getScaleTime() 
	{ 
		return mScaleTime; 
	}
	public float getCurScaleTime() 
	{ 
		return mCurScaleTime; 
	}
	public float getScalePercent()
	{
		if (MathUtility.isFloatZero(mScaleTime))
		{
			return 0.0f;
		}
		return mCurScaleTime / mScaleTime;
	} 
	public void setScalingCallback(ScaleCallback callback, object userData)
	{
		setCallback(callback, userData, ref mScalingCallback, ref mScalingUserData, this);
	}
	public void setScaleDoneCallback(ScaleCallback callback, object userData)
	{
		setCallback(callback, userData, ref mScaleDoneCallback, ref mScaleDoneUserData, this);
	}
	public  virtual void applyScale(Vector3 scale, bool refreshNow, bool done = false) 
	{
		;
	}
	public void afterApplyScale(bool done = false) 
	{
		// 无论是否完成,都应该调用正在执行的回调,确保数据正确
		if (mScalingCallback != null)
		{
			mScalingCallback(this, mScalingUserData, false, done);
		}
		if (done)
		{
			setActive(false);
			doneCallback(mScaleDoneCallback, mScaleDoneUserData, this);
		}
	}
	public void clearCallback()
	{
		mScalingCallback		= null;
		mScalingUserData		= null;
		mScaleDoneCallback		= null;
		mScaleDoneUserData		= null;
	}
	public static void setCallback(ScaleCallback callback, object userData, ref ScaleCallback curCallback, ref object curUserData, ComponentScale component)
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
	public void doneCallback(ScaleCallback curDoneCallback, object curDoneUserData, ComponentScale component)
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
