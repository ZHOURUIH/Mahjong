using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharacterInfoPanel
{
	public txUIObject mRoot;
	public txNGUISprite mHead;
	public txNGUIText mName;
	public txNGUISprite mMoneyIcon;
	public txNGUINumber mMoneyValue;
	public txNGUISprite mReady;
	public txNGUISprite mBanker;
	public ScriptAllCharacterInfo mScript;
	public CharacterInfoPanel(ScriptAllCharacterInfo script)
	{
		mScript = script;
	}
	public void assignWindow(string rootName)
	{
		mScript.newObject(out mRoot, rootName);
		mScript.newObject(out mHead, mRoot, "Head");
		mScript.newObject(out mName, mRoot, "Name");
		mScript.newObject(out mMoneyIcon, mRoot, "MoneyIcon");
		mScript.newObject(out mMoneyValue, mMoneyIcon, "MoneyValue");
		mScript.newObject(out mReady, mRoot, "Ready", 0);
		mScript.newObject(out mBanker, mRoot, "Banker", 0);
	}
	public void init()
	{
		mMoneyValue.setDockingPosition(DOCKING_POSITION.DP_LEFT);
	}
	public void onReset()
	{
		notifyLeave();
		LT.ACTIVE_WINDOW(mReady, false);
	}
	public void notifyJoin(Character player)
	{
		LT.ACTIVE_WINDOW(mRoot);
		CharacterData data = player.getCharacterData();
		setHead(data.mHead);
		setName(data.mName);
		setMoney(data.mMoney);
		notifyReady(data.mReady);
		setBanker(data.mBanker);
	}
	public void notifyLeave()
	{
		LT.ACTIVE_WINDOW(mRoot, false);
	}
	public void notifyReady(bool ready)
	{
		LT.ACTIVE_WINDOW(mReady, ready);
	}
	public void notifyStartGame()
	{
		LT.ACTIVE_WINDOW(mReady, false);
	}
	public void setHead(int index)
	{
		mHead.setSpriteName("Head" + index);
	}
	public void setName(string name)
	{
		mName.setLabel(name);
	}
	public void setMoney(int money)
	{
		mMoneyValue.setNumber(money);
	}
	public void setBanker(bool banker)
	{
		LT.ACTIVE_WINDOW(mBanker, banker);
	}
}

public class ScriptAllCharacterInfo : LayoutScript
{
	protected CharacterInfoPanel[] mInfoPanelList;
	public ScriptAllCharacterInfo(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mInfoPanelList = new CharacterInfoPanel[GameDefine.MAX_PLAYER_COUNT];
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i] = new CharacterInfoPanel(this);
		}
	}
	public override void assignWindow()
	{
		string[] rootNameList = new string[] { "MyInfoRoot", "LeftInfoRoot", "OppositeInfoRoot", "RightInfoRoot" };
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i].assignWindow(rootNameList[i]);
		}
	}
	public override void init()
	{
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i].init();
		}
	}
	public override void onReset()
	{
		foreach(var info in mInfoPanelList)
		{
			info.onReset();
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
	public void notifyCharacterJoin(Character player)
	{
		CharacterData data = player.getCharacterData();
		mInfoPanelList[(int)data.mPosition].notifyJoin(player);
	}
	public void notifyCharacterLeave(Character player)
	{
		CharacterData data = player.getCharacterData();
		mInfoPanelList[(int)data.mPosition].notifyLeave();
	}
	public void notifyCharacterReady(Character player, bool ready)
	{
		CharacterData data = player.getCharacterData();
		mInfoPanelList[(int)data.mPosition].notifyReady(ready);
	}
	public void notifyCharacterBanker(Character player, bool banker)
	{
		CharacterData data = player.getCharacterData();
		mInfoPanelList[(int)data.mPosition].setBanker(banker);
	}
	public void notifyStartGame()
	{
		int infoCount = mInfoPanelList.Length;
		for(int i = 0; i < infoCount; ++i)
		{
			mInfoPanelList[i].notifyStartGame();
		}
	}
}