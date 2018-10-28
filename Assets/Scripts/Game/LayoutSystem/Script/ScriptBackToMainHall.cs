using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptBackToMainHall : LayoutScript
{
	protected txNGUIButton	mBackButton;
	protected txNGUIText	mBackLabel;
	public ScriptBackToMainHall(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackButton, "BackButton");
		newObject(out mBackLabel, mBackButton, "Label");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mBackButton, onBackClicked, onButtonPress);
	}
	public override void onReset()
	{
		LT.SCALE_WINDOW(mBackButton);
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
	//-----------------------------------------------------------------------------------
	protected void onBackClicked(GameObject obj)
	{
		// 从房间列表流程返回到大厅流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_MAIN_HALL;
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onButtonPress(GameObject obj, bool press)
	{
		txUIObject window = mLayout.getUIObject(obj);
		LT.SCALE_WINDOW(window, window.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}