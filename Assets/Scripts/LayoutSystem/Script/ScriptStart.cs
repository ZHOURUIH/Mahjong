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
		CSLogin login = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_LOGIN) as CSLogin;
		login.setAccount(mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_ACCOUNT));
		login.setPassword(mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_PASSWORD));
		mSocketNetManager.sendMessage(login);
	}
	protected void onStartPress(txUIButton obj, bool press)
	{
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}