using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentHSLTrembling : ComponentKeyFrame
{
	public Vector3 mStartHSL;
	public Vector3 mTargetHSL;
	public WindowComponentHSLTrembling(Type type, string name)
		:
	base(type, name)
	{ }
	public void setStartHSL(Vector3 hsl) { mStartHSL = hsl; }
	public void setTargetHSL(Vector3 hsl) { mTargetHSL = hsl; }
	//------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentHSLTrembling); }
	protected override void applyTrembling(float offset)
	{
		txUIStaticTexture window = (mComponentOwner) as txUIStaticTexture;
		if(window != null)
		{
			Vector3 hsl = mStartHSL + (mTargetHSL - mStartHSL) * offset;
			window.setHSLOffset(hsl);
		}
	}
}