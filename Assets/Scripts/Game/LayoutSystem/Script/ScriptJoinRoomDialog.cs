using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptJoinRoomDialog : LayoutScript
{
	protected txNGUIEditbox mRoomIDEditbox;
	protected txNGUIButton mJoinButton;
	protected txNGUIButton mCancelButton;
	public ScriptJoinRoomDialog(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mRoomIDEditbox, "RoomIDEditbox");
		newObject(out mJoinButton, "JoinButton");
		newObject(out mCancelButton, "CancelButton");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mJoinButton, onJoinRoom);
		registeBoxColliderNGUI(mCancelButton, onCancel);
	}
	public override void onReset()
	{
		LT.SCALE_WINDOW(mJoinButton, Vector2.one);
		LT.SCALE_WINDOW(mCancelButton, Vector2.one);
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
	//----------------------------------------------------------------------------------------------------------
	protected void onJoinRoom(GameObject go)
	{
		CSJoinRoom join = mSocketManager.createPacket<CSJoinRoom>();
		join.mRoomID.mValue = stringToInt(mRoomIDEditbox.getText());
		mSocketManager.sendMessage(join);
	}
	protected void onCancel(GameObject go)
	{
		LT.HIDE_LAYOUT(mType);
	}
}