using UnityEngine;
using System.Collections;
public class CommandWindowRotateSpeed : Command
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
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentRotateSpeed component = window.getFirstComponent<WindowComponentRotateSpeed>();
		if (component != null)
		{
			component.setActive(true);
			component.startRotateSpeed(mStartAngle, mRotateSpeed, mRotateAcceleration);
		}
	}
}