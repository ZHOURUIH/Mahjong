using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptGameEnding : LayoutScript
{
	protected txUIStaticSprite mHu;
	protected txUIStaticSprite mPingJu;
	public ScriptGameEnding(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mHu = newObject<txUIStaticSprite>("Hu", 1);
		mPingJu = newObject<txUIStaticSprite>("PingJu", 1);
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		;
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
	public void setResult(List<HU_TYPE> huList)
	{
		// 先隐藏全部
		LayoutTools.ACTIVE_WINDOW(mHu, false);
		LayoutTools.ACTIVE_WINDOW(mPingJu, false);
		if (huList == null)
		{
			LayoutTools.ACTIVE_WINDOW(mPingJu);
		}
		else
		{
			LayoutTools.ACTIVE_WINDOW(mHu);
		}
	}
}