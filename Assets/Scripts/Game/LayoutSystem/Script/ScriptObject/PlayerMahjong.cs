using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


// 玩家手里所有的麻将,手牌,碰牌,花牌,胡牌后展示的牌
public class PlayerMahjong : GameBase
{
	protected ShowHuMahjong mShowMahjong;
	protected PengGangMahjong mPengGangMahjong;
	protected HandInMahjong mHandInMahjong;
	protected HuaMahjong mHuaMahjong;
	protected PLAYER_POSITION mPosition;
	protected ScriptMahjongHandIn mScript;
	public PlayerMahjong(ScriptMahjongHandIn script, PLAYER_POSITION position)
	{
		mScript = script;
		mPosition = position;
		if (mPosition == PLAYER_POSITION.PP_MYSELF)
		{
			mHandInMahjong = new HandInMahjongMyself(mScript, mPosition);
		}
		else
		{
			mHandInMahjong = new HandInMahjongOther(mScript, mPosition);
		}
		mShowMahjong = new ShowHuMahjong(mScript, GameDefine.mDropMahjongPreName[(int)mPosition]);
		mPengGangMahjong = new PengGangMahjong(mScript, GameDefine.mDropMahjongPreName[(int)mPosition]);
		mHuaMahjong = new HuaMahjong(mScript, GameDefine.mDropMahjongPreName[(int)mPosition]);
	}
	public void assignWindow(string handInRoot, string pengGangRoot, string showRoot, string huaRoot)
	{
		mHandInMahjong.assignWindow(handInRoot);
		mPengGangMahjong.assignWindow(pengGangRoot);
		mShowMahjong.assignWindow(showRoot);
		mHuaMahjong.assignWindow(huaRoot);
	}
	public void init()
	{
		mHandInMahjong.init();
		mPengGangMahjong.init();
		mShowMahjong.init();
		mHuaMahjong.init();
	}
	public void onReset()
	{
		mHandInMahjong.onReset();
		mPengGangMahjong.onReset();
		mShowMahjong.onReset();
		mHuaMahjong.onReset();
	}
	public void notifyGetStart(MAHJONG mah)
	{
		mHandInMahjong.notifyGetStart(mah);
	}
	public void notifyGet(MAHJONG mah)
	{
		mHandInMahjong.notifyGet(mah);
	}
	//public void notifyShowHua(MAHJONG mah, int index)
	//{
	//	mHandInMahjong.notifyShowHua(mah, index);
	//}
	public void notifyDrop(MAHJONG mah, int index)
	{
		mHandInMahjong.notifyDrop(mah, index);
	}
	public void notifyReorder(List<MAHJONG> handIn)
	{
		mHandInMahjong.notifyReorder(handIn);
	}
	public void notifyCanDrop(bool canDrop)
	{
		mHandInMahjong.notifyCanDrop(canDrop);
	}
	// 碰牌或者杠牌
	public void notifyPengOrGang(List<PengGangInfo> infoList)
	{
		mPengGangMahjong.showMahjong(infoList);
	}
	// 通知游戏结束,显示自己的牌
	public void notifyEnd(List<MAHJONG> handIn)
	{
		mHandInMahjong.notifyEnd();
		mShowMahjong.showCurMahjong(handIn);
	}
	public void notifyGetHua(List<MAHJONG> huaList)
	{
		mHuaMahjong.getHua(huaList);
	}
	public void notifyAbleToPengOrGang(MAHJONG mah)
	{
		mHandInMahjong.markAbleToPengOrGang(mah);
	}
}