using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptDice : LayoutScript
{
	protected txNGUISpriteAnim mDiceAnim;
	protected txNGUIStaticSprite mDice0;
	protected txNGUIStaticSprite mDice1;
	public ScriptDice(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
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
		LayoutTools.ACTIVE_WINDOW(mDiceAnim);
		LayoutTools.ACTIVE_WINDOW(mDice0, false);
		LayoutTools.ACTIVE_WINDOW(mDice1, false);
	}
	public override void onShow(bool immediately, string param)
	{
		mDiceAnim.stop();
		mDiceAnim.play();
		mDiceAnim.setPlayEndCallback(onDiceAnimDone);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void setDiceResult(byte[] dice)
	{
		mDice0.setSpriteName("Dice" + dice[0]);
		mDice1.setSpriteName("Dice" + dice[1]);
	}
	//-----------------------------------------------------------------------------------
	protected void onDiceAnimDone(txNGUISpriteAnim window, object userData, bool isBreak)
	{
		LayoutTools.ACTIVE_WINDOW(mDice0);
		LayoutTools.ACTIVE_WINDOW(mDice1);
		// 骰子停留0.2秒后再通知场景
		CommandMahjongSceneNotifyDiceDone cmd = newCmd(out cmd, true, true);
		pushDelayCommand(cmd, mGameSceneManager.getCurScene(), 0.2f);
	}
}