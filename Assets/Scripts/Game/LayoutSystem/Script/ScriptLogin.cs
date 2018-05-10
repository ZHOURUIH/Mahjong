using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptLogin : LayoutScript
{
	protected txNGUIStaticSprite mBackground;
	protected txNGUIEditbox mAccountEdit;
	protected txNGUIEditbox mPasswordEdit;
	protected txNGUIButton mLoginButton;
	protected txNGUIButton mRegisterButton;
	protected txNGUIButton mQuitButton;
	public ScriptLogin(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
		newObject(out mAccountEdit, mBackground, "AccountEdit");
		newObject(out mPasswordEdit, mBackground, "PasswordEdit");
		newObject(out mLoginButton, mBackground, "LoginButton");
		newObject(out mRegisterButton, mBackground, "RegisterButton");
		newObject(out mQuitButton, mBackground, "QuitButton");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mLoginButton, onLoginClick, onButtonPress);
		registeBoxColliderNGUI(mRegisterButton, onRegisterClick, onButtonPress);
		registeBoxColliderNGUI(mQuitButton, onQuitClick, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mLoginButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mRegisterButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mQuitButton, Vector2.one);
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
	protected void onLoginClick(GameObject obj)
	{
		CSLogin login = mSocketNetManager.createPacket<CSLogin>();
		login.setAccount(mAccountEdit.getText());
		login.setPassword(mPasswordEdit.getText());
		mSocketNetManager.sendMessage(login);
	}
	protected void onButtonPress(GameObject button, bool press)
	{
		txUIObject obj = mLayout.getUIObject(button);
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
	protected void onRegisterClick(GameObject button)
	{
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_REGISTER;
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onQuitClick(GameObject button)
	{
		mGameFramework.stop();
	}
}