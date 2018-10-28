using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongDrop : LayoutScript
{
	protected txUIObject[] mRootList;
	protected List<txNGUISprite>[] mDropList;
	protected int mMaxDropCount = 30;
	public ScriptMahjongDrop(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mRootList = new txUIObject[GameDefine.MAX_PLAYER_COUNT];
		mDropList = new List<txNGUISprite>[GameDefine.MAX_PLAYER_COUNT];
		for(int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mDropList[i] = new List<txNGUISprite>();
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
				txNGUISprite obj = newObject(out obj, mRootList[i], "Mahjong" + j, 0);
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
				LT.ACTIVE_WINDOW(mDropList[i][j], false);
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
			LT.ACTIVE_WINDOW(mDropList[(int)pos][preCount]);
		}
		else
		{
			refreshDropMahjong(pos, droppedMahjong);
		}
	}
	public void notifyTakeDroppedMahjong(PLAYER_POSITION pos, int index)
	{
		int maxIndex = getMin(mMaxDropCount - 1, index);
		LT.ACTIVE_WINDOW(mDropList[(int)pos][maxIndex], false);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected void refreshDropMahjong(PLAYER_POSITION pos, List<MAHJONG> droppedMahjong)
	{
		int droppedCount = droppedMahjong.Count;
		int showCount = getMin(mMaxDropCount, droppedCount);
		int showStartIndex = droppedCount - showCount;
		for (int i = 0; i < mMaxDropCount; ++i)
		{
			LT.ACTIVE_WINDOW(mDropList[(int)pos][i], i < showCount);
			if(i < showCount)
			{
				mDropList[(int)pos][i].setSpriteName(GameDefine.mDropMahjongPreName[(int)pos] + GameDefine.MAHJONG_NAME[(int)droppedMahjong[showStartIndex + i]]);
			}
		}
	}
}