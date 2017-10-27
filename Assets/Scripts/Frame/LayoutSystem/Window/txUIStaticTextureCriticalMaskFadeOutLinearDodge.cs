using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTextureCriticalMaskFadeOutLinearDodge : txUIStaticTextureCriticalMask
{
	protected float mFadeOutCriticalValue;
	public txUIStaticTextureCriticalMaskFadeOutLinearDodge()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
	}
	public void setFadeOutCriticalValue(float value) { mFadeOutCriticalValue = value; }
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
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