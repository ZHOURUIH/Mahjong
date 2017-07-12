using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EndingCharacter
{
	public txUIObject mRoot;
	public txUIStaticTexture mHead;
	public txUIText mName;
	public txUIText mMoneyDelta;
	public ScriptGameEnding mScript;
	public EndingCharacter(ScriptGameEnding script)
	{
		mScript = script;
	}
	public void assignWindow(txUIObject parent, string rootName)
	{
		mRoot = mScript.newObject<txUIObject>(parent, rootName);
		mHead = mScript.newObject<txUIStaticTexture>(mRoot, "Head");
		mName = mScript.newObject<txUIText>(mRoot, "Name");
		mMoneyDelta = mScript.newObject<txUIText>(mRoot, "MoneyDelta");
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		;
	}
	public void setHead(Texture tex)
	{
		mHead.setTexture(tex);
	}
	public void setHead(int head)
	{
		mHead.setTextureName(CommonDefine.R_GAME_TEXTURE_PATH + "Head/Head" + head);
	}
	public void setName(string name)
	{
		mName.setText(name);
	}
	public void setMoneyDelta(int moneyDelta)
	{
		string deltaStr = StringUtility.intToString(Mathf.Abs(moneyDelta));
		deltaStr = moneyDelta > 0 ? ("+" + deltaStr) : ("-" + deltaStr);
		mMoneyDelta.setText(deltaStr);
	}
}
public class Multiple
{
	public txUIObject mRoot;
	public txUIText mDescribe;
	public txUIText mMultipleCount;
	public ScriptGameEnding mScript;
	public Multiple(ScriptGameEnding script)
	{
		mScript = script;
	}
	public void assignWindow(txUIObject parent, string rootName)
	{
		mRoot = mScript.newObject<txUIObject>(parent, rootName);
		mDescribe = mScript.newObject<txUIText>(mRoot, "Describe");
		mMultipleCount = mScript.newObject<txUIText>(mRoot, "MultipleCount");
	}
	public void init()
	{
		;
	}
	public void onReset()
	{
		;
	}
	public void setVisible(bool show)
	{
		LayoutTools.ACTIVE_WINDOW(mRoot, show);
	}
	public void setDescribe(string describe)
	{
		mDescribe.setText(describe);
	}
	public void setMultiple(int multiple)
	{
		mMultipleCount.setText(StringUtility.intToString(multiple) + " 番");
	}
}

public class ScriptGameEnding : LayoutScript
{
	protected txUIObject mHuResultRoot;
	protected txUIStaticSprite mHu;
	protected txUIStaticSprite mPingJu;
	protected txUIObject mMoneyResultRoot;
	protected List<EndingCharacter> mEndingCharacterList;
	protected txUIObject mDetailRoot;
	protected txUIText mMultipleTitle;
	protected List<Multiple> mMultipleList;
	protected txUIObject mButtonRoot;
	protected txUIButton mContinueButton;
	protected txUIButton mMatchButton;
	protected txUIButton mReturnButton;
	public ScriptGameEnding(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mEndingCharacterList = new List<EndingCharacter>();
		mMultipleList = new List<Multiple>();
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			mEndingCharacterList.Add(new EndingCharacter(this));
		}
		for(int i = 0; i < CommonDefine.MAX_HU_COUNT; ++i)
		{
			mMultipleList.Add(new Multiple(this));
		}
	}
	public override void assignWindow()
	{
		mHuResultRoot = newObject<txUIObject>("HuResultRoot");
		mHu = newObject<txUIStaticSprite>(mHuResultRoot, "Hu", 1);
		mPingJu = newObject<txUIStaticSprite>(mHuResultRoot, "PingJu", 1);
		mMoneyResultRoot = newObject<txUIObject>("MoneyResultRoot");
		int charCount = mEndingCharacterList.Count;
		for(int i = 0; i < charCount; ++i)
		{
			mEndingCharacterList[i].assignWindow(mMoneyResultRoot, "Character" + i);
		}
		mDetailRoot = newObject<txUIObject>("DetailRoot");
		mMultipleTitle = newObject<txUIText>(mDetailRoot, "MultipleTitle");
		int multipleCount = mMultipleList.Count;
		for(int i = 0; i < multipleCount; ++i)
		{
			mMultipleList[i].assignWindow(mDetailRoot, "Multiple" + i);
		}
		mButtonRoot = newObject<txUIObject>("ButtonRoot");
		mContinueButton = newObject<txUIButton>(mButtonRoot, "Continue");
		mMatchButton = newObject<txUIButton>(mButtonRoot, "Match");
		mReturnButton = newObject<txUIButton>(mButtonRoot, "Return");
	}
	public override void init()
	{
		int charCount = mEndingCharacterList.Count;
		for (int i = 0; i < charCount; ++i)
		{
			mEndingCharacterList[i].init();
		}
		int multipleCount = mMultipleList.Count;
		for (int i = 0; i < multipleCount; ++i)
		{
			mMultipleList[i].init();
		}
		mContinueButton.setClickCallback(onContinueClick);
		mContinueButton.setPressCallback(onButtonPress);
		mMatchButton.setClickCallback(onMatchClick);
		mMatchButton.setPressCallback(onButtonPress);
		mReturnButton.setClickCallback(onReturnClick);
		mReturnButton.setPressCallback(onButtonPress);
	}
	public override void onReset()
	{
		int charCount = mEndingCharacterList.Count;
		for (int i = 0; i < charCount; ++i)
		{
			mEndingCharacterList[i].onReset();
		}
		int multipleCount = mMultipleList.Count;
		for (int i = 0; i < multipleCount; ++i)
		{
			mMultipleList[i].onReset();
		}
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.ACTIVE_WINDOW(mMoneyResultRoot, false);
		LayoutTools.ACTIVE_WINDOW(mDetailRoot, false);
		LayoutTools.ACTIVE_WINDOW(mButtonRoot, false);
		LayoutTools.ACTIVE_WINDOW_DELAY(this, mMoneyResultRoot, true, 2.0f);
		LayoutTools.ACTIVE_WINDOW_DELAY(this, mDetailRoot, true, 2.0f);
		LayoutTools.ACTIVE_WINDOW_DELAY(this, mButtonRoot, true, 2.0f);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void setResult(bool isHu)
	{
		
	}
	public void setDetail(List<ResultInfo> resultList)
	{
		LayoutTools.ACTIVE_WINDOW(mHu, resultList.Count > 0);
		LayoutTools.ACTIVE_WINDOW(mPingJu, resultList.Count == 0);
		// 显示番数
		if (resultList.Count == 0)
		{
			return;
		}
		// 暂时只显示第一个胡牌的玩家的番数
		ResultInfo info = resultList[0];
		int multipleCount = mMultipleList.Count;
		for (int i = 0; i < multipleCount; ++i)
		{
			bool visible = i < info.mHuList.Count;
			mMultipleList[i].setVisible(visible);
			if (visible)
			{
				mMultipleList[i].setDescribe(CommonDefine.HU_NAME[(int)info.mHuList[i]]);
				mMultipleList[i].setMultiple(CommonDefine.HU_MULTIPLE[(int)info.mHuList[i]]);
			}
		}
	}
	public void setPlayerInfo(Dictionary<Character, int> moneyDelta)
	{
		int count = MathUtility.getMin(mEndingCharacterList.Count, moneyDelta.Count);
		List<Character> keys = new List<Character>(moneyDelta.Keys);
		for(int i = 0; i < count; ++i)
		{
			CharacterData data = keys[i].getCharacterData();
			EndingCharacter endChar = mEndingCharacterList[i];
			// 如果是微信头像,则使用PlayerHeadManager
			endChar.setHead(data.mHead);
			endChar.setName(data.mName);
			endChar.setMoneyDelta(moneyDelta[keys[i]]);
		}
	}
	//---------------------------------------------------------------------------------
	protected void onContinueClick(GameObject go)
	{
		;
	}
	protected void onMatchClick(GameObject go)
	{
		;
	}
	protected void onReturnClick(GameObject go)
	{
		;
	}
	protected void onButtonPress(GameObject go, bool press)
	{
		txUIObject button = mLayout.getUIObject(go);
		LayoutTools.SCALE_WINDOW(button, button.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}