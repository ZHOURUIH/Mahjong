using UnityEngine;
using System;
using System.Collections;

public class WindowComponentAlphaTrembling : ComponentKeyFrame
{
	protected float mStartAlpha;
	protected float mTargetAlpha;
	public WindowComponentAlphaTrembling(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(WindowComponentAlphaTrembling);
	}
	public override void applyTrembling(float offset) 
	{
		txUIObject mObject = mComponentOwner as txUIObject;
		float alpha = mStartAlpha + (mTargetAlpha - mStartAlpha) * offset;
		mObject.setAlpha(alpha);
	}
	public void setTargetAlpha(float target) { mTargetAlpha = target; }
	public void setStartAlpha(float start) { mStartAlpha = start; }
}
