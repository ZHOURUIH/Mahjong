using UnityEngine;
using System;
using System.Collections;

public class ComponentMove : GameComponent
{
	public bool mPause;
	public float mMoveTime;
	public float mCurTimeCount;
	public Vector3 mStartPosition;
	public Vector3 mDestPosition;
	public MoveCallback mMovingCallback;
	public object mMovingUserData;
	public MoveCallback mMoveDoneCallback;
	public object mMoveDoneUserData;

	public ComponentMove(Type type, string name)
		:
		base(type, name)
	{
		mMoveTime = 0.0f;
		mCurTimeCount = 0.0f;
	    mPause = false;
		clearCallback();
	}
		
	public override  void setBaseType(){ mBaseType = typeof(ComponentMove); }
	public override  void update(float elapsedTime)
	{
		updatePosition(elapsedTime);
		base.update(elapsedTime);
	
	}
	public override bool isType(Type type) { return type == typeof(ComponentMove); }
	public override void setActive( bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public void start(float moveTime, Vector3 startPos, Vector3 destPos, float timeOffset)
	{	
		start();
		mMoveTime = moveTime;
		mStartPosition = startPos;
		mDestPosition = destPos;
		mCurTimeCount = timeOffset;
		bool isStop = MathUtility.isFloatZero(mMoveTime);
		if (!isStop)
		{
			applyMove(mStartPosition + (mDestPosition - mStartPosition) * (mCurTimeCount / mMoveTime));
		}
		else
		{
			applyMove(mDestPosition);
		}
		afterApplyMove(isStop);
	}
	public void stop()
	{
		mMoveTime = 0.0f;
		mCurTimeCount = 0.0f;
	}
	// 暂停
	public void pause(){ mPause = true; }
	// 从暂停状态恢复更新
	public void start(){ mPause = false; }
	public float getMovePercent()
	{
		if (MathUtility.isFloatZero(mMoveTime))
		{
			return 0.0f;
		}
		return mCurTimeCount / mMoveTime;
	}
	public void setMoveDoneCallback(MoveCallback doneCallback, object userdata)
	{
		setCallback(doneCallback, userdata, ref mMoveDoneCallback,ref mMoveDoneUserData, this);
	}
	public void setMovingCallback(MoveCallback movingCallback, object userdata)
	{
		setCallback(movingCallback, userdata, ref mMovingCallback,ref mMovingUserData, this);
	}
	public virtual void updatePosition(float elapsedTime) 
	{
		if (!mPause && mMoveTime > 0.0f)
		{
			Vector3 curPos;
			mCurTimeCount += elapsedTime;
			bool done = false;
			if (mCurTimeCount < mMoveTime)
			{
				curPos = mStartPosition + (mDestPosition - mStartPosition) * (mCurTimeCount / mMoveTime);
			}
			else
			{
				curPos = mDestPosition;
				done = true;
			}
			applyMove(curPos);
			afterApplyMove(done);
		}
	}
	public virtual void applyMove(Vector3 position, bool done = false)
	{
		;
	}
	public virtual void afterApplyMove(bool done = false) 
	{ 
		// 无论移动是否已经完成,都应该先调用正在移动的回调,确保数据正确
		if (mMovingCallback != null)
		{
			mMovingCallback(this, mMovingUserData, false, done);
		}
		if (done)
		{
			setActive(false);
			doneCallback(mMoveDoneCallback, mMoveDoneUserData, this);
		}
	}
	public void clearCallback()
	{
		mMoveDoneCallback = null;
		mMoveDoneUserData = null;
		mMovingCallback = null;
		mMovingUserData = null;
	}
	public static void setCallback(MoveCallback callback, object userData, ref MoveCallback curCallback, ref object curUserData, ComponentMove component)
	{
		MoveCallback tempCallback = curCallback;
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
	public void doneCallback(MoveCallback curDoneCallback, object curDoneUserData, ComponentMove component)
	{
		// 先保存回调,然后再调用回调之前就清空回调,确保在回调函数执行时已经完全完成
		MoveCallback tempCallback = curDoneCallback;
		object tempUserData = curDoneUserData;
		clearCallback();
		if (tempCallback != null)
		{
			tempCallback(component, tempUserData, false, true);
		}
	}
}
