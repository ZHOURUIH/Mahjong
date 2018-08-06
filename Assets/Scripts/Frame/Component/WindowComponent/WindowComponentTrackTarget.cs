using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentTrackTarget : ComponentTrackTargetNormal
{
	protected CheckPosition mCheckPosition;
	public WindowComponentTrackTarget(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void setMoveDoneTrack(object target, TrackDoneCallback doneCallback)
	{
		logError("please use void setMoveDoneTrack(txUIObject target, TrackDoneCallback doneCallback, CheckPosition checkPosition)");
	}
	public void setMoveDoneTrack(txUIObject target, TrackDoneCallback doneCallback, CheckPosition checkPosition)
	{
		base.setMoveDoneTrack(target, doneCallback);
		mCheckPosition = checkPosition;
	}
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentTrackTarget); }
	protected override Vector3 getPosition()
	{
		return (mComponentOwner as txUIObject).getPosition();
	}
	protected override void setPosition(Vector3 pos)
	{
		(mComponentOwner as txUIObject).setLocalPosition(pos);
	}
	protected override Vector3 getTargetPosition()
	{
		return mCheckPosition(mTarget as txUIObject);
	}
}