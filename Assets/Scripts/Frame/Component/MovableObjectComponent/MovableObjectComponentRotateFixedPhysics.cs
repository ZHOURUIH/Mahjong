using UnityEngine;
using System;
using System.Collections;

public class MovableObjectComponentRotateFixedPhysics : ComponentRotateFixedPhysics
{
	public MovableObjectComponentRotateFixedPhysics(Type type, string name)
		:
		base(type, name)
	{}
	public override void fixedUpdate(float elapsedTime)
	{
		MovableObject obj = mComponentOwner as MovableObject;
		obj.setWorldRotation(mFixedEuler);
		base.fixedUpdate(elapsedTime);
	}
	//---------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentRotateFixedPhysics); }
}
