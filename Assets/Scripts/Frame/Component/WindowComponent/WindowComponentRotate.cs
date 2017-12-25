using UnityEngine;
using System.Collections;
using System;

public class WindowComponentRotate : ComponentKeyFrameNormal
{
	public Vector3 mStartRotation;
	public Vector3 mTargetRotation;
	public WindowComponentRotate(Type type, string name)
		:
		base(type, name)
	{}
	public void setStartRotation(Vector3 rot){mStartRotation = rot;}
	public void setTargetRotation(Vector3 rot){	mTargetRotation = rot;}
	//-------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentRotate); }
	protected override void applyTrembling(float value)
	{
		txUIObject mObject = mComponentOwner as txUIObject;
		Vector3 curRotation = mStartRotation + (mTargetRotation - mStartRotation) * value;
		mObject.setLocalRotation(curRotation);
	}
}