using UnityEngine;
using System.Collections;

public class CommandCameraRotateSpeed : Command
{
	public Vector3 mStartAngle;
	public Vector3 mRotateSpeed;
	public Vector3 mRotateAcceleration;
	public override void init()
	{
		base.init();
		mStartAngle = Vector3.zero;
		mRotateSpeed = Vector3.zero;
		mRotateAcceleration = Vector3.zero;
	}
	public override void execute()
	{
		GameCamera camera = (mReceiver) as GameCamera;
		CameraComponentRotateSpeed component = camera.getFirstComponent<CameraComponentRotateSpeed>();
		if (component != null)
		{
			component.setActive(true);
			component.startRotateSpeed(mStartAngle, mRotateSpeed, mRotateAcceleration);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : start angle : " + mStartAngle + ", rotate speed : " + mRotateSpeed + ", rotate acceleration : "+ mRotateAcceleration;
	}
}