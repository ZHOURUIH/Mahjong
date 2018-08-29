using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMainFrameBack : LayoutScript
{
	protected txNGUITexture mBackground;
	public ScriptMainFrameBack(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		;
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	//-------------------------------------------------------------------------------------------------------------------------
}