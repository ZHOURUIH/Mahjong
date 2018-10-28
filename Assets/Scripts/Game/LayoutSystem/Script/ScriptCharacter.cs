using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScriptCharacter : LayoutScript
{
	protected txNGUISprite mCharacterHead;
	protected txNGUIText mCharacterName;
	protected txNGUIText mCharacterID;
	protected txNGUISprite mMoneyIcon;
	protected txNGUIText mMoney;
	public ScriptCharacter(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mCharacterHead, "CharacterHead");
		newObject(out mCharacterName, "CharacterName");
		newObject(out mCharacterID, "CharacterID");
		newObject(out mMoneyIcon, "MoneyIcon");
		newObject(out mMoney, mMoneyIcon, "Money");
	}
	public override void init()
	{
		;
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
	public void setCharacterInfo(int head, string name, int id, int money)
	{
		mCharacterHead.setSpriteName("Head" + head);
		mCharacterName.setLabel(name);
		mCharacterID.setLabel("ID:" + intToString(id));
		mMoney.setLabel(intToString(money));
	}
}