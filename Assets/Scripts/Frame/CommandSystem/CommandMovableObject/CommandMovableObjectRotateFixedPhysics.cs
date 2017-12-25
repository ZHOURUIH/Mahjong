using UnityEngine;
using System.Collections;

public class CommandMovableObjectRotateFixedPhysics : Command
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
		MovableObject obj = (mReceiver) as MovableObject;
		MovableObjectComponentRotateFixedPhysics component = obj.getFirstComponent<MovableObjectComponentRotateFixedPhysics>();
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