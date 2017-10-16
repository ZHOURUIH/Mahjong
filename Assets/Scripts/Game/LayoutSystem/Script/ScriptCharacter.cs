using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScriptCharacter : LayoutScript
{
	protected txUIStaticSprite mCharacterHead;
	protected txUIText mCharacterName;
	protected txUIText mCharacterID;
	protected txUIStaticSprite mMoneyIcon;
	protected txUIText mMoney;
	public ScriptCharacter(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mCharacterHead = newObject<txUIStaticSprite>("CharacterHead");
		mCharacterName = newObject<txUIText>("CharacterName");
		mCharacterID = newObject<txUIText>("CharacterID");
		mMoneyIcon = newObject<txUIStaticSprite>("MoneyIcon");
		mMoney = newObject<txUIText>(mMoneyIcon, "Money");
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
		mCharacterID.setLabel("ID:" + StringUtility.intToString(id));
		mMoney.setLabel(StringUtility.intToString(money));
	}
}