using UnityEngine;
using System.Collections;
using System;

public class WindowComponentScaleTrembling : ComponentKeyFrame
{
	public Vector2 mStartScale;
	public Vector2 mTargetScale;
	public WindowComponentScaleTrembling(Type type, string name)
		:
		base(type, name)
	{}
	public void setStartScale(Vector2 start){mStartScale = start;}
	public void setTargetScale(Vector2 target){mTargetScale = target;}
	//--------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentScaleTrembling); }
	protected override void applyTrembling(float value)
	{
		txUIObject mObject = mComponentOwner as txUIObject;
		Vector3 curScale = mObject.getScale();
		Vector2 newSacle = mStartScale + (mTargetScale - mStartScale) * value;
		mObject.setLocalScale(new Vector3(newSacle.x, newSacle.y, curScale.z));
	}
}