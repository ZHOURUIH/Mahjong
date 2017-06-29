using UnityEngine;
using System;
using System.Collections;

public class WindowComponentRotateToTarget : ComponentRotateToTarget 
{
	public WindowComponentRotateToTarget(Type type, string name)
		:
		base(type, name)
	{ }
	public override void applyRotation(Vector3 rotation, bool done = false, bool refreshNow = false) 
	{
		txUIObject window = mComponentOwner as txUIObject;
		window.setLocalRotation(rotation)  ;
	}
	public override Vector3  getCurRotation(){return (mComponentOwner as txUIObject).getRotationEuler();}
	//------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentRotateToTarget); }
}