using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderCriticalMask : WindowShader
{
	protected float mCriticalValue = 1.0f;
	protected bool mInverseVertical = false;
	public WindowShaderCriticalMask()
	{}
	public void setCriticalValue(float critical) { mCriticalValue = critical; }
	public void setInverseVertical(bool inverse) { mInverseVertical = inverse; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "CriticalMask")
			{
				mat.SetFloat("_CriticalValue", mCriticalValue);
				mat.SetInt("_InverseVertical", mInverseVertical ? 1 : 0);
			}
		}
	}
}