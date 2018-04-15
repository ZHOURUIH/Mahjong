using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongDrop : LayoutScript
{
	protected txUIObject[] mRootList;
	protected List<txNGUIStaticSprite>[] mDropList;
	protected int mMaxDropCount = 30;
	public ScriptMahjongDrop(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mRootList = new txUIObject[GameDefine.MAX_PLAYER_COUNT];
		mDropList = new List<txNGUIStaticSprite>[GameDefine.MAX_PLAYER_COUNT];
		for(int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mDropList[i] = new List<txNGUIStaticSprite>();
		}
	}
	public override void assignWindow()
	{
		string[] rootName = new string[] { "MyDropRoot", "LeftDropRoot", "OppositeDropRoot", "RightDropRoot"};
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			newObject(out mRootList[i], rootName[i], 1);
			for(int j = 0; j < mMaxDropCount; ++j)
			{
				txNGUIStaticSprite obj = newObject(out obj, mRootList[i], "Mahjong" + j, 0);
				mDropList[i].Add(obj);
			}
		}
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		for(int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
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
			string mahjongSpriteName = GameDefine.mDropMahjongPreName[(int)pos] + GameDefine.MAHJONG_NAME[(int)mahjong];
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
				mDropList[(int)pos][i].setSpriteName(GameDefine.mDropMahjongPreName[(int)pos] + GameDefine.MAHJONG_NAME[(int)droppedMahjong[showStartIndex + i]]);
			}
		}
	}
}