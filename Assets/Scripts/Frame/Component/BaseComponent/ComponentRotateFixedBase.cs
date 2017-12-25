using UnityEngine;
using System;
using System.Collections;

public class ComponentRotateFixedBase : GameComponent
{
	public Vector3 mFixedEuler = Vector3.zero;
	public ComponentRotateFixedBase(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public void setFixedEuler(Vector3 euler) { mFixedEuler = euler; }
	public Vector3 getFiexdEuler() { return mFixedEuler; }
	//---------------------------------------------------------------------------------------------------------------
	protected override void setBaseType(){mBaseType = typeof(ComponentRotateFixedBase);	}
	protected override bool isType(Type type) { return type == typeof(ComponentRotateFixedBase); }
}
