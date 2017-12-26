using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class CommandWindowSmoothFillAmount : Command
{
	public float mStartFillAmount = 1.0f;
	public float mTargetFillAmount = 1.0f;
	public float mFadeTime = 0.0f;
	public float mTimeOffset = 0.0f;
	public override void init()
	{
		base.init();
		mStartFillAmount = 1.0f;
		mTargetFillAmount = 1.0f;
		mFadeTime = 0.0f;
		mTimeOffset = 0.0f;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentSmoothFillAmount componentSmoothFillAmount = window.getFirstComponent<WindowComponentSmoothFillAmount>();
		if (componentSmoothFillAmount != null)
		{
			componentSmoothFillAmount.setActive(true);
			componentSmoothFillAmount.start(mStartFillAmount, mTargetFillAmount, mFadeTime, mTimeOffset);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : start fill amount : " + mStartFillAmount + ", target fill amount : " + mTargetFillAmount + ", fade time : " + mFadeTime + ", time offset : " + mTimeOffset;
	}
}