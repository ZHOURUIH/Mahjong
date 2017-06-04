using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptAddPlayer : LayoutScript
{
	protected List<string> mNameList;
	protected txUIButton mAdd;
	protected txUIButton mLeftReady;
	protected txUIButton mOppositeReady;
	protected txUIButton mRightReady;
	protected txUIButton mMyselfReady;
	public ScriptAddPlayer(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mNameList = new List<string>();
		mNameList.Add("player0");
		mNameList.Add("player1");
		mNameList.Add("player2");
	}
	public override void assignWindow()
	{
		mAdd = newObject<txUIButton>("AddButton");
		mLeftReady = newObject<txUIButton>("LeftReadyButton");
		mOppositeReady = newObject<txUIButton>("OppositeReadyButton");
		mRightReady = newObject<txUIButton>("RightReadyButton");
		mMyselfReady = newObject<txUIButton>("MyselfReadyButton");
	}
	public override void init()
	{
		mAdd.setClickCallback(onAdd);
		mLeftReady.setClickCallback(onLeftReady);
		mOppositeReady.setClickCallback(onOppositeReady);
		mRightReady.setClickCallback(onRightReady);
		mMyselfReady.setClickCallback(onMyselfReady);
	}
	public override void onReset()
	{
		;
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
	//-------------------------------------------------------------------------------------------------------
	protected void onAdd(GameObject button)
	{
		int nameCount = mNameList.Count;
		for (int i = 0; i < nameCount; ++i)
		{
			string name = mNameList[i];
			Character character = mCharacterManager.getCharacter(name);
			if (character == null)
			{
				CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
				cmdCreate.mName = name;
				cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
				mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
				character = cmdCreate.mResultCharacter;
				CharacterData data = character.getCharacterData();
				data.mGUID = i + 1;
				data.mMoney = 200;
				data.mHead = i + 1;
				CommandMahjongSystemJoin cmdJoin = new CommandMahjongSystemJoin();
				cmdJoin.mCharacter = character;
				mCommandSystem.pushCommand(cmdJoin, mMahjongSystem);
			}
		}
	}
	protected void onLeftReady(GameObject button)
	{
		Character character = mCharacterManager.getCharacter(mNameList[0]);
		if (character != null)
		{
			CommandCharacterReady cmd = new CommandCharacterReady();
			mCommandSystem.pushCommand(cmd, character);
		}
	}
	protected void onOppositeReady(GameObject button)
	{
		Character character = mCharacterManager.getCharacter(mNameList[1]);
		if (character != null)
		{
			CommandCharacterReady cmd = new CommandCharacterReady();
			mCommandSystem.pushCommand(cmd, character);
		}
	}
	protected void onRightReady(GameObject button)
	{
		Character character = mCharacterManager.getCharacter(mNameList[2]);
		if (character != null)
		{
			CommandCharacterReady cmd = new CommandCharacterReady();
			mCommandSystem.pushCommand(cmd, character);
		}
	}
	protected void onMyselfReady(GameObject button)
	{
		Character character = mCharacterManager.getMyself();
		if (character != null)
		{
			CommandCharacterReady cmd = new CommandCharacterReady();
			mCommandSystem.pushCommand(cmd, character);
		}
	}
}