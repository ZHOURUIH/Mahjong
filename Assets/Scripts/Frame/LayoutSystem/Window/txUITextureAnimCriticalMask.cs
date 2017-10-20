using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUITextureAnimCriticalMask : txUITextureAnim
{
	protected float mCriticalValue;
	public txUITextureAnimCriticalMask()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
	}
	public void setCriticalValue(float critical) { mCriticalValue = critical; }
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "CriticalMask")
			{
				mat.SetFloat("_CriticalValue", mCriticalValue);
			}
		}
	}
}