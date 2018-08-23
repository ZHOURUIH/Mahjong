using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentLum : ComponentKeyFrameNormal
{
	public float mStartLum;
	public float mTargetLum;
	public WindowComponentLum(Type type, string name)
		:
	base(type, name)
	{ }
	public void setStartLum(float lum) { mStartLum = lum; }
	public void setTargetLum(float lum) { mTargetLum = lum; }
	//------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentLum); }
	protected override void applyTrembling(float offset)
	{
		txUIObject uiObj = mComponentOwner as txUIObject;
		txNGUITexture staticTexture = uiObj as txNGUITexture;
		if(staticTexture == null)
		{
			logError("window is not a texture window!");
			return;
		}
		WindowShaderLumOffset lumOffset = staticTexture.getWindowShader<WindowShaderLumOffset>();
		if(lumOffset == null)
		{
			logError("window has no WindowShaderLumOffset!");
			return;
		}
		float lum = mStartLum + (mTargetLum - mStartLum) * offset;
		lumOffset.setLumOffset(lum);
	}
}