using UnityEngine;
using System.Collections;

public class CommandLayoutManagerUnloadLayout : Command 
{
	public LAYOUT_TYPE	mLayoutType = LAYOUT_TYPE.LT_MAX;
	public override void init()
	{
		base.init();
		mLayoutType = LAYOUT_TYPE.LT_MAX;
	}
	public override void execute()
	{
		GameLayoutManager layoutManager = mReceiver as GameLayoutManager;
		layoutManager.destroyLayout(mLayoutType);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : layout type : " + mLayoutType;
	}
}