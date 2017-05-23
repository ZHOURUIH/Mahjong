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
		notifyQuit();
		LayoutTools.ACTIVE_WINDOW(mReady, false);
	}
	public void notifyJoin(Character player)
	{
		CharacterData data = player.getCharacterData();
		LayoutTools.ACTIVE_WINDOW(mRoot);
		mHead.setSpriteName("Head" + data.mHead);
		mName.setText(data.mName);
		mMoneyValue.setNumber(data.mMoney);
		LayoutTools.ACTIVE_WINDOW(mBanker, data.mBanker);
	}
	public void notifyQuit()
	{
		LayoutTools.ACTIVE_WINDOW(mRoot, false);
	}
	public void notifyReady()
	{
		LayoutTools.ACTIVE_WINDOW(mReady, true);
	}
	public void notifyStartGame()
	{
		LayoutTools.ACTIVE_WINDOW(mReady, false);
	}
}

public class ScriptAllCharacterInfo : LayoutScript
{
	protected CharacterInfoPanel[] mInfoPanelList;
	public ScriptAllCharacterInfo(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mInfoPanelList = new CharacterInfoPanel[CommonDefine.MAX_PLAYER_COUNT];
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i] = new CharacterInfoPanel();
		}
	}
	public override void assignWindow()
	{
		string[] rootNameList = new string[] { "MyInfoRoot", "LeftInfoRoot", "OppositeInfoRoot", "RightInfoRoot" };
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mInfoPanelList[i].assignWindow(this, rootNameList[i]);
		}
	}
	public override void init()
	{
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
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
	public void notifyCharacterQuit(Character player)
	{
		CharacterData data = player.getCharacterData();
		mInfoPanelList[(int)data.mPosition].notifyQuit();
	}
	public void notifyCharacterReady(Character player)
	{
		CharacterData data = player.getCharacterData();
		mInfoPanelList[(int)data.mPosition].notifyReady();
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