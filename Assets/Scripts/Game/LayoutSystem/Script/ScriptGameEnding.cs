using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EndingCharacter
{
	public txUIObject mRoot;
	public txNGUITexture mHead;
	public txNGUIText mName;
	public txNGUIText mMoneyDelta;
	public ScriptGameEnding mScript;
	public EndingCharacter(ScriptGameEnding script)
	{
		mScript = script;
	}
	public void assignWindow(txUIObject parent, string rootName)
	{
		mScript.newObject(out mRoot, parent, rootName);
		mScript.newObject(out mHead, mRoot, "Head");
		mScript.newObject(out mName, mRoot, "Name");
		mScript.newObject(out mMoneyDelta, mRoot, "MoneyDelta");
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
		mName.setLabel(name);
	}
	public void setMoneyDelta(int moneyDelta)
	{
		string deltaStr = StringUtility.intToString(Mathf.Abs(moneyDelta));
		deltaStr = moneyDelta > 0 ? ("+" + deltaStr) : ("-" + deltaStr);
		mMoneyDelta.setLabel(deltaStr);
	}
}
public class Multiple
{
	public txUIObject mRoot;
	public txNGUIText mDescribe;
	public txNGUIText mMultipleCount;
	public ScriptGameEnding mScript;
	public Multiple(ScriptGameEnding script)
	{
		mScript = script;
	}
	public void assignWindow(txUIObject parent, string rootName)
	{
		mScript.newObject(out mRoot, parent, rootName);
		mScript.newObject(out mDescribe, mRoot, "Describe");
		mScript.newObject(out mMultipleCount, mRoot, "MultipleCount");
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
		mDescribe.setLabel(describe);
	}
	public void setMultiple(int multiple)
	{
		mMultipleCount.setLabel(StringUtility.intToString(multiple) + " 番");
	}
}

public class ScriptGameEnding : LayoutScript
{
	protected txUIObject mHuResultRoot;
	protected txNGUISprite mHu;
	protected txNGUISprite mPingJu;
	protected txUIObject mMoneyResultRoot;
	protected List<EndingCharacter> mEndingCharacterList;
	protected txUIObject mDetailRoot;
	protected txNGUIText mMultipleTitle;
	protected List<Multiple> mMultipleList;
	protected txUIObject mButtonRoot;
	protected txNGUIButton mContinueButton;
	protected txNGUIButton mReturnButton;
	public ScriptGameEnding(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mEndingCharacterList = new List<EndingCharacter>();
		mMultipleList = new List<Multiple>();
		for (int i = 0; i < GameDefine.MAX_PLAYER_COUNT; ++i)
		{
			mEndingCharacterList.Add(new EndingCharacter(this));
		}
		for(int i = 0; i < GameDefine.MAX_HU_COUNT; ++i)
		{
			mMultipleList.Add(new Multiple(this));
		}
	}
	public override void assignWindow()
	{
		newObject(out mHuResultRoot, "HuResultRoot");
		newObject(out mHu, mHuResultRoot, "Hu", 1);
		newObject(out mPingJu, mHuResultRoot, "PingJu", 1);
		newObject(out mMoneyResultRoot, "MoneyResultRoot");
		int charCount = mEndingCharacterList.Count;
		for(int i = 0; i < charCount; ++i)
		{
			mEndingCharacterList[i].assignWindow(mMoneyResultRoot, "Character" + i);
		}
		newObject(out mDetailRoot, "DetailRoot");
		newObject(out mMultipleTitle, mDetailRoot, "MultipleTitle");
		int multipleCount = mMultipleList.Count;
		for(int i = 0; i < multipleCount; ++i)
		{
			mMultipleList[i].assignWindow(mDetailRoot, "Multiple" + i);
		}
		newObject(out mButtonRoot, "ButtonRoot");
		newObject(out mContinueButton, mButtonRoot, "Continue");
		newObject(out mReturnButton, mButtonRoot, "Return");
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
		registeBoxColliderNGUI(mContinueButton, onContinueClick, onButtonPress);
		registeBoxColliderNGUI(mReturnButton, onReturnClick, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mContinueButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mReturnButton, Vector2.one);
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
		LayoutTools.ACTIVE_WINDOW(mMoneyResultRoot);
		LayoutTools.ACTIVE_WINDOW(mDetailRoot);
		LayoutTools.ACTIVE_WINDOW(mButtonRoot);
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
				mMultipleList[i].setDescribe(GameDefine.HU_NAME[(int)info.mHuList[i]]);
				mMultipleList[i].setMultiple(GameDefine.HU_MULTIPLE[(int)info.mHuList[i]]);
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
		mSocketNetManager.sendMessage<CSContinueGame>();
	}
	protected void onReturnClick(GameObject go)
	{
		mSocketNetManager.sendMessage<CSBackToMahjongHall>();
	}
	protected void onButtonPress(GameObject go, bool press)
	{
		txUIObject obj = mLayout.getUIObject(go);
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}