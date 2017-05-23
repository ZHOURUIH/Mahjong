using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptGlobalTouch : LayoutScript
{
	protected txUIButton mGlobalTouch;
	
	public ScriptGlobalTouch(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{}
	public override void assignWindow()
	{
		mGlobalTouch = newObject<txUIButton>("GlobalTouch", -1);
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