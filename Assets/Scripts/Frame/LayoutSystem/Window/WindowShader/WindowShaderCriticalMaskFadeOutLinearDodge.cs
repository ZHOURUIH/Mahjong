using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderCriticalMaskFadeOutLinearDodge : WindowShaderCriticalMask
{
	protected float mFadeOutCriticalValue;
	public WindowShaderCriticalMaskFadeOutLinearDodge()
	{}
	public void setFadeOutCriticalValue(float value) { mFadeOutCriticalValue = value; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "CriticalMaskFadeOutLinearDodge")
			{
				mat.SetFloat("_FadeOutCriticalValue", mFadeOutCriticalValue);
				mat.SetFloat("_CriticalValue", mCriticalValue);
				mat.SetInt("_InverseVertical", mInverseVertical ? 1 : 0);
			}
		}
	}
}