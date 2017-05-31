using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptRoomMenu : LayoutScript
{
	protected txUIStaticSprite mBackground;
	protected txUIButton mCreateRoomButton;
	protected txUIStaticSprite mCreateLabel;
	protected txUIButton mJoinRoomButton;
	protected txUIStaticSprite mJoinLabel;
	public ScriptRoomMenu(LAYOUT_TYPE type, string name, GameLayout layout)
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
		mBackground = newObject<txUIStaticSprite>("Background");
		mCreateRoomButton = newObject<txUIButton>(mBackground, "CreateRoomButton");
		mCreateLabel = newObject<txUIStaticSprite>(mCreateRoomButton, "CreateLabel");
		mJoinRoomButton = newObject<txUIButton>(mBackground, "JoinRoomButton");
		mJoinLabel = newObject<txUIStaticSprite>(mJoinRoomButton, "JoinLabel");

		mGlobalTouchSystem.registerBoxCollider(mCreateRoomButton, onCreateClicked, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mCreateRoomButton, onCreateClicked, null, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mCreateRoomButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mJoinRoomButton, Vector2.one, Vector2.one, 0.0f);
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
	protected void onCreateClicked(txUIButton obj)
	{
		// 向服务器发送创建房间的消息
		CSCreateRoom createRoom = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_CREATE_ROOM) as CSCreateRoom;
		mSocketNetManager.sendMessage(createRoom);
	}
	protected void onJoinClicked(txUIButton obj)
	{
		;
	}
	protected void onButtonPress(txUIButton obj, bool press)
	{
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}