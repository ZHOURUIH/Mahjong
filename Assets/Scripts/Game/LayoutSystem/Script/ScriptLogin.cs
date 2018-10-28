using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptLogin : LayoutScript
{
	protected txNGUISprite mBackground;
	protected txNGUIEditbox mAccountEdit;
	protected txNGUIEditbox mPasswordEdit;
	protected txNGUIButton mLoginButton;
	protected txNGUIButton mRegisterButton;
	protected txNGUIButton mQuitButton;
	protected txUIObject mTipMask;
	protected txNGUISprite mTipBackground;
	protected txNGUIText mTipLabel;
	protected txNGUIButton mCancelButton;
	protected bool mTestAccount = false;
	protected float mCurTime;
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
		newObject(out mTipMask, mBackground, "TipMask", 0);
		newObject(out mTipBackground, mTipMask, "TipBackground");
		newObject(out mTipLabel, mTipBackground, "TipLabel");
		newObject(out mCancelButton, mTipBackground, "CancelButton");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mLoginButton, onLoginClick, onButtonPress);
		registeBoxColliderNGUI(mRegisterButton, onRegisterClick, onButtonPress);
		registeBoxColliderNGUI(mQuitButton, onQuitClick, onButtonPress);
		registeBoxColliderNGUI(mCancelButton, onQuitClick, onButtonPress);
	}
	public override void onReset()
	{
		LT.SCALE_WINDOW(mLoginButton);
		LT.SCALE_WINDOW(mRegisterButton);
		LT.SCALE_WINDOW(mQuitButton);
		LT.SCALE_WINDOW(mCancelButton);
		mCurTime = 0.0f;
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
		if(mTipMask.isActive())
		{
			mCurTime += elapsedTime;
			string dotSuffix = "";
			int dotCount = (int)(mCurTime / 0.5f);
			if(dotCount >= 4)
			{
				mCurTime = 0.0f;
			}
			else
			{
				for(int i = 0; i < dotCount; ++i)
				{
					dotSuffix += ".";
				}
			}
			mTipLabel.setLabel("登录中" + dotSuffix);
		}
	}
	//---------------------------------------------------------------------------------------------------------------------
	protected void onLoginClick(GameObject obj)
	{
		if(!mTestAccount)
		{
			CSLogin login = mSocketManager.createPacket<CSLogin>();
			login.setAccount(mAccountEdit.getText());
			login.setPassword(mPasswordEdit.getText());
			mSocketManager.sendMessage(login);
			// 发送登录消息后显示正在登录的提示框
			LT.ACTIVE_WINDOW(mTipMask);
			mCurTime = 0.0f;
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
		LT.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
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
	protected void onCancelClick(GameObject button)
	{
		// 关闭登录提示框,然后发送消息取消登录
		LT.ACTIVE_WINDOW(mTipMask, false);
		mSocketManager.sendMessage<CSCancelLogin>();
	}
}