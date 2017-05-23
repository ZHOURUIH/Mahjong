using UnityEngine;
using System.Collections;

public class CommandWindowMove : Command
{
	public float mTimeOffset;
	public float mMoveTime;
	public Vector3 mStartPosition;
	public Vector3 mDestPosition;
	public MoveCallback mMovingCallback;
	public object mMovingUserData;
	public MoveCallback mMoveDoneCallback;
	public object mMoveDoneUserData;

	public CommandWindowMove(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
		mMoveTime = 0.0f;
		mTimeOffset = 0.0f;
		mMovingCallback = null;
		mMovingUserData = null;
		mMoveDoneCallback = null;
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
