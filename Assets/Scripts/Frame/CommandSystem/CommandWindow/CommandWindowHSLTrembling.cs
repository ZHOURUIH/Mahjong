using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandWindowHSLTremble : Command
{
	public string mName;
	public float mOnceLength;
	public float mOffset;
	public Vector3 mStartHSL;
	public Vector3 mTargetHSL;
	public bool mLoop;
	public float mAmplitude = 1.0f;
	public bool mFullOnce;
	public KeyFrameCallback mTremblingCallBack = null;
	public KeyFrameCallback mTrembleDoneCallBack = null;
	public object mTremblingUserData = null;
	public object mTrembleDoneUserData = null;
	public override void init()
	{
		base.init();
		mName = "";
		mOnceLength = 1.0f;
		mOffset = 0.0f;
		mStartHSL = Vector3.zero;
		mTargetHSL = Vector3.zero;
		mLoop = false;
		mAmplitude = 1.0f;
		mFullOnce = true;
		mTremblingCallBack = null;
		mTrembleDoneCallBack = null;
		mTremblingUserData = null;
		mTrembleDoneUserData = null;
	}
	public void setTremblingCallback(KeyFrameCallback callback, object userData)
	{
		mTremblingCallBack = callback;
		mTremblingUserData = userData;
	}
	public void setTrembleDoneCallback(KeyFrameCallback callback, object userData)
	{
		mTrembleDoneCallBack = callback;
		mTrembleDoneUserData = userData;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentHSLTrembling comTrembling = window.getFirstComponent<WindowComponentHSLTrembling>();
		if (comTrembling != null)
		{
			comTrembling.setTremblingCallback(mTremblingCallBack, mTremblingUserData);
			comTrembling.setTrembleDoneCallback(mTrembleDoneCallBack, mTrembleDoneUserData);
			comTrembling.setActive(true);
			comTrembling.setStartHSL(mStartHSL);
			comTrembling.setTargetHSL(mTargetHSL);
			comTrembling.play(mName, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : name : " + mName + ", once length : " + mOnceLength + ", offset : " + mOffset + ", start HSL : " + mStartHSL + ", target HSL : " + mTargetHSL + ", loop : " + mLoop + ", amplitude : " + mAmplitude + ", fullOnce : " + mFullOnce;
	}
}
