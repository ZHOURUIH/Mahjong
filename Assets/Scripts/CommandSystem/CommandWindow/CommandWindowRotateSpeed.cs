using UnityEngine;
using System.Collections;
public class CommandWindowRotateSpeed : Command
{
	public Vector3 mStartAngle;
	public Vector3 mRotateSpeed;
	public Vector3 mRotateAcceleration;
	public CommandWindowRotateSpeed (bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
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
