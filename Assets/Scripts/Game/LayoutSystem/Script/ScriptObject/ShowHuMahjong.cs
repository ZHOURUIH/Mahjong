using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 胡牌后展示的牌
public class ShowHuMahjong
{
	protected ScriptMahjongHandIn mScript;
	protected string mMahjongPreName;
	protected txUIObject mShowRoot;
	protected List<txNGUISprite> mShowMahjong;
	public ShowHuMahjong(ScriptMahjongHandIn script, string mahjongPreName)
	{
		mScript = script;
		mMahjongPreName = mahjongPreName;
		mShowMahjong = new List<txNGUISprite>();
	}
	public void assignWindow(string showRoot)
	{
		mScript.newObject(out mShowRoot, showRoot, 0);
		for (int i = 0; i < GameDefine.MAX_HAND_IN_COUNT; ++i)
		{
			txNGUISprite obj = mScript.newObject(out obj, mShowRoot, "Mahjong" + i);
			mShowMahjong.Add(obj);
		}
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		LT.ACTIVE_WINDOW(mShowRoot, false);
	}
	public void showCurMahjong(List<MAHJONG> mahList)
	{
		LT.ACTIVE_WINDOW(mShowRoot);
		int curCount = mahList.Count;
		int maxCount = mShowMahjong.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			bool show = i < curCount;
			LT.ACTIVE_WINDOW(mShowMahjong[i], show);
			if (show)
			{
				mShowMahjong[i].setSpriteName(mMahjongPreName + GameDefine.MAHJONG_NAME[(int)mahList[i]]);
			}
		}
	}
}
