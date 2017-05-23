
using UnityEngine;
using System.Collections;

public class CommandWindowKeyFrameRotate : Command
{
	public string mName;
	public float mOnceLength;
	public float mOffset;
	public bool mLoop;
	public float mAmplitude;
	public bool mFullOnce;
	public bool mRandomOffset;
	public Vector3 mStartRotation;
	public Vector3 mTargetRotation;
	public KeyFrameCallback mTremblingCallBack;
	public object mTremblingUserData;
	public KeyFrameCallback mTrembleDoneCallBack;
	public object mTrembleDoneUserData;
	public CommandWindowKeyFrameRotate(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
		mAmplitude = 1.0f;
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentKeyFrameRotate component = window.getFirstComponent<WindowComponentKeyFrameRotate>();
		if (component != null)
		{
			component.setTremblingCallback(mTremblingCallBack, mTremblingUserData);
			component.setTrembleDoneCallback(mTrembleDoneCallBack, mTrembleDoneUserData);
			component.setActive(true);
			if (mRandomOffset)
			{
				mOffset = MathUtility.randomFloat(0.0f, mOnceLength);
			}
			component.setTargetRotation(mTargetRotation);
			component.setStartRotation(mStartRotation);
			component.play(mName, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		}
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
}
