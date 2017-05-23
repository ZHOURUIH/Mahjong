using UnityEngine;
using System;
using System.Collections;

public class CompoentRotateFixed : GameComponent
{
	protected Vector3 mFixedRotation = Vector3.zero;
	public CompoentRotateFixed(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void setBaseType() 
	{
		mBaseType = typeof(ComponentRotate);
	}
	public override bool isType(Type type) 
	{
		return base.isType(type) || type == typeof(CompoentRotateFixed);
	}
	public override void update(float elapsedTime)
	{
		txUIObject uiObj = mComponentOwner as txUIObject;
		uiObj.setWorldRotation(mFixedRotation);
		base.update(elapsedTime);
	}
	public void setFixedRotation(Vector3 rot)
	{
		mFixedRotation = rot;
	}
}
