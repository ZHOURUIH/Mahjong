using UnityEngine;
using System.Collections;

public class CommandCameraRotateSpeed : Command
{
	public Vector3 mStartAngle;
	public Vector3 mRotateSpeed;
	public Vector3 mRotateAcceleration;
	public CommandCameraRotateSpeed(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
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
}