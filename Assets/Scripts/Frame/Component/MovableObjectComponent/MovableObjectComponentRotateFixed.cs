using UnityEngine;
using System;
using System.Collections;

public class MovableObjectComponentRotateFixed : ComponentRotateFixedNormal
{
	public MovableObjectComponentRotateFixed(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		MovableObject obj = mComponentOwner as MovableObject;
		obj.setWorldRotation(mFixedEuler);
		base.update(elapsedTime);
	}
	//---------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(ComponentRotateFixedNormal); }
}
