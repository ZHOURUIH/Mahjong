using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptStart : LayoutScript
{
	protected txUIStaticTexture mBackground;
	protected txUIButton mStartButton;
	public ScriptStart(LAYOUT_TYPE type, string name, GameLayout layout)
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
		mBackground = newObject<txUIStaticTexture>("Background");
		mStartButton = newObject<txUIButton>(mBackground, "StartButton");
		mGlobalTouchSystem.registerBoxCollider(mStartButton, onStartButtonClick, null, onStartPress);
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
	//---------------------------------------------------------------------------------------------------------------------
	protected void onStartButtonClick(txUIButton obj)
	{
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_EXIT;
		mCommandSystem.pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onStartPress(txUIButton obj, bool press)
	{
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}