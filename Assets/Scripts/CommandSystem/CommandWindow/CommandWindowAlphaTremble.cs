using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandWindowAlphaTremble : Command
{
	public string mName;
	public float mOnceLength;
	public float mOffset;
	public float mStartAlpha;
	public float mTargetAlpha;
	public bool mLoop;
	public float mAmplitude = 1.0f;
	public bool mFullOnce;
	public bool mRandomOffset;
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
		mStartAlpha = 1.0f;
		mTargetAlpha = 1.0f;
		mLoop = false;
		mAmplitude = 1.0f;
		mFullOnce = true;
		mRandomOffset = false;
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
		WindowComponentAlphaTrembling comTrembling = window.getFirstComponent<WindowComponentAlphaTrembling>();
		if (comTrembling != null)
		{
			comTrembling.setTremblingCallback(mTremblingCallBack, mTremblingUserData);
			comTrembling.setTrembleDoneCallback(mTrembleDoneCallBack, mTrembleDoneUserData);
			// 随机偏移量
			if (mRandomOffset)
			{
				mOffset = MathUtility.randomFloat(0.0f, mOnceLength);
			}
			comTrembling.setActive(true);
			comTrembling.setStartAlpha(mStartAlpha);
			comTrembling.setTargetAlpha(mTargetAlpha);
			comTrembling.play(mName, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		}
	}
}
