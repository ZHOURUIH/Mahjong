using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptRoomMenu : LayoutScript
{
	protected txNGUIStaticSprite mBackground;
	protected txNGUIButton mCreateRoomButton;
	protected txNGUIStaticSprite mCreateLabel;
	protected txNGUIButton mJoinRoomButton;
	protected txNGUIStaticSprite mJoinLabel;
	public ScriptRoomMenu(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
		newObject(out mCreateRoomButton, mBackground, "CreateRoomButton");
		newObject(out mCreateLabel, mCreateRoomButton, "CreateLabel");
		newObject(out mJoinRoomButton, mBackground, "JoinRoomButton");
		newObject(out mJoinLabel, mJoinRoomButton, "JoinLabel");
	}
	public override void init()
	{
		mGlobalTouchSystem.registeBoxCollider(mCreateRoomButton, onCreateClicked, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mJoinRoomButton, onJoinClicked, null, onButtonPress);
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
	protected void onCreateClicked(txUIObject obj)
	{
		// 向服务器发送创建房间的消息
		mSocketNetManager.sendMessage<CSCreateRoom>();
	}
	protected void onJoinClicked(txUIObject obj)
	{
		// 显示加入房间对话框
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG);
	}
	protected void onButtonPress(txUIObject obj, bool press)
	{
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}