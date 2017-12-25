using UnityEngine;
using System;
using System.Collections;

public class ComponentRotateFixedNormal : ComponentRotateFixedBase
{
	public ComponentRotateFixedNormal(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	//---------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(ComponentRotateFixedNormal); }
}
