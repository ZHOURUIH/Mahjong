using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptRegister : LayoutScript
{
	protected txNGUIStaticSprite mBackground;
	protected txNGUIEditbox mAccountEdit;
	protected txNGUIEditbox mPasswordEdit;
	protected txNGUIEditbox mNameEdit;
	protected txNGUIButton mRegisterButton;
	protected txNGUIButton mCancelButton;
	public ScriptRegister(string name, GameLayout layout)
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
		newObject(out mNameEdit, mBackground, "NameEdit");
		newObject(out mRegisterButton, mBackground, "RegisterButton");
		newObject(out mCancelButton, mBackground, "CancelButton");
	}
	public override void init()
	{
		mGlobalTouchSystem.registeBoxCollider(mRegisterButton, onRegisterClick, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mCancelButton, onCancelClick, null, onButtonPress);
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
	//-------------------------------------------------------------------------------------------------------
	protected void onRegisterClick(txUIObject button)
	{
		// 发送注册消息
		CSRegister register = mSocketNetManager.createPacket<CSRegister>();
		register.setAccount(mAccountEdit.getText());
		register.setPassword(mPasswordEdit.getText());
		register.setName(mNameEdit.getText());
		register.mHead.mValue = 0;
		mSocketNetManager.sendMessage(register);
	}
	protected void onCancelClick(txUIObject button)
	{
		// 取消时返回登录流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onButtonPress(txUIObject button, bool press)
	{
		LayoutTools.SCALE_WINDOW(button, button.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}