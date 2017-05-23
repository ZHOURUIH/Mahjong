using UnityEngine;
using System.Collections;

public class CommandWindowActive : Command
{
	public bool mActive;
	public CommandWindowActive(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		txUIObject uiObjcet = (txUIObject)(mReceiver);
		uiObjcet.setActive(mActive);
	}
}
