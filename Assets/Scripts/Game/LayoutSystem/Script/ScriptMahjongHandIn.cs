using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongHandIn : LayoutScript
{
	protected List<PlayerMahjong> mPlayerMahjong;
	public ScriptMahjongHandIn(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mPlayerMahjong = new List<PlayerMahjong>();
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mPlayerMahjong.Add(new PlayerMahjong(this, (PLAYER_POSITION)i));
		}
	}
	public override void assignWindow()
	{
		string[] handInRootName = new string[GameDefine.MAX_PLAYER_COUNT] { "MyHandInRoot", "LeftHandInRoot", "OppositeHandInRoot", "RightHandInRoot" };
		string[] pengGangRootName = new string[GameDefine.MAX_PLAYER_COUNT] { "MyPengGangRoot", "LeftPengGangRoot", "OppositePengGangRoot", "RightPengGangRoot" };
		string[] showRootName = new string[GameDefine.MAX_PLAYER_COUNT] { "MyShowRoot", "LeftShowRoot", "OppositeShowRoot", "RightShowRoot" };
		string[] huaRootName = new string[GameDefine.MAX_PLAYER_COUNT] { "MyHuaRoot", "LeftHuaRoot", "OppositeHuaRoot", "RightHuaRoot" };
		int length = mPlayerMahjong.Count;
		for (int i = 0; i < length; ++i)
		{
			mPlayerMahjong[i].assignWindow(handInRootName[i], pengGangRootName[i], showRootName[i], huaRootName[i]);
		}
	}
	public override void init()
	{
		int length = mPlayerMahjong.Count;
		for (int i = 0; i < length; ++i)
		{
			mPlayerMahjong[i].init();
		}
	}
	public override void onReset()
	{
		// 隐藏所有的碰牌
		int length = mPlayerMahjong.Count;
		for (int i = 0; i < length; ++i)
		{
			mPlayerMahjong[i].onReset();
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
	// 开局时拿牌
	public void notifyGetMahjongStart(PLAYER_POSITION pos, MAHJONG mah)
	{
		mPlayerMahjong[(int)pos].notifyGetStart(mah);
	}
	// 摸牌
	public void notifyGetMahjong(PLAYER_POSITION pos, MAHJONG mah)
	{
		mPlayerMahjong[(int)pos].notifyGet(mah);
	}
	// 打牌
	public void notifyDropMahjong(PLAYER_POSITION pos, MAHJONG mah, int index)
	{
		mPlayerMahjong[(int)pos].notifyDrop(mah, index);
	}
	// 碰牌或者杠牌
	public void notifyPengOrGang(PLAYER_POSITION pos, List<PengGangInfo> infoList)
	{
		mPlayerMahjong[(int)pos].notifyPengOrGang(infoList);
	}
	// 刷新牌
	public void notifyReorder(PLAYER_POSITION pos, List<MAHJONG> handIn)
	{
		mPlayerMahjong[(int)pos].notifyReorder(handIn);
	}
	public void notifyCanDrop(bool canDrop)
	{
		mPlayerMahjong[(int)PLAYER_POSITION.PP_MYSELF].notifyCanDrop(canDrop);
	}
	public void notifyEnd(PLAYER_POSITION pos, List<MAHJONG> handIn)
	{
		mPlayerMahjong[(int)pos].notifyEnd(handIn);
	}
	public void notifyShowHua(PLAYER_POSITION pos, List<MAHJONG> huaList)
	{
		mPlayerMahjong[(int)pos].notifyShowHua(huaList);
	}
	public void notifyAbleToPengOrGang(PLAYER_POSITION pos, MAHJONG mah)
	{
		mPlayerMahjong[(int)pos].notifyAbleToPengOrGang(mah);
	}
}