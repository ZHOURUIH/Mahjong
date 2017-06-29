using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class WindowComponentSmoothFillAmount : ComponentLinear
{
	public WindowComponentSmoothFillAmount(Type type, string name)
		:
		base(type, name)
	{ }
	//------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentSmoothFillAmount); }
	protected override void applyValue(float value, bool done = false)
	{
		txUIStaticTexture window = mComponentOwner as txUIStaticTexture;
		window.setFillPercent(value);
	}
}