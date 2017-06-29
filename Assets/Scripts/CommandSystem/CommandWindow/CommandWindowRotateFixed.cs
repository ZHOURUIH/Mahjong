using UnityEngine;
using System.Collections;

public class CommandWindowRotateFixed : Command
{
	public bool mActive;
	public Vector3 mFixedEuler;
	public CommandWindowRotateFixed (bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
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