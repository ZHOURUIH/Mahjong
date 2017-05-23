using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongDrop : LayoutScript
{
	protected txUIObject[] mRootList;
	protected List<txUIStaticSprite>[] mDropList;
	protected int mMaxDropCount = 30;
	public ScriptMahjongDrop(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mRootList = new txUIObject[CommonDefine.MAX_PLAYER_COUNT];
		mDropList = new List<txUIStaticSprite>[CommonDefine.MAX_PLAYER_COUNT];
		for(int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mDropList[i] = new List<txUIStaticSprite>();
		}
	}
	public override void assignWindow()
	{
		;
	}
	public override void init()
	{
		string[] rootName = new string[] { "MyDropRoot", "LeftDropRoot", "OppositeDropRoot", "RightDropRoot"};
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mRootList[i] = newObject<txUIObject>(rootName[i], 1);
			for(int j = 0; j < mMaxDropCount; ++j)
			{
				string name = "Mahjong" + StringUtility.intToString(j);
				mDropList[i].Add(newObject<txUIStaticSprite>(mRootList[i], name, 0));
			}
		}
	}
	public override void onReset()
	{
		for(int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			int count = mDropList[i].Count;
			for(int j = 0; j < count; ++j)
			{
				LayoutTools.ACTIVE_WINDOW(mDropList[i][j], false);
			}
		}
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
	// pos是玩家的位置
	public void notifyDropMahjong(PLAYER_POSITION pos, List<MAHJONG> droppedMahjong, MAHJONG mahjong)
	{
		int preCount = droppedMahjong.Count - 1;
		if(preCount < mDropList[(int)pos].Count)
		{
			string mahjongSpriteName = CommonDefine.mDropMahjongPreName[(int)pos] + CommonDefine.MAHJONG_NAME[(int)mahjong];
			mDropList[(int)pos][preCount].setSpriteName(mahjongSpriteName);
			LayoutTools.ACTIVE_WINDOW(mDropList[(int)pos][preCount]);
		}
		else
		{
			refreshDropMahjong(pos, droppedMahjong);
		}
	}
	public void notifyTakeDroppedMahjong(PLAYER_POSITION pos, int index)
	{
		int maxIndex = MathUtility.getMin(mMaxDropCount - 1, index);
		LayoutTools.ACTIVE_WINDOW(mDropList[(int)pos][maxIndex], false);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected void refreshDropMahjong(PLAYER_POSITION pos, List<MAHJONG> droppedMahjong)
	{
		int droppedCount = droppedMahjong.Count;
		int showCount = MathUtility.getMin(mMaxDropCount, droppedCount);
		int showStartIndex = droppedCount - showCount;
		for (int i = 0; i < mMaxDropCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mDropList[(int)pos][i], i < showCount);
			if(i < showCount)
			{
				mDropList[(int)pos][i].setSpriteName(CommonDefine.mDropMahjongPreName[(int)pos] + CommonDefine.MAHJONG_NAME[(int)droppedMahjong[showStartIndex + i]]);
			}
		}
	}
}