using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 花牌
public class HuaMahjong
{
	protected ScriptMahjongHandIn mScript;
	protected string mMahjongPreName;
	protected txUIObject mHuaRoot;
	protected List<txNGUISprite> mHuaMahjong;
	public HuaMahjong(ScriptMahjongHandIn script, string mahjongPreName)
	{
		mScript = script;
		mMahjongPreName = mahjongPreName;
		mHuaMahjong = new List<txNGUISprite>();
	}
	public void assignWindow(string huaRoot)
	{
		mScript.newObject(out mHuaRoot, huaRoot, 0);
		for (int i = 0; i < GameDefine.MAX_HUA_COUNT; ++i)
		{
			txNGUISprite obj = mScript.newObject(out obj, mHuaRoot, "Mahjong" + i);
			mHuaMahjong.Add(obj);
		}
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		getHua(new List<MAHJONG>());
	}
	public void getHua(List<MAHJONG> huaList)
	{
		LT.ACTIVE_WINDOW(mHuaRoot);
		int curCount = huaList.Count;
		int maxCount = mHuaMahjong.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			bool show = i < curCount;
			LT.ACTIVE_WINDOW(mHuaMahjong[i], show);
			if (show)
			{
				mHuaMahjong[i].setSpriteName(mMahjongPreName + GameDefine.MAHJONG_NAME[(int)huaList[i]]);
			}
		}
	}
}
