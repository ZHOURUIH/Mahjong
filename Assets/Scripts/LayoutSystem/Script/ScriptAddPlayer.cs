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
}