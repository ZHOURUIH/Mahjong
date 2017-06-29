using UnityEngine;
using System;
using System.Collections;

public class ComponentMove : ComponentLinear
{
	protected Vector3 mStartPosition;
	protected Vector3 mTargetPosition;
	protected MoveCallback mMovingCallback;
	protected object mMovingUserData;
	protected MoveCallback mMoveDoneCallback;
	protected object mMoveDoneUserData;
	public ComponentMove(Type type, string name)
		:
		base(type, name)
	{
		clearCallback();
	}
	public void start(float moveTime, Vector3 startPos, Vector3 targetPos, float timeOffset)
	{
		mStartPosition = startPos;
		mTargetPosition = targetPos;
		base.start(0.0f, 1.0f, moveTime, timeOffset);
	}
	public void setMoveDoneCallback(MoveCallback doneCallback, object userdata)
	{
		setCallback(doneCallback, userdata, ref mMoveDoneCallback,ref mMoveDoneUserData, this);
	}
	public void setMovingCallback(MoveCallback movingCallback, object userdata)
	{
		setCallback(movingCallback, userdata, ref mMovingCallback,ref mMovingUserData, this);
	}
	public void setStartPosition(Vector3 startPos) { mStartPosition = startPos; }
	public void setTargetPosition(Vector3 targetPos) { mTargetPosition = targetPos; }
	public Vector3 getStartPosition() { return mStartPosition; }
	public Vector3 getTargetPosition() { return mTargetPosition; }
	//---------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType() { mBaseType = typeof(ComponentMove); }
	protected override bool isType(Type type) { return type == typeof(ComponentMove); }
	protected void clearCallback()
	{
		mMoveDoneCallback = null;
		mMoveDoneUserData = null;
		mMovingCallback = null;
		mMovingUserData = null;
	}
	protected static void setCallback(MoveCallback callback, object userData, ref MoveCallback curCallback, ref object curUserData, ComponentMove component)
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
	protected void doneCallback(MoveCallback curDoneCallback, object curDoneUserData, ComponentMove component)
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
	protected override void applyValue(float value, bool done = false)
	{
		applyMove(mStartPosition + (mTargetPosition - mStartPosition) * value, done);
	}
	protected virtual void applyMove(Vector3 position, bool done = false) { }
	protected override void afterApplyValue(bool done = false)
	{
		// 无论移动是否已经完成,都应该先调用正在移动的回调,确保数据正确
		if (mMovingCallback != null)
		{
			mMovingCallback(this, mMovingUserData, false, done);
		}
		base.afterApplyValue(done);
		if (done)
		{
			doneCallback(mMoveDoneCallback, mMoveDoneUserData, this);
		}
	}
}