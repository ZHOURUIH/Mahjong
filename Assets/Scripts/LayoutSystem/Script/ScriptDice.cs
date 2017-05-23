using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptDice : LayoutScript
{
	protected txUISpriteAnim mDiceAnim;
	protected txUIStaticSprite mDice0;
	protected txUIStaticSprite mDice1;
	public ScriptDice(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		;
	}
	public override void init()
	{
		mDiceAnim = newObject<txUISpriteAnim>("DiceAnim");
		mDice0 = newObject<txUIStaticSprite>("Dice0", 0);
		mDice1 = newObject<txUIStaticSprite>("Dice1", 0);
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
		mDiceAnim.setPlayEndCallback(onDiceAnimDone, this);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void setDiceResult(int[] dice)
	{
		mDice0.setSpriteName("Dice" + dice[0]);
		mDice1.setSpriteName("Dice" + dice[1]);
	}
	//-----------------------------------------------------------------------------------
	protected void onDiceAnimDone(txUISpriteAnim window, object userData, bool isBreak)
	{
		LayoutTools.ACTIVE_WINDOW(mDice0);
		LayoutTools.ACTIVE_WINDOW(mDice1);
		// 骰子停留1秒后再通知场景
		CommandMahjongSceneNotifyDiceDone cmd = new CommandMahjongSceneNotifyDiceDone(true, true);
		mCommandSystem.pushDelayCommand(cmd, mGameSceneManager.getCurScene(), 2.0f);
	}
}