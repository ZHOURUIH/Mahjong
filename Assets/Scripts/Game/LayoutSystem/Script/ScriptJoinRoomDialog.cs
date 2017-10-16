using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptJoinRoomDialog : LayoutScript
{
	protected txUIEditbox mRoomIDEditbox;
	protected txUIButton mJoinButton;
	protected txUIButton mCancelButton;
	public ScriptJoinRoomDialog(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mRoomIDEditbox = newObject<txUIEditbox>("RoomIDEditbox");
		mJoinButton = newObject<txUIButton>("JoinButton");
		mCancelButton = newObject<txUIButton>("CancelButton");
	}
	public override void init()
	{
		mJoinButton.setClickCallback(onJoinRoom);
		mCancelButton.setClickCallback(onCancel);
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
	protected void onJoinRoom(GameObject go)
	{
		CSJoinRoom join = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_JOIN_ROOM) as CSJoinRoom;
		join.mRoomID.mValue = StringUtility.stringToInt(mRoomIDEditbox.getText());
		mSocketNetManager.sendMessage(join);
	}
	protected void onCancel(GameObject go)
	{
		LayoutTools.HIDE_LAYOUT(mType);
	}
}