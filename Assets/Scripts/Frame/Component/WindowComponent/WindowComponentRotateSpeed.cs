using UnityEngine;
using System;
using System.Collections;

public class WindowComponentRotateSpeed : ComponentRotateSpeedNormal  
{
	public WindowComponentRotateSpeed(Type type, string name)
		:
		base(type, name)
	{ }
	//-------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentRotateSpeed); }
	protected override void applyRotation(Vector3 rotation, bool done = false, bool refreshNow = false)
	{
		txUIObject window = mComponentOwner as txUIObject;
		window.setLocalRotation(rotation);
	}
	protected override Vector3 getCurRotation() { return (mComponentOwner as txUIObject).getRotationEuler(); }
}