using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class CommandWindowTrackTarget : Command
{
	public txUIObject mObject;
	public TrackDoneCallback mDoneCallback;
	public CheckPosition mCheckPosition;
	public float mSpeed;
	public override void init()
	{
		base.init();
		mSpeed = 0.0f;
		mDoneCallback = null;
		mCheckPosition = null;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentTrackTarget component = window.getFirstComponent<WindowComponentTrackTarget>();
		if (component != null)
		{
			component.setSpeed(mSpeed);
			component.setActive(true);
			component.setMoveDoneTrack(mObject, mDoneCallback, mCheckPosition);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + ": object name : " + mObject.getName() + ", speed value : " + mSpeed;
	}
}