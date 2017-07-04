using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class CommandWindowHSL : Command
{
	public Vector3 mStartHSL = Vector3.zero;
	public Vector3 mTargetHSL = Vector3.zero;
	public float mFadeTime = 1.0f;
	public float mTimeOffset = 0.0f;
	public bool mFadeChildren = true;
	public HSLCallback mFadingCallback = null;
	public HSLCallback mFadeDoneCallback = null;
	public object mFadingUserData = null;
	public object mFadeDoneUserData = null;
	public override void init()
	{
		base.init();
		mStartHSL = Vector3.zero;
		mTargetHSL = Vector3.zero;
		mFadeTime = 1.0f;
		mTimeOffset = 0.0f;
		mFadeChildren = true;
		mFadingCallback = null;
		mFadeDoneCallback = null;
		mFadingUserData = null;
		mFadeDoneUserData = null;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentHSL componentHSL = window.getFirstComponent<WindowComponentHSL>();
		if (componentHSL != null)
		{
			componentHSL.setActive(true);
			componentHSL.setHSLFadingCallback(mFadingCallback, mFadingUserData);
			componentHSL.setHSLDoneCallback(mFadeDoneCallback, mFadeDoneUserData);
			componentHSL.start(mFadeTime, mStartHSL, mTargetHSL, mTimeOffset);
		}
	}
	public void setScalingCallback(HSLCallback hslCallback, object userData)
	{
		mFadingCallback = hslCallback;
		mFadingUserData = userData;
	}
	public void setScaleDoneCallback(HSLCallback hslCallback, object userData)
	{
		mFadeDoneCallback = hslCallback;
		mFadeDoneUserData = userData;
	}
}