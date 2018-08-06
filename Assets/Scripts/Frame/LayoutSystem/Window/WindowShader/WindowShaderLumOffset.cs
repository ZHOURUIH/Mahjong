using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderLumOffset : WindowShader
{
	protected float mLumOffset = 0.0f;
	public WindowShaderLumOffset()
	{ }
	public void setLumOffset(float lumOffset){ mLumOffset = lumOffset;}
	public float getLumOffset() { return mLumOffset; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "LumOffset")
			{
				mat.SetFloat("_LumOffset", mLumOffset);
			}
		}
	}
}