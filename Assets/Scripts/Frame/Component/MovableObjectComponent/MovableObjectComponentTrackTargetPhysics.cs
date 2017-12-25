using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MovableObjectComponentTrackTargetPhysics : ComponentTrackTargetPhysics
{
	public MovableObjectComponentTrackTargetPhysics(Type type, string name)
		:
		base(type, name)
	{}
	public override void setMoveDoneTrack(object target, TrackDoneCallback doneCallback)
	{
		if(!(target is MovableObject))
		{
			UnityUtility.logError("track target must be a MovableObject!");
			return;
		}
		base.setMoveDoneTrack(target, doneCallback);
	}
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentTrackTargetPhysics); }
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
		return (mTarget as MovableObject).getWorldPosition();
	}
}