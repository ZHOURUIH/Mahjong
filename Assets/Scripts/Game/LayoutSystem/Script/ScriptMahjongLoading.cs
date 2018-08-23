
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongLoading : LayoutScript
{
	protected txNGUISprite mBackground;
	protected txNGUISlider mProgress;
	public ScriptMahjongLoading(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
		newObject(out mProgress, mBackground, "Progress");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		setProgress(0.0f);
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void setProgress(float progress)
	{
		mProgress.setSliderValue(progress);
	}
}