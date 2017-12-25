using UnityEngine;
using System.Collections;

public class CommandWindowRotateFixed : Command
{
	public bool mActive;
	public Vector3 mFixedEuler = Vector3.zero;
	public override void init()
	{
		base.init();
		mActive = true;
		mFixedEuler = Vector3.zero;
	}
	public override void execute()
	{
		txUIObject window = (mReceiver) as txUIObject;
		WindowComponentRotateFixed component = window.getFirstComponent<WindowComponentRotateFixed>();
		if (component != null)
		{
			component.setActive(mActive);
			component.setFixedEuler(mFixedEuler);
		}	
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + ": active : " + mActive + ", fixed euler : " + mFixedEuler; 
	}
}