using UnityEngine;
using System.Collections;
using System;

public class MovableObjectComponentRotatePhysics : ComponentKeyFramePhysics
{
	public Vector3 mStartRotation;
	public Vector3 mTargetRotation;
	public MovableObjectComponentRotatePhysics(Type type, string name)
		:
		base(type, name)
	{}
	public void setStartRotation(Vector3 rot){mStartRotation = rot;}
	public void setTargetRotation(Vector3 rot){	mTargetRotation = rot;}
	//-------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentRotatePhysics); }
	protected override void applyTrembling(float value)
	{
		MovableObject obj = mComponentOwner as MovableObject;
		Vector3 curRotation = mStartRotation + (mTargetRotation - mStartRotation) * value;
		obj.setRotation(curRotation);
	}
}