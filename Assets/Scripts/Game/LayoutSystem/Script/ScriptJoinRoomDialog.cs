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
		mGlobalTouchSystem.registeBoxCollider(mJoinButton, onJoinRoom);
		mGlobalTouchSystem.registeBoxCollider(mCancelButton, onCancel);
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
	//----------------------------------------------------------------------------------------------------------
	protected void onJoinRoom(txUIObject go)
	{
		CSJoinRoom join = mSocketNetManager.createPacket<CSJoinRoom>();
		join.mRoomID.mValue = StringUtility.stringToInt(mRoomIDEditbox.getText());
		mSocketNetManager.sendMessage(join);
	}
	protected void onCancel(txUIObject go)
	{
		LayoutTools.HIDE_LAYOUT(mType);
	}
}