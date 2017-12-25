using UnityEngine;
using System;
using System.Collections;

public class WindowComponentRotateFixed : ComponentRotateFixedNormal
{
	public WindowComponentRotateFixed(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		txUIObject obj = mComponentOwner as txUIObject;
		obj.setWorldRotation(mFixedEuler);
		base.update(elapsedTime);
	}
	//---------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentRotateFixed); }
}
