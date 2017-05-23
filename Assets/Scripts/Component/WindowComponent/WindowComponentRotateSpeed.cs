using UnityEngine;
using System;
using System.Collections;

public class WindowComponentRotateSpeed : ComponentRotateSpeed  
{
	public WindowComponentRotateSpeed(Type type, string name)
		:
		base(type, name)
	{ }
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(WindowComponentRotateSpeed);
	}
	public override void applyRotation(Vector3 rotation, bool done = false, bool refreshNow = false) 
	{
		txUIObject window = mComponentOwner as txUIObject;
		window.setLocalRotation(rotation);
	}
	public 	override Vector3 getCurRotation()
	{
		Vector3 rot = (mComponentOwner as txUIObject).getRotationEuler();
		return rot;
	}
}