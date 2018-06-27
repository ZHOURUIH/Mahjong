using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUIStaticTextureCriticalMask : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderCriticalMask>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureCriticalMaskFadeOutLinearDodge : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderCriticalMaskFadeOutLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureFeather : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderFeather>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureGrey : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderGrey>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureLumOffset : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderLumOffset>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureLumOffsetLinearDodge : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderLumOffsetLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureLinearDodge : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureHSLOffset : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderHSLOffset>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureHSLOffsetLinearDodge : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderHSLOffsetLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUIStaticTextureMaskCut : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderMaskCut>();
	}
}