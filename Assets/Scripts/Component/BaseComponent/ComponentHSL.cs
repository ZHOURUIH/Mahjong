using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class ComponentHSL : GameComponent
{
	protected PLAY_STATE mPlayState;
	protected Vector3 mStartHSL;
	protected Vector3 mTargetHSL;
	protected float mFadeTime;
	protected float mCurTime;
	protected HSLCallback mHSLFadingCallback;
	protected object mHSLFadingUserData;
	protected HSLCallback mHSLDoneCallback;
	protected object mHSLDoneUserData;
	public ComponentHSL(Type type, string name)
		:
		base(type, name)
	{
		mPlayState = PLAY_STATE.PS_STOP;
		mFadeTime = 0.0f;
		mCurTime = 0.0f;
	}
	public override void setBaseType()
	{
		mBaseType = typeof(ComponentHSL);
	}
	public override bool isType(Type type)
	{
		return type == typeof(ComponentHSL);
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public override void update(float elapsedTime)
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && !MathUtility.isFloatZero(mFadeTime))
		{
			mCurTime += elapsedTime;
			bool done = false;
			Vector3 curHSL;
			if (mCurTime < mFadeTime)
			{
				curHSL = mStartHSL + (mTargetHSL - mStartHSL) * (mCurTime / mFadeTime);
			}
			else
			{
				curHSL = mTargetHSL;
				done = true;
			}
			applyHSL(curHSL, done);
			afterApplyHSL(done);
		}
		base.update(elapsedTime);
	}
	public void stop()
	{
		mPlayState = PLAY_STATE.PS_STOP;
		mCurTime = 0.0f;
	}
	public void pause(bool pause) { mPlayState = pause ? PLAY_STATE.PS_PAUSE : PLAY_STATE.PS_PLAY; }
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
	public PLAY_STATE getPlayState() { return mPlayState; }
	public void start(float fadeTime, Vector3 startHSL, Vector3 targetHSL, float timeOffset)
	{
		pause(false);
		mStartHSL = startHSL;
		mTargetHSL = targetHSL;
		mFadeTime = fadeTime;
		mCurTime = timeOffset;
		bool isStop = MathUtility.isFloatZero(fadeTime);
		if (!isStop)
		{
			applyHSL(mStartHSL + (mTargetHSL - mStartHSL) * mCurTime / mFadeTime);
		}
		else
		{
			applyHSL(mTargetHSL);
		}
		// 如果缩放时间为0,则停止并禁用组件
		afterApplyHSL(isStop);
	}
	public void setStartHSL(Vector3 startHSL) { mStartHSL = startHSL; }
	public void setTargetHSL(Vector3 targetHSL) { mTargetHSL = targetHSL; }
	public void setFadeTime(float fadeTime) { mFadeTime = fadeTime; }
	public Vector3 getStartHSL() { return mStartHSL; }
	public Vector3 getTargetHSL() { return mTargetHSL; }
	public float getFadeTime() { return mFadeTime; }
	public float getCurTime() { return mCurTime; }
	public float getFadePercent()
	{
		if (MathUtility.isFloatZero(mFadeTime))
		{
			return 0.0f;
		}
		return mCurTime / mFadeTime;
	}
	public void setHSLFadingCallback(HSLCallback fading, object fadingUserData)
	{
		setCallback(fading, fadingUserData, ref mHSLFadingCallback, ref mHSLFadingUserData, this);
	}
	public void setHSLDoneCallback(HSLCallback hslDone, object doneUserData)
	{
		setCallback(hslDone, doneUserData, ref mHSLDoneCallback, ref mHSLDoneUserData, this);
	}
	public virtual void applyHSL(Vector3 hsl, bool done = false)
	{
		// 无论是否完成,都应该调用正在执行的回调,确保数据正确
		if (mHSLFadingCallback != null)
		{
			mHSLFadingCallback(this, mHSLFadingUserData, false, done);
		}
		if (done)
		{
			setActive(false);
			doneCallback(mHSLDoneCallback, mHSLDoneUserData, this);
		}
	}
	public void afterApplyHSL(bool done = false) 
	{
		// 无论是否完成,都应该调用正在执行的回调,确保数据正确
		if (mHSLFadingCallback != null)
		{
			mHSLFadingCallback(this, mHSLFadingUserData, false, done);
		}
		if (done)
		{
			setActive(false);
			doneCallback(mHSLDoneCallback, mHSLDoneUserData, this);
		}
	}
	public virtual void clearCallback()
	{
		mHSLFadingUserData = null;
		mHSLFadingUserData = null;
		mHSLDoneCallback = null;
		mHSLDoneUserData = null;
	}
	public static void setCallback(HSLCallback callback, object userData, ref HSLCallback curCallback, ref object curUserData, ComponentHSL component)
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
	public void doneCallback(HSLCallback curDoneCallback, object curDoneUserData, ComponentHSL component)
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
}

