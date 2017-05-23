using UnityEngine;
using System;
using System.Collections;

public class ComponentAlpha : GameComponent
{
	public PLAY_STATE mPlayState;
	public float mStartAlpha;
	public float mTargetAlpha;
	public float mFadeTime;
	public float mCurFadeTime;
	public float mCurAlpha;
	public AlphaFadeCallback mFadeDoneCallback;	// 当透明度变化到目标值时的回调函数
	public AlphaFadeCallback mFadingCallback;	// 当透明度变化到目标值时的回调函数
	public object mFadeDoneUserData;
	public object mFadingUserData;

	public ComponentAlpha(Type type, string name)
		:
		base(type, name)
	{
		mPlayState = PLAY_STATE.PS_STOP;
		mStartAlpha = 1.0f;
		mTargetAlpha = 1.0f;
		mCurAlpha = 1.0f;
		mFadeTime = 0.0f;
		mCurFadeTime = 0.0f;
		clearCallback();
	}
	public override void setBaseType()
	{
		mBaseType = typeof(ComponentAlpha);
	}
	public override void update(float elapsedTime) 
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && !MathUtility.isFloatZero(mFadeTime))
		{
			mCurFadeTime += elapsedTime;
			bool done = false;
			if (mCurFadeTime < mFadeTime)
			{
				mCurAlpha = mStartAlpha + (mTargetAlpha - mStartAlpha) * (mCurFadeTime / mFadeTime);
			}
			else
			{
				mCurAlpha = mTargetAlpha;
				done = true;
			}

			applyAlpha(mCurAlpha, done);
			afterApplyAlpha(done);
		}
		// 调用基类更新子组件
		base.update(elapsedTime);
	}
	public override bool isType(Type type)
	{
		return type == typeof(ComponentAlpha);
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public void stop()
	{
		mPlayState = PLAY_STATE.PS_STOP;
		mCurFadeTime = 0.0f;
	}
	public void setPlayState(PLAY_STATE state)
	{
		if (state == PLAY_STATE.PS_PAUSE)
		{
			pause(true);
		}
		else if (state == PLAY_STATE.PS_PLAY)
		{
			pause(false);
		}
		else if (state == PLAY_STATE.PS_STOP)
		{
			stop();
		}
	}
	public PLAY_STATE getPlayState() { return mPlayState; }
	public void pause(bool isPause = true) { mPlayState = isPause ? PLAY_STATE.PS_PAUSE : PLAY_STATE.PS_PLAY; }
	public void start(float startAlpha, float destAlpha, float changeTime, float timeOffset) 
	{
		pause(false);
		mStartAlpha = startAlpha; 
		mTargetAlpha = destAlpha;
		mFadeTime = changeTime;
		mCurFadeTime = timeOffset;
		bool isStop = MathUtility.isFloatZero(mFadeTime);
		if (!isStop)
		{
			applyAlpha(mStartAlpha + (mCurFadeTime / mFadeTime) * (destAlpha - startAlpha));
		}
		else
		{
			applyAlpha(mTargetAlpha);
		}
		afterApplyAlpha(isStop);
	}
	public float getFadePercent()
	{
		if (MathUtility.isFloatZero(mFadeTime))
		{
			return 0.0f;
		}
		return mCurFadeTime / mFadeTime;
	}
	public virtual void applyAlpha(float alpha, bool done = false) { }
	public void setStartAlpha(float startAlpha) { mStartAlpha = startAlpha; }
	public float getStartAlpha() { return mStartAlpha; }
	public void setTargetAlpha(float targetAlpha) { mTargetAlpha = targetAlpha; }
	public float getTargetAlpha() { return mTargetAlpha; }
	public void setFadeTime(float fadeTime) { mFadeTime = fadeTime; }
	public float getFadeTime() { return mFadeTime; }
	public float getCurFadeTime() { return mCurFadeTime; }
	public void setCurAlpha(float alpha) 
	{ 
		mCurAlpha = alpha;
		applyAlpha(mCurAlpha);
		afterApplyAlpha();
	}
	public float getCurAlpha() { return mCurAlpha; }
	public void setFadeDoneCallback(AlphaFadeCallback callback, object userData)
	{
		setAlphaCallback(callback, userData, ref mFadeDoneCallback, ref mFadeDoneUserData, this);
	}
	public void setFadingCallback(AlphaFadeCallback callback, object userData)
	{
		setAlphaCallback(callback, userData, ref mFadingCallback, ref mFadingUserData, this);
	}
	public void afterApplyAlpha(bool done = false) 
	{
		if (mFadingCallback != null)
		{
			mFadingCallback(this, mFadingUserData, false, done);
		}
		if (done)
		{
			setActive(false);
			doneAlphaCallback(mFadeDoneCallback, mFadeDoneUserData, this);
		}
	
	}
	public void clearCallback()
	{
		mFadingCallback = null;
		mFadeDoneCallback = null;
		mFadingUserData = null;
		mFadeDoneUserData = null;
	}
	public static void setAlphaCallback(AlphaFadeCallback callback, object userData, ref AlphaFadeCallback curCallback, ref object curUserData, ComponentAlpha component)
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
	public void doneAlphaCallback(AlphaFadeCallback curDoneCallback, object curDoneUserData, ComponentAlpha component)
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
}
