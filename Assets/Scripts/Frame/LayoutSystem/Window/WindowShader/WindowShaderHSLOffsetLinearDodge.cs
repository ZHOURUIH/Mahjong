using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderHSLOffsetLinearDodge : WindowShaderHSLOffset
{
	public WindowShaderHSLOffsetLinearDodge()
	{
		;
	}
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "HSLOffsetLinearDodge")
			{
				mat.SetColor("_HSLOffset", new Color(mHSLOffset.x, mHSLOffset.y, mHSLOffset.z));
				mat.SetTexture("_HSLTex", mHSLTexture);
				mat.SetInt("_HasHSLTex", mHSLTexture == null ? 0 : 1);
			}
		}
	}
}