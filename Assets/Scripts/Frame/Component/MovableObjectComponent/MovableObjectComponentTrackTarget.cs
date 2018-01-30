using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MovableObjectComponentTrackTarget : ComponentTrackTargetNormal
{
	public MovableObjectComponentTrackTarget(Type type, string name)
		:
		base(type, name)
	{}
	public override void setMoveDoneTrack(object target, TrackDoneCallback doneCallback)
	{
		if(target != null && !(target is MovableObject))
		{
			UnityUtility.logError("track target must be a MovableObject!");
			return;
		}
		base.setMoveDoneTrack(target, doneCallback);
	}
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentTrackTarget); }
	protected override Vector3 getPosition()
	{
		return (mComponentOwner as MovableObject).getWorldPosition();
	}
	protected override void setPosition(Vector3 pos)
	{
		(mComponentOwner as MovableObject).setWorldPosition(pos);
	}
	protected override Vector3 getTargetPosition()
	{
		MovableObject movable = mTarget as MovableObject;
		Vector3 pos = movable.getWorldPosition();
		pos += movable.getTransform().localToWorldMatrix.MultiplyVector(mTargetOffset);
		return pos;
	}
}