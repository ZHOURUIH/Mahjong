using System;
using UnityEngine;
using System.Collections;

public class ComponentRotateSpeedPhysics : ComponentRotateSpeedBase
{
	public ComponentRotateSpeedPhysics(Type type, string name)
		:
		base(type, name)
	{}
	public override void fixedUpdate(float elapsedTime) 
	{
		if (mPlayState == PLAY_STATE.PS_PLAY && 
			(!isFloatZero(getLength(mRotateSpeed)) || 
			!isFloatZero(getLength(mRotateAcceleration))))
		{
			mCurRotation += mRotateSpeed * elapsedTime;
			adjustAngle360(ref mCurRotation);
			applyRotation(mCurRotation, false);
			mRotateSpeed += mRotateAcceleration * elapsedTime;
		}
		base.fixedUpdate(elapsedTime);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(ComponentRotateSpeedPhysics);}
}