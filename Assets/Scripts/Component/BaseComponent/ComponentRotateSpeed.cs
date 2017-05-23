using System;
using UnityEngine;
using System.Collections;

public class ComponentRotateSpeed : ComponentRotate
{
	public Vector3 mRotateSpeed;				// 欧拉角旋转速度
	public Vector3 mRotateAcceleration;			// 旋转加速度
	public Vector3 mCurRotation;

	public ComponentRotateSpeed(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime) 
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && 
			(!MathUtility.isFloatZero(MathUtility.getLength(mRotateSpeed)) || 
			!MathUtility.isFloatZero(MathUtility.getLength(mRotateAcceleration))))
		{
			mCurRotation += mRotateSpeed * elapsedTime;
			MathUtility.adjustAngle360(ref mCurRotation, false);
			applyRotation(mCurRotation, false);
			mRotateSpeed += mRotateAcceleration * elapsedTime;
		}
		 base.update(elapsedTime);
	}
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(ComponentRotateSpeed);
	}
	public Vector3 getRotateSpeed() { return mRotateSpeed; }
	public Vector3 getRotateAcceleration() { return mRotateAcceleration; }
	public void setRotateSpeed( Vector3 speed) { mRotateSpeed = speed; }
	public void setRotateAcceleration( Vector3 acceleration) { mRotateAcceleration = acceleration; }
	public void startRotateSpeed(Vector3 startAngle, Vector3 rotateSpeed, Vector3 rotateAcceleration) 
	{
		pause(false);
		mCurRotation = startAngle;
		mRotateSpeed = rotateSpeed;
		mRotateAcceleration = rotateAcceleration;
		applyRotation(mCurRotation, false, true);
		// 如果速度和加速度都为0,则停止旋转
		if (MathUtility.isFloatZero(MathUtility.getLength(rotateSpeed)) && MathUtility.isFloatZero(MathUtility.getLength(rotateAcceleration)))
		{
			setActive(false);
		}
	}
}
