using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderMaskCut : WindowShader
{
	protected Texture mMask;
	protected Vector2 mMaskSize;
	public WindowShaderMaskCut()
	{}
	public void setMaskTexture(Texture mask) { mMask = mask; }
	public void setMaskSize(Vector2 size) { mMaskSize = size; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "MaskCut")
			{
				mat.SetTexture("_MaskTex", mMask);
				mat.SetFloat("_SizeX", mMaskSize.x);
				mat.SetFloat("_SizeY", mMaskSize.y);
			}
		}
	}
}