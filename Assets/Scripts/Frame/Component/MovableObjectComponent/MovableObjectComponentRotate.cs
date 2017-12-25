using UnityEngine;
using System.Collections;
using System;

public class MovableObjectComponentRotate : ComponentKeyFrameNormal
{
	public Vector3 mStartRotation;
	public Vector3 mTargetRotation;
	public MovableObjectComponentRotate(Type type, string name)
		:
		base(type, name)
	{}
	public void setStartRotation(Vector3 rot){mStartRotation = rot;}
	public void setTargetRotation(Vector3 rot){	mTargetRotation = rot;}
	//-------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentRotate); }
	protected override void applyTrembling(float value)
	{
		MovableObject obj = mComponentOwner as MovableObject;
		Vector3 curRotation = mStartRotation + (mTargetRotation - mStartRotation) * value;
		obj.setRotation(curRotation);
	}
}