using UnityEngine;
using System.Collections;

public class CommandWindowScale : Command
{
	public ScaleCallback mScalingCallback;
	public ScaleCallback mScaleDoneCallback;
	public object		 mScalingUserData;
	public object		 mScaleDoneUserData;
	public float mTimeOffset;				// 缩放的起始时间偏移
	public float mScaleTime;				// 缩放的总时间
	public Vector2 mStartScale;				// 窗口起始缩放值
	public Vector2 mTargetScale;			// 窗口的目标缩放值
	public override void init()
	{
		base.init();
		mScalingCallback = null;
		mScaleDoneCallback = null;
		mScalingUserData = null;
		mScaleDoneUserData = null;
		mTimeOffset = 0.0f;
		mScaleTime = 0.0f;
		mStartScale = Vector2.one;
		mTargetScale = Vector2.one;
	}
	public override void execute()
	{
		txUIObject window = (txUIObject)(mReceiver);
		WindowComponentScale componentScale = window.getFirstComponent<WindowComponentScale>();
		if (componentScale != null)
		{
			componentScale.setActive(true);
			componentScale.setScalingCallback(mScalingCallback, mScalingUserData);
			componentScale.setScaleDoneCallback(mScaleDoneCallback, mScaleDoneUserData);
			componentScale.start(new Vector3(mStartScale.x, mStartScale.y, 1.0f), new Vector3(mTargetScale.x, mTargetScale.y, 1.0f), mScaleTime, mTimeOffset);
		}
	}
	public void setScalingCallback(ScaleCallback scalCallback, object userData)
	{
		mScalingCallback = scalCallback;
		mScalingUserData = userData;
	}
	public void setScaleDoneCallback(ScaleCallback scalCallback, object userData)
	{
		mScaleDoneCallback = scalCallback;
		mScaleDoneUserData = userData;
	}
}