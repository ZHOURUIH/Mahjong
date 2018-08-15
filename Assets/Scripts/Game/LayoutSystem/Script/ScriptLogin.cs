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
	protected bool mTestAccount = false;
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
		LayoutTools.SCALE_WINDOW(mLoginButton);
		LayoutTools.SCALE_WINDOW(mRegisterButton);
		LayoutTools.SCALE_WINDOW(mQuitButton);
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
		if(!mTestAccount)
		{
			CSLogin login = mSocketNetManager.createPacket<CSLogin>();
			login.setAccount(mAccountEdit.getText());
			login.setPassword(mPasswordEdit.getText());
			mSocketNetManager.sendMessage(login);
		}
		else
		{
			// 创建玩家
			CommandCharacterManagerCreateCharacter cmdCreate = newCmd(out cmdCreate);
			cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
			cmdCreate.mName = "测试";
			cmdCreate.mID = 0;
			pushCommand(cmdCreate, mCharacterManager);
			// 设置角色数据
			CharacterMyself myself = mCharacterManager.getMyself();
			CharacterData data = myself.getCharacterData();
			data.mMoney = 100;
			data.mHead = 1;

			// 进入到主场景
			CommandGameSceneManagerEnter cmdEnterMain = newCmd(out cmdEnterMain, true, true);
			cmdEnterMain.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			pushDelayCommand(cmdEnterMain, mGameSceneManager);
		}
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