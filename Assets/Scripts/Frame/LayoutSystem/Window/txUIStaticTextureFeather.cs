using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTextureFeather : txUIStaticTexture
{
	public txUIStaticTextureFeather()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
	}
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
	}
}