using UnityEngine;
using System.Collections;

public class CommandWindowActive : Command
{
	public bool mActive;
	public override void init()
	{
		base.init();
		mActive = false;
	}
	public override void execute()
	{
		txUIObject uiObjcet = (txUIObject)(mReceiver);
		uiObjcet.setActive(mActive);
	}
}