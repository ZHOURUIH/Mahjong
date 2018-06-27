using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderLumOffsetLinearDodge : WindowShaderLumOffset
{
	public WindowShaderLumOffsetLinearDodge()
	{ }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "LumOffsetLinearDodge")
			{
				mat.SetFloat("_LumOffset", mLumOffset);
			}
		}
	}
}