using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharacterInfoPanel
{
	public txUIObject mRoot;
	public txUIStaticSprite mHead;
	public txUIText mName;
	public txUIStaticSprite mMoneyIcon;
	public txUINumber mMoneyValue;
	public txUIStaticSprite mReady;
	public txUIStaticSprite mBanker;
	public ScriptAllCharacterInfo mScript;
	public CharacterInfoPanel()
	{
		;
	}
	public void assignWindow(ScriptAllCharacterInfo script, string rootName)
	{
		mScript = script;
		mRoot = mScript.newObject<txUIObject>(rootName);
		mHead = mScript.newObject<txUIStaticSprite>(mRoot, "Head");
		mName = mScript.newObject<txUIText>(mRoot, "Name");
		mMoneyIcon = mScript.newObject<txUIStaticSprite>(mRoot, "MoneyIcon");
		mMoneyValue = mScript.newObject<txUINumber>(mMoneyIcon, "MoneyValue");
		mReady = mScript.newObject<txUIStaticSprite>(mRoot, "Ready", 0);
		mBanker = mScript.newObject<txUIStaticSprite>(mRoot, "Banker", 0);
	}
	public void init()
	{
		mMoneyValue.setDockingPosition(DOCKING_POSITION.DP_LEFT);
	}
	public void onReset()
	{
		notifyLeave();
		LayoutTools.ACTIVE_WINDOW(mReady, false);
	}
	public void notifyJoin(Character player)
	{
		LayoutTools.ACTIVE_WINDOW(mRoot);
		CharacterData data = player.getCharacterData();
		setHead(data.mHead);
		setName(data.mName);
		setMoney(data.mMoney);
		notifyReady(data.mReady);
		setBanker(data.mBanker);
	}
	public void notifyLeave()
	{
		LayoutTools.ACTIVE_WINDOW(mRoot, false);
	}
	public void notifyReady(bool ready)
	{
		LayoutTools.ACTIVE_WINDOW(mReady, ready);
	}
	public void notifyStartGame()
	{
		LayoutTools.ACTIVE_WINDOW(mReady, false);
	}
	public void setHead(int index)
	{
		mHead.setSpriteName("Head" + index);
	}
	public void setName(string name)
	{
		mName.setText(name);
	}
	public void setMoney(int money)
	{
		mMoneyValue.setNumber(money);
	}
	public void setBanker(bool banker)
	{
		LayoutTools.ACTIVE_WINDOW(mBanker, banker);
	}
}

public class ScriptAllCharacterInfo : LayoutScript
{
	protected CharacterInfoPanel[] mInfoPanelList;
	public ScriptAllCharacterInfo(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mInfoPanelList = new CharacterInfoPanel[GameDefine.MAX_PLAYER_COUNT];
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i] = new CharacterInfoPanel();
		}
	}
	public override void assignWindow()
	{
		string[] rootNameList = new string[] { "MyInfoRoot", "LeftInfoRoot", "OppositeInfoRoot", "RightInfoRoot" };
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i].assignWindow(this, rootNameList[i]);
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