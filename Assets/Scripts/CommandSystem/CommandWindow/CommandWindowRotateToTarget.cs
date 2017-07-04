using UnityEngine;
using System.Collections;
public class CommandWindowRotateToTarget : Command
{
	public Vector3 mStartAngle;
	public Vector3 mTargetAngle;
	public float mRotateTime;
	public float mTimeOffset;
	public RotateToTargetCallback mRotatingCallback;
	public RotateToTargetCallback mRotateDoneCallback;
	public object mRotatingUserData;
	public object mRotateDoneUserData;
	public override void init()
	{
		base.init();
		mStartAngle = Vector3.zero;
		mTargetAngle = Vector3.zero;
		mRotateTime = 1.0f;
		mTimeOffset = 0.0f;
		mRotatingCallback = null;
		mRotateDoneCallback = null;
		mRotatingUserData = null;
		mRotateDoneUserData = null;
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentRotateToTarget component = window.getFirstComponent<WindowComponentRotateToTarget>();
		if (component != null)
		{
			component.setRotateDoneCallback(mRotateDoneCallback, mRotateDoneUserData);
			component.setRotatingCallback(mRotatingCallback, mRotatingUserData);
			component.setActive(true);
			component.startRotateToTarget(mTargetAngle, mStartAngle, mRotateTime, mTimeOffset);
		}
	}
	public void setRotatingCallback(RotateToTargetCallback callback, object userData)
	{
		mRotatingCallback = callback;
		mRotatingUserData = userData;
	}
	public void setRotateDoneCallback(RotateToTargetCallback callback, object userData)
	{
		mRotateDoneCallback = callback;
		mRotateDoneUserData = userData;
	}
}