using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class WindowComponentSmoothSlider : ComponentLinear
{
	public WindowComponentSmoothSlider(Type type, string name)
		:
		base(type, name)
	{ }
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentSmoothSlider); }
	protected override void applyValue(float value, bool done = false)
	{
		txUISlider window = mComponentOwner as txUISlider;
		window.setSliderValue(value);
	}
}