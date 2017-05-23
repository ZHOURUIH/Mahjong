using UnityEngine;
using System.Collections;
public class CommandWindowRotateToTarget : Command
{
	public Vector3 mStartAngle;
	public Vector3 mTargetAngle;
	public float mRotateTime;
	public float mTimeOffset;
	public RotateToTargetCallback mRotatingCallback;
	public object mRotatingUserData;
	public RotateToTargetCallback mRotateDoneCallback;
	public object mRotateDoneUserData;

	public CommandWindowRotateToTarget(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
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
