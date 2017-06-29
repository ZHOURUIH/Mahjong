using UnityEngine;
using System;
using System.Collections;

public class WindowComponentAlphaTrembling : ComponentKeyFrame
{
	public float mStartAlpha;
	public float mTargetAlpha;
	public WindowComponentAlphaTrembling(Type type, string name)
		:
		base(type, name)
	{}
	public override void applyTrembling(float offset) 
	{
		txUIObject mObject = mComponentOwner as txUIObject;
		float newAlpha = mStartAlpha + (mTargetAlpha - mStartAlpha) * offset;
		mObject.setAlpha(newAlpha);
	}
	public void setStartAlpha(float alpha) {mStartAlpha = alpha;}
	public void setTargetAlpha(float alpha) {mTargetAlpha = alpha;}
	//------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentAlphaTrembling); }
}
