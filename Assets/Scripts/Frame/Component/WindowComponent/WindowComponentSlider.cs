using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentSlider : ComponentKeyFrameNormal
{
	public float mStartValue;   // 移动开始时的位置
	public float mTargetValue;
	public WindowComponentSlider(Type type, string name)
		:
		base(type, name)
	{ }
	public void setTargetValue(float value) { mTargetValue = value; }
	public void setStartValue(float value) { mStartValue = value; }
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentSlider); }
	protected override void applyTrembling(float value)
	{
		txNGUISlider window = mComponentOwner as txNGUISlider;
		if(window != null)
		{
			float curValue = mStartValue + (mTargetValue - mStartValue) * value;
			window.setSliderValue(curValue);
		}
		else
		{
			UnityUtility.logError("window is not a Slider Window!");
		}
	}
}