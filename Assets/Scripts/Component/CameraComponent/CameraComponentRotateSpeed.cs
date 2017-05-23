using UnityEngine;
using System;
using System.Collections;

public class CameraComponentRotateSpeed : ComponentRotateSpeed
{
	public CameraComponentRotateSpeed(Type type, string name)
		:
		base(type, name)
	{ }
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(CameraComponentRotateSpeed);
	}
	public override void applyRotation(Vector3 rotation, bool done = false, bool refreshNow = false)
	{
		GameCamera window = mComponentOwner as GameCamera;
		window.setRotation(rotation);
	}
	public override Vector3 getCurRotation()
	{
		Vector3 rot = (mComponentOwner as GameCamera).getRotation();
		return rot;
	}
}