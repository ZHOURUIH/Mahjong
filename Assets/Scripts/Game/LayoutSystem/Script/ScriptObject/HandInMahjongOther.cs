using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HandInMahjongOther : HandInMahjong
{
	public HandInMahjongOther(ScriptMahjongHandIn script, PLAYER_POSITION position)
		: base(script, position)
	{ }
	public override void assignWindow(string handInRoot)
	{
		base.assignWindow(handInRoot);
	}
	public override void init()
	{
		base.init();
		int count = mHandInMahjong.Count;
		for (int i = 0; i < count; ++i)
		{
			mHandInMahjong[i].mMahjongWindow.setHandleInput(false);
		}
	}
	public override void onReset()
	{
		base.onReset();
		refreshMahjongCount(0);
	}
	public override void notifyGetStart(MAHJONG mah)
	{
		base.notifyGetStart(mah);
	}
	public override void notifyGet(MAHJONG mah)
	{
		base.notifyGet(mah);
	}
	public override void notifyDrop(MAHJONG mah, int index)
	{
		base.notifyDrop(mah, index);
	}
	public override void notifyReorder(List<MAHJONG> handIn)
	{
		base.notifyReorder(handIn);
		refreshMahjongCount(handIn.Count);
	}
}