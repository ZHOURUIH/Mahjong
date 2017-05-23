using System;
using UnityEngine;
using System.Collections;

public class ComponentRotate : GameComponent
{
	public Vector3 mStartAngle;
	public PLAY_STATE mPlayState;
	public ComponentRotate(Type type, string name)
		:
		base(type, name)
	{
		mPlayState = PLAY_STATE.PS_STOP;
	}
	public override void setBaseType() 
	{
		mBaseType = typeof(ComponentRotate);
	}
	public override bool isType(Type type) 
	{ 
		return type == typeof(ComponentRotate); 
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public virtual void stop() { mPlayState = PLAY_STATE.PS_STOP; }
	public void pause(bool pause) { mPlayState = pause ? PLAY_STATE.PS_PAUSE : PLAY_STATE.PS_PLAY; }
	public void setPlayState(PLAY_STATE state)
	{
		if (mComponentOwner == null)
		{
			return;
		}
		if (state == PLAY_STATE.PS_PLAY)
		{
			pause(false);
		}
		else if (state == PLAY_STATE.PS_PAUSE)
		{
			pause(true);
		}
		else if (state == PLAY_STATE.PS_STOP)
		{
			stop();
		}
	}
	
	public PLAY_STATE getPlayState() { return mPlayState; }
	public void setStartAngle(Vector3 startAngle) { mStartAngle = startAngle; }
	public Vector3 getStartAngle() { return mStartAngle; }
	public virtual void applyRotation( Vector3 rotation, bool done = false, bool refreshNow = false) { }
	public virtual Vector3 getCurRotation() { return Vector3.zero; }
}
