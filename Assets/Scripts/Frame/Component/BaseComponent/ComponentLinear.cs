using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public abstract class ComponentLinear : GameComponent
{
	public PLAY_STATE mPlayState;
	public float mStart;
	public float mTarget;
	public float mTime;
	public float mCurTime;
	public float mCurValue;
	public ComponentLinear(Type type, string name)
		:
		base(type, name)
	{
		mPlayState = PLAY_STATE.PS_STOP;
		mStart = 0.0f;
		mTarget = 1.0f;
		mCurValue = 1.0f;
		mTime = 0.0f;
		mCurTime = 0.0f;
	}
	public override void update(float elapsedTime) 
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && !MathUtility.isFloatZero(mTime))
		{
			mCurTime += elapsedTime;
			bool done = false;
			if (mCurTime < mTime)
			{
				mCurValue = mStart + (mTarget - mStart) * (mCurTime / mTime);
			}
			else
			{
				mCurValue = mTarget;
				done = true;
			}
			applyValue(mCurValue, done);
			afterApplyValue(done);
		}
		// 调用基类更新子组件
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
	public virtual void stop()
	{
		mPlayState = PLAY_STATE.PS_STOP;
		mCurTime = 0.0f;
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
	public void start(float start, float target, float time, float timeOffset) 
	{
		pause(false);
		mStart = start;
		mTarget = target;
		mTime = time;
		mCurTime = timeOffset;
		bool isStop = MathUtility.isFloatZero(mTime);
		if (!isStop)
		{
			applyValue(mStart + (mCurTime / mTime) * (mTarget - mStart));
		}
		else
		{
			applyValue(mTarget);
		}
		afterApplyValue(isStop);
	}
	public float getProgress()
	{
		if (MathUtility.isFloatZero(mTime))
		{
			return 0.0f;
		}
		return mCurTime / mTime;
	}
	public void setStart(float start) { mStart = start; }
	public float getStart() { return mStart; }
	public void setTarget(float target) { mTarget = target; }
	public float getTarget() { return mTarget; }
	public void setTime(float time) { mTime = time; }
	public float getTime() { return mTime; }
	public float getCurTime() { return mCurTime; }
	//-------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType() { mBaseType = typeof(ComponentLinear); }
	protected override bool isType(Type type) { return type == typeof(ComponentLinear); }
	protected abstract void applyValue(float value, bool done = false);
	protected virtual void afterApplyValue(bool done = false)
	{	
		if (done)
		{
			setActive(false);
		}	
	}
}