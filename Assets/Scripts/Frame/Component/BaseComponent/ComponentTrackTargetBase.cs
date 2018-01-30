using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ComponentTrackTargetBase : GameComponent
{
	protected object mTarget;
	protected float mSpeed;
	protected Vector3 mTargetOffset;
	protected TrackDoneCallback mDoneCallback;
	public ComponentTrackTargetBase(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public virtual void setMoveDoneTrack(object target, TrackDoneCallback doneCallback)
	{
		mTarget = target;
		mDoneCallback = doneCallback;
		if(mTarget == null)
		{
			setActive(false);
		}
	}
	public void setSpeed(float speed) { mSpeed = speed; }
	public void setTargetOffset(Vector3 offset) { mTargetOffset = offset; }
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return type == typeof(ComponentTrackTargetBase); }
	protected override void setBaseType(){mBaseType = typeof(ComponentTrackTargetBase);}
	protected virtual Vector3 getPosition()
	{
		return Vector3.zero;
	}
	protected virtual void setPosition(Vector3 pos)
	{
		;
	}
	protected virtual Vector3 getTargetPosition()
	{
		return Vector3.zero;
	}
}