using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CommandWindowAlpha : Command
{
	public float mStartAlpha;
	public float mTargetAlpha;
	public float mFadeTime;
	public float mTimeOffset;
	public AlphaFadeCallback mFadingCallback;
	public AlphaFadeCallback mFadeDoneCallBack;
	public object mFadingData;
	public object mFadeDoneData;

	public CommandWindowAlpha(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
		mStartAlpha = 1.0f;
		mTargetAlpha = 1.0f;
		mFadeTime = 0.0f;
		mTimeOffset = 0.0f;
		mFadingCallback = null;
		mFadingData = null;
		mFadeDoneCallBack = null;
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
