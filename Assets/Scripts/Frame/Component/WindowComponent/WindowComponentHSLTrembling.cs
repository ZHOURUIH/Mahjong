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
		txUIObject uiObj = mComponentOwner as txUIObject;
		if (uiObj == null)
		{
			UnityUtility.logError("ComponentOwner is not a window! name : " + mComponentOwner.getName());
			return;
		}
		if(uiObj.getUIType() == UI_OBJECT_TYPE.UBT_STATIC_TEXTURE)
		{
			txUIStaticTextureHSLOffset window = (mComponentOwner) as txUIStaticTextureHSLOffset;
			if(window != null)
			{
				Vector3 hsl = mStartHSL + (mTargetHSL - mStartHSL) * offset;
				window.setHSLOffset(hsl);
			}
			else
			{
				UnityUtility.logError("window is not a hsl window! name : " + mComponentOwner.getName() + ", layout : " + uiObj.mLayout.getName());
			}
		}
		else if(uiObj.getUIType() == UI_OBJECT_TYPE.UBT_TEXTURE_ANIM)
		{
			txUITextureAnimHSLOffset window = (mComponentOwner) as txUITextureAnimHSLOffset;
			if(window != null)
			{
				Vector3 hsl = mStartHSL + (mTargetHSL - mStartHSL) * offset;
				window.setHSLOffset(hsl);
			}
			else
			{
				UnityUtility.logError("window is not a hsl window! name : " + mComponentOwner.getName() + ", layout : " + uiObj.mLayout.getName());
			}
		}
		else
		{
			UnityUtility.logError("window is not a texture window! name : " + mComponentOwner.getName() + ", layout : " + uiObj.mLayout.getName());
		}
	}
}