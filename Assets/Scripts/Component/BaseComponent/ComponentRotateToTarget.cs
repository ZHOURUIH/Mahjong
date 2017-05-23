using UnityEngine;
using System;
using System.Collections;

public class ComponentRotateToTarget : ComponentRotate
{
	public float mCurRotateTime;			// 当前旋转计时
	public float mRotateTime;				// 旋转总时间
	public Vector3 mTargetAngle;			// 目标欧拉角
	public RotateToTargetCallback mRotatingCallback;
	public RotateToTargetCallback mRotateDoneCallback;
	public object mRotatingUserData;
	public object mRotateDoneUserData;
	public ComponentRotateToTarget(Type type, string name)
		:
		base(type, name)
	{
		mCurRotateTime = 0.0f;
		mRotateTime = 0.0f;
		clearCallback();
	}
	public override void update(float elapsedTime)
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && !MathUtility.isFloatZero(mRotateTime))
		{
			mCurRotateTime += elapsedTime;
			bool done = false;
			Vector3 curAngle;
			if (mCurRotateTime < mRotateTime)
			{
				curAngle = mStartAngle + (mTargetAngle - mStartAngle) * (mCurRotateTime / mRotateTime);
			}
			else
			{
				curAngle = mTargetAngle;
				done = true;
			}
			applyRotation(curAngle, done);
			afterApplyRotation(done);
		}
		base.update(elapsedTime);
	}
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(ComponentRotateToTarget);
	}
	public override void stop()
	{
		base.stop();
		mCurRotateTime = 0.0f;
	}
	public float getRotatePercent()
	{
		if (MathUtility.isFloatZero(mRotateTime))
		{
			return 0.0f;
		}
		return mCurRotateTime / mRotateTime;
	}
	public float getRotateTime() { return mRotateTime; }
	public float getCurRotateTime() { return mCurRotateTime; }
	public Vector3 getTargetAngle() { return mTargetAngle; }
	public void setRotateTime(float time) { mRotateTime = time; }
	public void setTargetAngle(Vector3 target) { mTargetAngle = target; }
	public void startRotateToTarget(Vector3 targetAngle, Vector3 startAngle, float rotateTime, float timeOffset)
	{
		pause(false);
		mRotateTime = rotateTime;
		mStartAngle = startAngle;
		mTargetAngle = targetAngle;
		mCurRotateTime = timeOffset;

		bool isStop = MathUtility.isFloatZero(mRotateTime);
		if (!isStop)
		{
			applyRotation(mStartAngle + mCurRotateTime * (mTargetAngle - mStartAngle), false, true);
		}
		else
		{
			applyRotation(mTargetAngle, false, true);
		}
		// 如果旋转时间为0,则停止并禁用组件
		afterApplyRotation(isStop);
	}
	public void setRotatingCallback(RotateToTargetCallback callback, object userData)
	{
		setRotateCallback(callback, userData, ref mRotatingCallback, ref mRotatingUserData, this);
	}
	public void setRotateDoneCallback(RotateToTargetCallback callback, object userData)
	{
		setRotateCallback(callback, userData, ref mRotateDoneCallback, ref mRotateDoneUserData, this);
	}
	public void clearCallback()
	{
		mRotatingCallback = null;
		mRotatingUserData = null;
		mRotateDoneCallback = null;
		mRotateDoneUserData = null;
	}
	public void afterApplyRotation(bool done = false)
	{
		// 无论是否已经旋转完成,都应该先调用正在旋转的回调函数,确保数据正确
		if (mRotatingCallback != null)
		{
			mRotatingCallback(this, mRotatingUserData, false, done);
		}
		// 如果全部都旋转完成了,则停止旋转
		if (done)
		{
			setActive(false);
			doneRotateCallback(mRotateDoneCallback, mRotateDoneUserData, this);
		}
	}
	public void setRotateCallback(RotateToTargetCallback callback, object userData, ref RotateToTargetCallback curCallback, ref object curUserData, ComponentRotateToTarget component)
	{
		RotateToTargetCallback rotateCallback = curCallback;
		object tempUserData = curUserData;
		curCallback = null;
		curUserData = null;
		// 如果回调函数当前不为空,则是中断了正在进行的变化
		if (rotateCallback != null)
		{
			rotateCallback(component, tempUserData, true, false);
		}
		curCallback = callback;
		curUserData = userData;
	}
	public void doneRotateCallback(RotateToTargetCallback curDoneCallback, object curDoneUserData, ComponentRotateToTarget component)
	{
		// 先保存回调,然后再调用回调之前就清空回调,确保在回调函数执行时已经完全完成
		RotateToTargetCallback rotateCallback = curDoneCallback;
		object tempUserData = curDoneUserData;
		clearCallback();
		if (rotateCallback != null)
		{
			rotateCallback(component, tempUserData, false, true);
		}
	}
}
