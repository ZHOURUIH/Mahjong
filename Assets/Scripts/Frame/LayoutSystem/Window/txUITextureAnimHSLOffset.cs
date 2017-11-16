using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUITextureAnimHSLOffset : txUITextureAnim
{
	protected Vector3   mHSLOffset;	// 当前HSL偏移,只有当shader为HSLOffet或者HSLOffsetLinearDodge时才有效
	public txUITextureAnimHSLOffset()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
	}
	public void setHSLOffset(Vector3 offset){mHSLOffset = offset;}
	public Vector3 getHSLOff() {return mHSLOffset;}
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "HSLOffset")
			{
				mat.SetColor("_HSLOffset", new Color(mHSLOffset.x, mHSLOffset.y, mHSLOffset.z));
			}
		}
	}
}