using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptRegister : LayoutScript
{
	protected txUIStaticSprite mBackground;
	protected txUIEditbox mAccountEdit;
	protected txUIEditbox mPasswordEdit;
	protected txUIEditbox mNameEdit;
	protected txUIButton mRegisterButton;
	protected txUIButton mCancelButton;
	public ScriptRegister(LAYOUT_TYPE type, string name, GameLayout layout)
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
		mNameEdit = newObject<txUIEditbox>(mBackground, "NameEdit");
		mRegisterButton = newObject<txUIButton>(mBackground, "RegisterButton");
		mCancelButton = newObject<txUIButton>(mBackground, "CancelButton");
	}
	public override void init()
	{
		mRegisterButton.setClickCallback(onRegisterClick);
		mRegisterButton.setPressCallback(onButtonPress);
		mCancelButton.setClickCallback(onCancelClick);
		mCancelButton.setPressCallback(onButtonPress);
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
	protected void onRegisterClick(GameObject button)
	{
		// 发送注册消息
		CSRegister register = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_REGISTER) as CSRegister;
		register.setAccount(mAccountEdit.getText());
		register.setPassword(mPasswordEdit.getText());
		register.setName(mNameEdit.getText());
		register.mHead.mValue = 0;
		mSocketNetManager.sendMessage(register);
	}
	protected void onCancelClick(GameObject button)
	{
		// 取消时返回登录流程
		CommandGameSceneChangeProcedure cmd = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>();
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
		mCommandSystem.pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onButtonPress(GameObject button, bool press)
	{
		txUIObject obj = mLayout.getUIObject(button);
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}