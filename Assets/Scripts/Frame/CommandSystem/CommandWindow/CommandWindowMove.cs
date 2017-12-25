
using UnityEngine;
using System.Collections;

public class CommandWindowMove : Command
{
	public string mName;
	public float mOnceLength;
	public float mOffset;
	public bool mLoop;
	public float mAmplitude = 1.0f;
	public bool mFullOnce;
	public Vector3 mStartPos;
	public Vector3 mTargetPos;
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
		mFullOnce = false;
		mStartPos = Vector3.zero;
		mTargetPos = Vector3.zero;
		mTremblingCallBack = null;
		mTrembleDoneCallBack = null;
		mTremblingUserData = null;
		mTrembleDoneUserData = null;
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentMove component = window.getFirstComponent<WindowComponentMove>();
		if (component != null)
		{
			component.setTremblingCallback(mTremblingCallBack, mTremblingUserData);
			component.setTrembleDoneCallback(mTrembleDoneCallBack, mTrembleDoneUserData);
			component.setActive(true);
			component.setTargetPos(mTargetPos);
			component.setStartPos(mStartPos);
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
		return this.GetType().ToString() + " : name : " + mName + ", once length : " + mOnceLength + ", offset : " + mOffset + ", start pos : " + mStartPos + ", target pos : " + mTargetPos + ", loop : " + mLoop + ", amplitude : " + mAmplitude + ", fullOnce : " + mFullOnce;
	}
}