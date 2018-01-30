using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class CommandWindowFill : Command
{
	public string mTremblingName;
	public float mStartValue;
	public float mTargetValue;
	public float mOnceLength = 0.0f;
	public float mOffset = 0.0f;
	public bool mLoop;
	public bool mFullOnce;
	public float mAmplitude;
	public KeyFrameCallback mTremblingCallBack;
	public KeyFrameCallback mTrembleDoneCallBack;
	public override void init()
	{
		base.init();
		mTremblingName = "";
		mStartValue = 0.0f;
		mTargetValue = 0.0f;
		mOnceLength = 0.0f;
		mOffset = 0.0f;
		mLoop = false;
		mFullOnce = false;
		mAmplitude = 1.0f;
		mTremblingCallBack = null;
		mTrembleDoneCallBack = null;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentFill componentFill = window.getFirstComponent<WindowComponentFill>();
		if (componentFill != null)
		{
			componentFill.setActive(true);
			componentFill.setTremblingCallback(mTremblingCallBack, null);
			componentFill.setTrembleDoneCallback(mTrembleDoneCallBack, null);
			componentFill.setStartValue(mStartValue);
			componentFill.setTargetValue(mTargetValue);
			componentFill.play(mTremblingName, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString();
	}
}