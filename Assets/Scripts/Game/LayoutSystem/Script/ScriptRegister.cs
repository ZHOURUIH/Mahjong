﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptRegister : LayoutScript
{
	protected txNGUISprite mBackground;
	protected txNGUIEditbox mAccountEdit;
	protected txNGUIEditbox mPasswordEdit;
	protected txNGUIEditbox mNameEdit;
	protected txNGUIButton mRegisterButton;
	protected txNGUIButton mCancelButton;
	protected txNGUIButton mCheckNameButton;
	protected txNGUIText mValidNameTip;
	protected txNGUIText mInvalidNameTip;
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
		newObject(out mCheckNameButton, mBackground, "CheckNameButton");
		newObject(out mValidNameTip, mBackground, "ValidNameTip", 0);
		newObject(out mInvalidNameTip, mBackground, "InvalidNameTip", 0);
	}
	public override void init()
	{
		registeBoxColliderNGUI(mRegisterButton, onRegisterClick, onButtonPress);
		registeBoxColliderNGUI(mCancelButton, onCancelClick, onButtonPress);
		registeBoxColliderNGUI(mCheckNameButton, onCheckNameClick, onButtonPress);
	}
	public override void onReset()
	{
		LT.SCALE_WINDOW(mRegisterButton);
		LT.SCALE_WINDOW(mCancelButton);
		LT.SCALE_WINDOW(mCheckNameButton);
		LT.ACTIVE_WINDOW(mCheckNameButton);
		LT.ACTIVE_WINDOW(mValidNameTip, false);
		LT.ACTIVE_WINDOW(mInvalidNameTip, false);
		mRegisterButton.setHandleInput(false);
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
	public void setNameCheckRet(bool available)
	{
		mRegisterButton.setHandleInput(available);
		mCheckNameButton.setHandleInput(!available);
		LT.ACTIVE_WINDOW(mCheckNameButton, !available);
		LT.ACTIVE_WINDOW(mValidNameTip, available);
		LT.ACTIVE_WINDOW(mInvalidNameTip, !available);
	}
	//-------------------------------------------------------------------------------------------------------
	protected void onRegisterClick(GameObject button)
	{
		// 发送注册消息
		CSRegister register = mSocketManager.createPacket<CSRegister>();
		register.setAccount(mAccountEdit.getText());
		register.setPassword(mPasswordEdit.getText());
		register.setName(mNameEdit.getText());
		register.mHead.mValue = 0;
		mSocketManager.sendMessage(register);
	}
	protected void onCancelClick(GameObject button)
	{
		// 取消时返回登录流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onCheckNameClick(GameObject button)
	{
		string nameText = mNameEdit.getText();
		if(nameText != "")
		{
			CSCheckName checkName = mSocketManager.createPacket<CSCheckName>();
			byte[] nameBytes = stringToBytes(nameText, BinaryUtility.getGB2312());
			checkName.setName(nameBytes);
			mSocketManager.sendMessage(checkName);
			// 检测按钮点击后就禁用该按钮
			mCheckNameButton.setHandleInput(false);
		}
	}
	protected void onButtonPress(GameObject button, bool press)
	{
		txUIObject obj = mLayout.getUIObject(button);
		LT.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}