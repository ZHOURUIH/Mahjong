
using UnityEngine;
using System.Collections;

public class CommandWindowRotate : Command
{
	public string mName;
	public float mOnceLength;
	public float mOffset;
	public bool mLoop;
	public float mAmplitude = 1.0f;
	public bool mFullOnce;
	public bool mRandomOffset;
	public Vector3 mStartRotation;
	public Vector3 mTargetRotation;
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
		mStartRotation = Vector3.zero;
		mTargetRotation = Vector3.zero;
		mTremblingCallBack = null;
		mTrembleDoneCallBack = null;
		mTremblingUserData = null;
		mTrembleDoneUserData = null;
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentRotate component = window.getFirstComponent<WindowComponentRotate>();
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
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + ": name : " + mName + ", once length : " + mOnceLength + ", offset : " + mOffset + ", loop : " + mLoop + ", amplitude : " + mAmplitude + ", full once : " + mFullOnce + ", random offset : " + mRandomOffset + ", start totation : " + mStartRotation + ", target rotation : " + mTargetRotation;
	}
}