using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderCurve : WindowShader
{
	public float mMinY = 0.0f;
	public float mMaxY = 700.0f;
	public float mAlpha = 1.0f;
	public WindowShaderCurve()
	{}
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			if (mat.shader.name == "Curve")
			{
				mat.SetFloat("_MinY", mMinY);
				mat.SetFloat("_MaxY", mMaxY);
				mat.SetFloat("_Alpha", mAlpha);
			}
		}
	}
}