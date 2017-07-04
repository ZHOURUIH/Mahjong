using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandWindowScaleTrembling : Command
{
	public string mName;
	public float mOnceLength;
	public float mOffset;
	public bool mLoop;
	public float mAmplitude = 1.0f;
	public bool mFullOnce;
	public bool mRandomOffset;
	public Vector2 mStartScale;
	public Vector2 mTargetScale;
	public KeyFrameCallback mTremblingCallBack;
	public KeyFrameCallback mTrembleDoneCallBack;
	public object mTremblingUserData;
	public object mTrembleDoneUserData;
	public override void init()
	{
		base.init();
		mName = "";
		mOnceLength = 1.0f;
		mOffset = 0.0f;
		mLoop = false;
		mAmplitude = 1.0f;
		mFullOnce = true;
		mRandomOffset = false;
		mStartScale = Vector2.one;
		mTargetScale = Vector2.one;
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
		WindowComponentScaleTrembling comTrembling = window.getFirstComponent<WindowComponentScaleTrembling>();
		if (null != comTrembling)
		{
			comTrembling.setActive(true);
			comTrembling.setTremblingCallback(mTremblingCallBack, mTremblingUserData);
			comTrembling.setTrembleDoneCallback(mTrembleDoneCallBack, mTrembleDoneUserData);
			// 随机偏移量
			if (mRandomOffset)
			{
				mOffset = MathUtility.randomFloat(0.0f, mOnceLength);
			}
			comTrembling.setStartScale(mStartScale);
			comTrembling.setTargetScale(mTargetScale);
			comTrembling.play(mName, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		}
	}
}