using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class WindowComponentFill : ComponentKeyFrameNormal
{
	public float mStartValue;   // 移动开始时的位置
	public float mTargetValue;
	public WindowComponentFill(Type type, string name)
		:
		base(type, name)
	{ }
	public void setTargetValue(float value) { mTargetValue = value; }
	public void setStartValue(float value) { mStartValue = value; }
	//------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentFill); }
	protected override void applyTrembling(float value)
	{
		txUIObject window = mComponentOwner as txUIObject;
		float curValue = mStartValue + (mTargetValue - mStartValue) * value;
		window.setFillPercent(curValue);
	}
}