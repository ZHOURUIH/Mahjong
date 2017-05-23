
using UnityEngine;
using System.Collections;

public class CommandWindowKeyFrameMove : Command
{
	public	string mName;
	public	float mOnceLength;
	public	float mOffset;
	public	bool mLoop;
	public	float mAmplitude;
	public	bool mFullOnce;
	public	bool mRandomOffset;
	public Vector3 mStartPos;
	public Vector3 mTargetPos;
	public	KeyFrameCallback mTremblingCallBack;
	public	object mTremblingUserData;
	public	KeyFrameCallback mTrembleDoneCallBack;
	public	object mTrembleDoneUserData;
	public CommandWindowKeyFrameMove (bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
 		mAmplitude = 1.0f;	
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentKeyFrameMove component = window.getFirstComponent<WindowComponentKeyFrameMove>();
		if (component != null)
		{
			component.setTremblingCallback(mTremblingCallBack, mTremblingUserData);
			component.setTrembleDoneCallback(mTrembleDoneCallBack, mTrembleDoneUserData);
			component.setActive(true);
			if (mRandomOffset)
			{
				mOffset = MathUtility.randomFloat(0.0f, mOnceLength);
			}
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
}
