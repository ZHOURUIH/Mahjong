using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class CommandWindowSmoothSlider :Command
{
	public float mStartSliderValue = 1.0f;
	public float mTargetSliderValue = 1.0f;
	public float mFadeTime = 0.0f;
	public float mTimeOffset = 0.0f;
	public override void init()
	{
		base.init();
		mStartSliderValue = 1.0f;
		mTargetSliderValue = 1.0f;
		mFadeTime = 0.0f;
		mTimeOffset = 0.0f;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentSmoothSlider componentSmoothSlider = window.getFirstComponent<WindowComponentSmoothSlider>();
		if (componentSmoothSlider != null)
		{
			componentSmoothSlider.setActive(true);
			componentSmoothSlider.start(mStartSliderValue, mTargetSliderValue, mFadeTime, mTimeOffset);
		}
	}
}