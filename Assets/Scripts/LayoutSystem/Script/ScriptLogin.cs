using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptLogin : LayoutScript
{
	protected txUIStaticSprite mBackground;
	protected txUIEditbox mAccountEdit;
	protected txUIEditbox mPasswordEdit;
	protected txUIButton mLoginButton;
	protected txUIButton mRegisterButton;
	protected txUIButton mQuitButton;
	public ScriptLogin(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mBackground = newObject<txUIStaticSprite>("Background");
		mAccountEdit = newObject<txUIEditbox>(mBackground, "AccountEdit");
		mPasswordEdit = newObject<txUIEditbox>(mBackground, "PasswordEdit");
		mLoginButton = newObject<txUIButton>(mBackground, "LoginButton");
		mRegisterButton = newObject<txUIButton>(mBackground, "RegisterButton");
		mQuitButton = newObject<txUIButton>(mBackground, "QuitButton");
	}
	public override void init()
	{
		mLoginButton.setClickCallback(onLoginClick);
		mLoginButton.setPressCallback(onButtonPress);
		mRegisterButton.setClickCallback(onRegisterClick);
		mRegisterButton.setPressCallback(onButtonPress);
		mQuitButton.setClickCallback(onQuitClick);
		mQuitButton.setPressCallback(onButtonPress);
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
	protected void onLoginClick(GameObject obj)
	{
		CSLogin login = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_LOGIN) as CSLogin;
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
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_REGISTER;
		mCommandSystem.pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onQuitClick(GameObject button)
	{
		mGameFramework.stop();
	}
}