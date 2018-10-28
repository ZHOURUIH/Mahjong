using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ComponentKeyFrameBase : GameComponent
{
	protected AnimationCurve	mKeyFrame				= null;		// 当前使用的关键帧
	protected float				mOnceLength				= 1.0f;		// 关键帧长度默认为1秒
	protected string			mTremblingName			= "";
	protected bool				mLoop					= true;
	protected float				mAmplitude				= 1.0f;
	protected float				mCurrentTime			= 0.0f;		// 从上一次从头开始播放到现在的时长
	protected float				mPlayLength				= 0.0f;		// <0 无限播放, >0 播放length时长
	protected float				mPlayedTime				= 0.0f;		// 本次震动已经播放的时长,从上一次开始播放到现在的累计时长
	protected PLAY_STATE		mPlayState				= PLAY_STATE.PS_STOP;
	protected float				mOffset					= 0.0f;
	protected bool				mFullOnce				= true;
	protected float				mStopValue				= 0.0f;		// 当组件停止时,需要应用的关键帧值
	protected KeyFrameCallback	mTremblingCallBack		= null;
	protected object			mTremblingUserData		= null;
	protected KeyFrameCallback	mTrembleDoneCallBack	= null;
	protected object			mTrembleDoneUserData	= null;
	public ComponentKeyFrameBase(Type type, string name)
		:
		base(type, name)
	{
		clearCallback();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public virtual void play(string name, bool loop = false, float onceLength = 1.0f, float offset = 0.0f, bool fullOnce = true, float amplitude = 1.0f)
	{
		setTrembling(name);
		mKeyFrame = mKeyFrameManager.getKeyFrame(mTremblingName);
		if (mKeyFrame == null || isFloatZero(onceLength))
		{
			mStopValue = 0.0f;
			// 停止并禁用组件
			afterApllyTrembling(true);
			return;
		}
		else 
		{
			mStopValue = mKeyFrame.Evaluate(mKeyFrame.length);
		}
		if(offset > onceLength)
		{
			logError("offset must be less than onceLength!");
		}
		mOnceLength = onceLength;
		mPlayState = PLAY_STATE.PS_PLAY;
		mLoop = loop;
		mOffset = offset;
		mCurrentTime = mOffset;
		mAmplitude = amplitude;
		mPlayedTime = 0.0f;
		if (mLoop)
		{
			mPlayLength = -1.0f;
		}
		else
		{
			if (fullOnce)
			{
				mPlayLength = mOnceLength;
			}
			else
			{
				mPlayLength = mOnceLength - offset;
			}
		}
	}
	public virtual void stop(bool force = false)
	{
		// 如果已经是停止的状态,并且不是要强制停止,则不再执行
		if (mPlayState == PLAY_STATE.PS_STOP && !force)
		{
			return;
		}
		// 构建值全部为默认值的关键帧
		if(mComponentOwner != null)
		{
			applyTrembling(mStopValue);
		}
		mPlayState = PLAY_STATE.PS_STOP;
		mKeyFrame = null;
		mCurrentTime = 0.0f;
		mPlayedTime = 0.0f;
	}
	public virtual void pause() { mPlayState = PLAY_STATE.PS_PAUSE; }
	public void setState(PLAY_STATE state)
	{
		if (mPlayState == state)
		{
			return;
		}
		if (state == PLAY_STATE.PS_PLAY)
		{
			play(mTremblingName, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		}
		else if (state == PLAY_STATE.PS_STOP)
		{
			stop();
		}
		else if (state == PLAY_STATE.PS_PAUSE)
		{
			pause();
		}
	}
	public float getTremblingPercent()
	{
		return mOnceLength > 0.0f ? mCurrentTime / mOnceLength : 0.0f;
	}
	public void setTremblingCallback(KeyFrameCallback callback, object userData)
	{
		setCallback(callback, userData, ref mTremblingCallBack, ref mTremblingUserData, this);
	}
	public void setTrembleDoneCallback(KeyFrameCallback callback, object userData)
	{
		setCallback(callback, userData, ref mTrembleDoneCallBack, ref mTrembleDoneUserData, this);
	}
	public static void setCallback(KeyFrameCallback callback, object userData, ref KeyFrameCallback curCallback, ref object curUserData, ComponentKeyFrameBase component)
	{
		KeyFrameCallback tempCallback = curCallback;
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

	// 获得成员变量
	public bool getLoop()					{ return mLoop; }
	public float getOnceLength()			{ return mOnceLength; }
	public float getAmplitude()				{ return mAmplitude; }
	public float getOffset()				{ return mOffset; }
	public bool getFullOnce()				{ return mFullOnce; }
	public PLAY_STATE getState()			{ return mPlayState; }
	public float getCurrentTime()			{ return mCurrentTime; }
	public AnimationCurve getKeyFrame()		{ return mKeyFrame; }
	public string getTremblingName()		{ return mTremblingName; }

	// 设置成员变量
	public void setLoop(bool loop)				{ mLoop = loop; }
	public void setOnceLength(float length)		{ mOnceLength = length; }
	public void setAmplitude(float amplitude)	{ mAmplitude = amplitude; }
	public void setOffset(float offset)			{ mOffset = offset; }
	public void setFullOnce(bool fullOnce)		{ mFullOnce = fullOnce; }
	public void setCurrentTime(float time)		{ mCurrentTime = time; }
	public void setTrembling(string name)		{ mTremblingName = name; }
    //----------------------------------------------------------------------------------------------------------------------------
    protected void clearCallback()
    {
        mTremblingCallBack = null;
        mTremblingUserData = null;
        mTrembleDoneCallBack = null;
        mTrembleDoneUserData = null;
    }
    protected void afterApllyTrembling(bool done)
    {
        if (mTremblingCallBack != null)
        {
            mTremblingCallBack(this, mTremblingUserData, false, done);
        }

        if (done)
        {
            setActive(false);
            // 强制停止组件
            stop(true);
            doneCallback(ref mTrembleDoneCallBack, ref mTrembleDoneUserData, this);
        }
    }
    protected static void doneCallback(ref KeyFrameCallback curDoneCallback, ref object curDoneUserData, ComponentKeyFrameBase component)
    {
        // 先保存回调,然后再调用回调之前就清空回调,确保在回调函数执行时已经完全完成
        KeyFrameCallback tempCallback = curDoneCallback;
        object tempUserData = curDoneUserData;
		component.clearCallback();
        if (tempCallback != null)
        {
            tempCallback(component, tempUserData, false, true);
        }
    }
    protected override bool isType(Type type) { return type == typeof(ComponentKeyFrameBase); }
	protected override void setBaseType() { mBaseType = typeof(ComponentKeyFrameBase); }
	protected virtual void applyTrembling(float value) { }
}