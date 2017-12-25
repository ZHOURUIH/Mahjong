using UnityEngine;
using System.Collections;
using System;

public class MovableObjectComponentScale : ComponentKeyFrameNormal
{
	public Vector2 mStartScale;
	public Vector2 mTargetScale;
	public MovableObjectComponentScale(Type type, string name)
		:
		base(type, name)
	{}
	public void setStartScale(Vector2 start){mStartScale = start;}
	public void setTargetScale(Vector2 target){mTargetScale = target;}
	//--------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentScale); }
	protected override void applyTrembling(float value)
	{
		MovableObject mObject = mComponentOwner as MovableObject;
		Vector2 newSacle = mStartScale + (mTargetScale - mStartScale) * value;
		mObject.setScale(newSacle);
	}
}