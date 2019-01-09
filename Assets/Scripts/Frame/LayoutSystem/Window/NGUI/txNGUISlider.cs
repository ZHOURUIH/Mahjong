using UnityEngine;
using System.Collections;

public class txNGUISlider : txUIObject
{	
	protected UISlider mSlider;
	public txNGUISlider()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mSlider = mObject.GetComponent<UISlider>();
		if(mSlider == null)
		{
			mSlider = mObject.AddComponent<UISlider>();
		}
	}
	public float getSliderValue ()
	{
		if (mSlider == null)
		{
			return 0.0f;
		}
		return mSlider.value;
	}
	public void setSliderValue(float value)
	{
		if (mSlider == null)
		{
			return;
		}
		clamp(ref value, 0.0f, 1.0f);
		mSlider.value = value;
	}
	public void setSliderValueChange(EventDelegate.Callback mUislider) 
	{
		if (mSlider == null)
		{
			return;
		}
		EventDelegate.Add(mSlider.onChange, mUislider);
	}
	public override void setAlpha(float alpha)
	{
		if (mSlider == null)
		{
			return;
		}
		mSlider.alpha = alpha;
	}
	public override float getAlpha()
	{
		if (mSlider == null)
		{
			return 0.0f;
		}
		return mSlider.alpha;
	}
}