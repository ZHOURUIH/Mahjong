using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderHSLOffset : WindowShader
{
	protected Vector3 mHSLOffset;   // 当前HSL偏移,只有当shader为HSLOffet或者HSLOffsetLinearDodge时才有效
	protected bool mGray = false;
	public WindowShaderHSLOffset()
	{}
	public void setHSLOffset(Vector3 offset) { mHSLOffset = offset; }
	public Vector3 getHSLOffset() { return mHSLOffset; }
	public void setGray(bool gray) { mGray = gray; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "HSLOffset")
			{
				mat.SetColor("_HSLOffset", new Color(mHSLOffset.x, mHSLOffset.y, mHSLOffset.z));
				mat.SetInt("_GrayHSL", mGray ? 1 : 0);
			}
		}
	}
}