using UnityEngine;
using System;
using System.Collections;

public class ComponentRotateFixedPhysics : ComponentRotateFixedBase
{
	public ComponentRotateFixedPhysics(Type type, string name)
		:
		base(type, name)
	{ }
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
	}
	//---------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(ComponentRotateFixedPhysics); }
}
