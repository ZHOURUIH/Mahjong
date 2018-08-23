using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentHSL : ComponentKeyFrameNormal
{
	public Vector3 mStartHSL;
	public Vector3 mTargetHSL;
	public WindowComponentHSL(Type type, string name)
		:
	base(type, name)
	{ }
	public void setStartHSL(Vector3 hsl) { mStartHSL = hsl; }
	public void setTargetHSL(Vector3 hsl) { mTargetHSL = hsl; }
	//------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentHSL); }
	protected override void applyTrembling(float offset)
	{
		txUIObject uiObj = mComponentOwner as txUIObject;
		txNGUITexture staticTexture = uiObj as txNGUITexture;
		if(staticTexture == null)
		{
			logError("window is not a texture window! can not offset hsl!");
			return;
		}
		WindowShaderHSLOffset hslOffset = staticTexture.getWindowShader<WindowShaderHSLOffset>();
		if(hslOffset == null)
		{
			logError("window has no hsl offset shader! can not offset hsl!");
			return;
		}
		Vector3 hsl = mStartHSL + (mTargetHSL - mStartHSL) * offset;
		hslOffset.setHSLOffset(hsl);
	}
}