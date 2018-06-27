using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderGrey : WindowShader
{
	protected bool mGrey = false;
	public WindowShaderGrey()
	{ }
	public void setGrey(bool grey){mGrey = grey;}
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "Grey")
			{
				mat.SetInt("_Grey", mGrey ? 1 : 0);
			}
		}
	}
}