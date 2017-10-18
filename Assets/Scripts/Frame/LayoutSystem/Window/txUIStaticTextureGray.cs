using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTextureGray : txUIStaticTexture
{
	public txUIStaticTextureGray()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
	}
	public void setGray(bool gray)
	{
		if (mTexture == null)
		{
			return;
		}
		// 设置为灰色shader时,需要记录当前shader名,以便取消灰色时恢复之前的shader
		if (gray)
		{
			mTexture.shader = mShaderManager.getShader("Gray");
		}
		else
		{
			mTexture.shader = mShaderManager.getShader(mNormalShaderName);
		}
	}
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
	}
}