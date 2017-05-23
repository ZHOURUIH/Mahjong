using UnityEngine;
using System.Collections;
using System;

public class WindowComponentKeyFrameRotate : ComponentKeyFrame
{
	public Vector3 mStartRotation;
	public Vector3 mTargetRotation;
	public WindowComponentKeyFrameRotate(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(WindowComponentKeyFrameRotate);
	}
	public override void applyTrembling(float value)
	{
		txUIObject mObject = mComponentOwner as txUIObject;
		Vector3 curRotation = mStartRotation + (mTargetRotation - mStartRotation) * value;
		mObject.setLocalRotation(curRotation);
	}
	public void setStartRotation(Vector3 rot)
	{
		mStartRotation = rot;
	}
	public void setTargetRotation(Vector3 rot)
	{
		mTargetRotation = rot;
	}
}