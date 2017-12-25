using System;
using UnityEngine;
using System.Collections;

public class ComponentRotateSpeedNormal : ComponentRotateSpeedBase
{
	public ComponentRotateSpeedNormal(Type type, string name)
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
			MathUtility.adjustAngle360(ref mCurRotation);
			applyRotation(mCurRotation, false);
			mRotateSpeed += mRotateAcceleration * elapsedTime;
		}
		base.update(elapsedTime);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(ComponentRotateSpeedNormal);}
}