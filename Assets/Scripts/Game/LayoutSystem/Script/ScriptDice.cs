using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptDice : LayoutScript
{
	protected txNGUISpriteAnim mDiceAnim;
	protected txNGUISprite mDice0;
	protected txNGUISprite mDice1;
	public ScriptDice(string name, GameLayout layout)
		:
		base(name, layout)
	{}
	public override void assignWindow()
	{
		newObject(out mDiceAnim, "DiceAnim");
		newObject(out mDice0, "Dice0", 0);
		newObject(out mDice1, "Dice1", 0);
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		LT.ACTIVE_WINDOW(mDiceAnim);
		LT.ACTIVE_WINDOW(mDice0, false);
		LT.ACTIVE_WINDOW(mDice1, false);
	}
	public override void onGameState()
	{
		base.onGameState();
		byte[] dice = mMahjongSystem.getDice();
		mDice0.setSpriteName("Dice" + dice[0]);
		mDice1.setSpriteName("Dice" + dice[1]);
	}
	public override void onShow(bool immediately, string param)
	{
		mDiceAnim.stop();
		mDiceAnim.play();
		mDiceAnim.addPlayEndCallback(onDiceAnimDone);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public float getDiceAnimTime()
	{
		return mDiceAnim.getInterval() * mDiceAnim.getTextureFrameCount();
	}
	//-----------------------------------------------------------------------------------
	protected void onDiceAnimDone(INGUIAnimation window, bool isBreak)
	{
		LT.ACTIVE_WINDOW(mDice0);
		LT.ACTIVE_WINDOW(mDice1);
	}
}