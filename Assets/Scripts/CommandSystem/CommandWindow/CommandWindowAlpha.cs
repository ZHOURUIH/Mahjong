using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CommandWindowAlpha : Command
{
	public float mStartAlpha = 1.0f;
	public float mTargetAlpha = 1.0f;
	public float mFadeTime = 0.0f;
	public float mTimeOffset = 0.0f;
	public AlphaFadeCallback mFadingCallback = null;
	public AlphaFadeCallback mFadeDoneCallBack = null;
	public object mFadingData = null;
	public object mFadeDoneData = null;
	public override void init()
	{
		base.init();
		mStartAlpha = 1.0f;
		mTargetAlpha = 1.0f;
		mFadeTime = 0.0f;
		mTimeOffset = 0.0f;
		mFadingCallback = null;
		mFadeDoneCallBack = null;
		mFadingData = null;
		mFadeDoneData = null;
	}
	public void setFadingCallback(AlphaFadeCallback callback, object userData)
	{
		mFadingCallback = callback;
		mFadingData = userData;
	}
	public void setFadeDoneCallback(AlphaFadeCallback callBack, object userData)
	{
		mFadeDoneCallBack = callBack;
		mFadeDoneData = userData;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentAlpha componentAlpha = window.getFirstComponent<WindowComponentAlpha>();
		if (componentAlpha != null)
		{
			componentAlpha.setActive(true);
			componentAlpha.setFadingCallback(mFadingCallback, mFadingData);
			componentAlpha.setFadeDoneCallback(mFadeDoneCallBack, mFadeDoneData);
			componentAlpha.start(mStartAlpha, mTargetAlpha, mFadeTime, mTimeOffset);
		}
	}
}
