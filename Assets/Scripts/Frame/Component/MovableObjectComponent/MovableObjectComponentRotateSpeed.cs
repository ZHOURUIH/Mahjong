using UnityEngine;
using System;
using System.Collections;

public class MovableObjectComponentRotateSpeed : ComponentRotateSpeedNormal
{
	public MovableObjectComponentRotateSpeed(Type type, string name)
		:
		base(type, name)
	{ }
	//-----------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(MovableObjectComponentRotateSpeed);}
	protected override Vector3 getCurRotation() { return (mComponentOwner as GameCamera).getRotation(); }
	protected override void applyRotation(Vector3 rotation, bool done = false, bool refreshNow = false)
	{
		MovableObject window = mComponentOwner as MovableObject;
		window.setRotation(rotation);
	}
}