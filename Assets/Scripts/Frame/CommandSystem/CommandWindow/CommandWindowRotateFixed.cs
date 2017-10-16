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
		ComponentRotateFixed component = window.getFirstComponent<ComponentRotateFixed>();
		if (component != null)
		{
			component.setActive(mActive);
			component.setFixedEuler(mFixedEuler);
		}	
	}
}