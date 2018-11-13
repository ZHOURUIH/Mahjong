
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderPixelMaskCut : WindowShader
{
	protected Texture mMask;
	protected Vector2 mMaskSize = Vector2.zero;
	protected Vector2 mMaskPos = Vector2.zero;
	public WindowShaderPixelMaskCut()
	{}
	public void setMaskTexture(Texture mask) { mMask = mask; }
	public void setMaskSize(Vector2 size) { mMaskSize = size; }
	public void setMaskPos(Vector2 pos) { mMaskPos = pos; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "PixelMaskCut")
			{
				mat.SetTexture("_MaskTex", mMask);
				if(isVectorZero(mMaskSize))
				{
					mMaskSize = new Vector2(mMask.width, mMask.height);
				}
				mat.SetFloat("_SizeX", mMaskSize.x);
				mat.SetFloat("_SizeY", mMaskSize.y);
				mat.SetFloat("_PosX", mMaskPos.x);
				mat.SetFloat("_PosY", mMaskPos.y);
			}
		}
	}
}