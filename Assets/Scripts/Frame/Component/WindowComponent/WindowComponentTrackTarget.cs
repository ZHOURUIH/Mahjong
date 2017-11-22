using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentTrackTarget : GameComponent
{
	protected txUIObject mTarget;
	protected float mSpeed;
	protected TrackDoneCallback mDoneCallback;
	protected CheckPosition mCheckPosition;
	public WindowComponentTrackTarget(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		if(mTarget != null)
		{
			Vector3 targetPos = mCheckPosition(mTarget);
			Vector3 curPos = (mComponentOwner as txUIObject).getPosition();
			float remainDis = (targetPos - curPos).magnitude;
			Vector3 newPos;
			bool done = false;
			if (remainDis > mSpeed * elapsedTime)
			{
				newPos = Vector3.Normalize(targetPos - curPos) * mSpeed + curPos;
			}
			else
			{
				newPos = targetPos;
				done = true;
			}
			(mComponentOwner as txUIObject).setLocalPosition(newPos);

			if (done && mDoneCallback != null)
			{
				mDoneCallback(this);
			}
		}
		base.update(elapsedTime);
	}
	public void setMoveDoneTrack(txUIObject target, TrackDoneCallback doneCallback, CheckPosition checkPosition)
	{
		mTarget = target;
		mDoneCallback = doneCallback;
		mCheckPosition = checkPosition;
	}
	public float setSpeed(float speed) { return mSpeed = speed; }
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentTrackTarget); }
	protected override void setBaseType(){mBaseType = typeof(WindowComponentTrackTarget);}
}