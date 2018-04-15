using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptAddPlayer : LayoutScript
{
	protected List<string> mNameList;
	protected txNGUIButton mAdd;
	protected txNGUIButton mLeftReady;
	protected txNGUIButton mOppositeReady;
	protected txNGUIButton mRightReady;
	protected txNGUIButton mMyselfReady;
	public ScriptAddPlayer(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mNameList = new List<string>();
		mNameList.Add("player0");
		mNameList.Add("player1");
		mNameList.Add("player2");
	}
	public override void assignWindow()
	{
		newObject(out mAdd, "AddButton");
		newObject(out mLeftReady, "LeftReadyButton");
		newObject(out mOppositeReady, "OppositeReadyButton");
		newObject(out mRightReady, "RightReadyButton");
		newObject(out mMyselfReady, "MyselfReadyButton");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		;
	}
	public override void onGameState()
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
}