using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CommandMovableObjectTrackTarget : Command
{
	public MovableObject mObject;
	public TrackDoneCallback mDoneCallback;
	public float mSpeed;
	public Vector3 mOffset;
	public override void init()
	{
		base.init();
		mSpeed = 0.0f;
		mDoneCallback = null;
		mObject = null;
		mOffset = Vector3.zero;
	}
	public override void execute()
	{
		MovableObject obj = mReceiver as MovableObject;
		MovableObjectComponentTrackTarget component = obj.getFirstComponent<MovableObjectComponentTrackTarget>();
		if (component != null)
		{

			component.setSpeed(mSpeed);
			component.setTargetOffset(mOffset);
			component.setActive(true);
			component.setMoveDoneTrack(mObject, mDoneCallback);
		}
	}
	public override string showDebugInfo()
	{
		string target = mObject != null ? mObject.getName() : "";
		return this.GetType().ToString() + ": object name : " + target + ", speed value : " + mSpeed;
	}
}