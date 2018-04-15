using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptGlobalTouch : LayoutScript
{
	protected txNGUIButton mGlobalTouch;
	public ScriptGlobalTouch(string name, GameLayout layout)
		:
		base(name, layout)
	{}
	public override void assignWindow()
	{
		newObject(out mGlobalTouch, "GlobalTouch");
	}
	public override void init()
	{
		mGlobalTouch.setPressCallback(onGlobalTouchPressed);
	}
	public override void onShow(bool immediately, string param) {}
	public override void onHide(bool immediately, string param) {}
	
	//--------------------------------------------------------------------------------------------------------------------------
	protected void onGlobalTouchPressed(GameObject go, bool press)
	{
		mGlobalTouchSystem.notifyGlobalPress(press);
	}
}