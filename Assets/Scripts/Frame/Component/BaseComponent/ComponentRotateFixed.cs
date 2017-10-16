using UnityEngine;
using System;
using System.Collections;

public class ComponentRotateFixed : GameComponent
{
	public Vector3 mFixedEuler = Vector3.zero;
	public ComponentRotateFixed(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		txUIObject uiObj = mComponentOwner as txUIObject;
		uiObj.setWorldRotation(mFixedEuler);
		base.update(elapsedTime);
	}
	public void setFixedEuler(Vector3 euler) { mFixedEuler = euler; }
	public Vector3 getFiexdEuler() { return mFixedEuler; }
	//---------------------------------------------------------------------------------------------------------------
	protected override void setBaseType(){mBaseType = typeof(ComponentRotateFixed);	}
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(ComponentRotateFixed); }
}
