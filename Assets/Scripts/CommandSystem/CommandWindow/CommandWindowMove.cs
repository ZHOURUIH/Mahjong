using UnityEngine;
using System.Collections;

public class CommandWindowMove : Command
{
	public float mTimeOffset = 0.0f;
	public float mMoveTime = 0.0f;
	public Vector3 mStartPosition;
	public Vector3 mDestPosition;
	public MoveCallback mMovingCallback;
	public MoveCallback mMoveDoneCallback;
	public object mMovingUserData;
	public object mMoveDoneUserData;
	public override void init()
	{
		base.init();
		mTimeOffset = 0.0f;
		mMoveTime = 0.0f;
		mStartPosition = Vector3.zero;
		mDestPosition = Vector3.zero;
		mMovingCallback = null;
		mMoveDoneCallback = null;
		mMovingUserData = null;
		mMoveDoneUserData = null;
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentMove comMove = window.getFirstComponent<WindowComponentMove>();
		if (comMove != null)
		{
			comMove.setMovingCallback(mMovingCallback, mMovingUserData);
			comMove.setMoveDoneCallback(mMoveDoneCallback, mMoveDoneUserData);
			comMove.setActive(true);
			comMove.start(mMoveTime, mStartPosition, mDestPosition, mTimeOffset);
		}
	}
	public void setMovingCallback(MoveCallback movingCallback, object userData)
	{
		mMovingCallback = movingCallback;
		mMovingUserData = userData;
	}
	public void setMoveDoneCallback(MoveCallback moveDoneCallback, object userData)
	{
		mMoveDoneCallback = moveDoneCallback;
		mMoveDoneUserData = userData;
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : move time : " + mMoveTime + ", time offset : " + mTimeOffset + ", start : " + mStartPosition + ", target" + mDestPosition;
	}
}