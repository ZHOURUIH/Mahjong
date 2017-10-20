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
		txUIStaticTextureHSLOffset window = (mComponentOwner) as txUIStaticTextureHSLOffset;
		if(window != null)
		{
			Vector3 hsl = mStartHSL + (mTargetHSL - mStartHSL) * offset;
			window.setHSLOffset(hsl);
		}
		else if(mComponentOwner != null)
		{
			txUIObject obj = mComponentOwner as txUIObject;
			if(obj != null)
			{
				UnityUtility.logError("window is not a hsl window! name : " + mComponentOwner.getName() + ", layout : " + obj.mLayout.getName());
			}
			else
			{
				UnityUtility.logError("ComponentOwner is not a window! name : " + mComponentOwner.getName());
			}
		}
	}
}