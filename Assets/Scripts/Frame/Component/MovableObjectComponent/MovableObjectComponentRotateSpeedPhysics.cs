using UnityEngine;
using System;
using System.Collections;

public class MovableObjectComponentRotateSpeedPhysics : ComponentRotateSpeedPhysics
{
	public MovableObjectComponentRotateSpeedPhysics(Type type, string name)
		:
		base(type, name)
	{ }
	//-----------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(MovableObjectComponentRotateSpeedPhysics);}
	protected override void applyRotation(Vector3 rotation, bool done = false, bool refreshNow = false)
	{
		MovableObject obj = mComponentOwner as MovableObject;
		obj.setRotation(rotation);
	}
	protected override Vector3 getCurRotation() { return (mComponentOwner as GameCamera).getRotation(); }
}