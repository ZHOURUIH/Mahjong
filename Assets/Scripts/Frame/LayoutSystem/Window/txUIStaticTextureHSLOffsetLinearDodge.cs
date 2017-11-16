using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTextureHSLOffsetLinearDodge : txUIStaticTextureHSLOffset
{
	public txUIStaticTextureHSLOffsetLinearDodge()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
	}
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "HSLOffsetLinearDodge")
			{
				mat.SetColor("_HSLOffset", new Color(mHSLOffset.x, mHSLOffset.y, mHSLOffset.z));
			}
		}
	}
}