using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class CommandWindowHSL : Command
{
	public Vector3 mStartHSL = Vector3.one;
	public Vector3 mTargetHSL = Vector3.one;
	public float mFadeTime = 1.0f;
	public float mTimeOffset = 0.0f;
	public bool mFadeChildren = true;
	public HSLCallback mFadingCallback = null;
	public object mFadingUserData = null;
	public HSLCallback mFadeDoneCallback = null;
	public object mFadeDoneUserData = null;

	public CommandWindowHSL(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		txUIObject window = (txUIObject)(mReceiver);
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

