using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 碰或者杠的牌
public class PengGangMahjong
{
	protected ScriptMahjongHandIn mScript;
	protected string mMahjongPreName;
	protected txUIObject mPengGangRoot;
	protected List<txUIObject> mPengGangSingleRoot;
	protected List<List<txNGUIStaticSprite>> mMahjongWindows;
	public PengGangMahjong(ScriptMahjongHandIn script, string preName)
	{
		mScript = script;
		mMahjongPreName = preName;
		mPengGangSingleRoot = new List<txUIObject>();
		mMahjongWindows = new List<List<txNGUIStaticSprite>>();
		for (int i = 0; i < GameDefine.MAX_PENG_TIMES; ++i)
		{
			mMahjongWindows.Add(new List<txNGUIStaticSprite>());
		}
	}
	public void assignWindow(string rootName)
	{
		mScript.newObject(out mPengGangRoot, rootName);
		for (int i = 0; i < GameDefine.MAX_PENG_TIMES; ++i)
		{
			txUIObject obj = mScript.newObject(out obj, mPengGangRoot, "PengGang" + i);
			mPengGangSingleRoot.Add(obj);
		}
		int pengTimes = mMahjongWindows.Count;
		for (int i = 0; i < pengTimes; ++i)
		{
			for (int j = 0; j < GameDefine.MAX_SINGLE_COUNT; ++j)
			{
				txNGUIStaticSprite obj = mScript.newObject(out obj, mPengGangSingleRoot[i], "Mahjong" + j, 0);
				mMahjongWindows[i].Add(obj);
			}
		}
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		int count = mPengGangSingleRoot.Count;
		for (int i = 0; i < count; ++i)
		{
			resetPengGang(i);
		}
	}
	public void showMahjong(List<PengGangInfo> infoList)
	{
		int maxCount = mPengGangSingleRoot.Count;
		for (int i = 0; i < maxCount; ++i)
		{
			resetPengGang(i);
			if (i < infoList.Count)
			{
				showPengGang(i, infoList[i].mType, infoList[i].mMahjong);
			}
		}
	}
	//----------------------------------------------------------------------------------------------------------
	protected void resetPengGang(int index)
	{
		LayoutTools.ACTIVE_WINDOW(mPengGangSingleRoot[index], false);
		int maxCount = mMahjongWindows[index].Count;
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mMahjongWindows[index][i], false);
		}
	}
	protected void showPengGang(int index, ACTION_TYPE type, MAHJONG mah)
	{
		int count = 0;
		if (type == ACTION_TYPE.AT_PENG)
		{
			count = 3;
		}
		else if (type == ACTION_TYPE.AT_GANG)
		{
			count = 4;
		}
		else
		{
			return;
		}
		LayoutTools.ACTIVE_WINDOW(mPengGangSingleRoot[index]);
		int maxCount = mMahjongWindows[index].Count;
		string mahjongSpriteName = mMahjongPreName + GameDefine.MAHJONG_NAME[(int)mah];
		for (int i = 0; i < maxCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mMahjongWindows[index][i], i < count);
			if (i < count)
			{
				mMahjongWindows[index][i].setSpriteName(mahjongSpriteName);
			}
		}
	}
}