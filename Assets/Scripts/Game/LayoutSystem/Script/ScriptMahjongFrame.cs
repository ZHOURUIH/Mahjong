using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongFrame : LayoutScript
{
	protected txUIObject mRoomInfoRoot;
	protected txNGUIText mRoomIDLabel;
	protected txNGUIButton mLeaveRoomButton;
	protected txNGUIButton mReadyButton;
	protected txNGUIButton mCancelReadyButton;
	protected txNGUIText mInfo;
	protected txUIObject mMahjongPoolSize;
	protected txNGUIText mCountLabel;
	public ScriptMahjongFrame(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mRoomInfoRoot, "RoomInfoRoot");
		newObject(out mRoomIDLabel, mRoomInfoRoot, "RoomID");
		newObject(out mLeaveRoomButton, "LeaveRoom");
		newObject(out mReadyButton, "Ready");
		newObject(out mCancelReadyButton, "CancelReady");
		newObject(out mInfo, "Info");
		newObject(out mMahjongPoolSize, "MahjongPoolSize");
		newObject(out mCountLabel, mMahjongPoolSize, "CountLabel");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mLeaveRoomButton, onLeaveRoomClick, onButtonPress);
		registeBoxColliderNGUI(mReadyButton, onReadyClick, onButtonPress);
		registeBoxColliderNGUI(mCancelReadyButton, onCancelReadyClick, onButtonPress);
	}
	public override void onReset()
	{
		LT.SCALE_WINDOW(mLeaveRoomButton, Vector2.one);
		LT.SCALE_WINDOW(mReadyButton, Vector2.one);
		LT.SCALE_WINDOW(mCancelReadyButton, Vector2.one);
		notifyReady(false);
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
	public void setRoomID(int roomID)
	{
		mRoomIDLabel.setLabel(intToString(roomID));
	}
	public void notifyReady(bool ready)
	{
		LT.ACTIVE_WINDOW(mReadyButton, !ready);
		LT.ACTIVE_WINDOW(mCancelReadyButton, ready);
	}
	public void notifyStartGame()
	{
		LT.ACTIVE_WINDOW(mReadyButton, false);
		LT.ACTIVE_WINDOW(mCancelReadyButton, false);
	}
	public void notifyInfo(string info)
	{
		mInfo.setLabel(info);
	}
	public void setMahjongPoolSize(int count)
	{
		mCountLabel.setLabel(count + "个");
	}
	//-----------------------------------------------------------------------------------
	protected void onReadyClick(GameObject go)
	{
		// 发送消息通知服务器玩家已经准备
		CSReady packetReady = mSocketManager.createPacket<CSReady>();
		packetReady.mReady.mValue = true;
		mSocketManager.sendMessage(packetReady);
	}
	protected void onCancelReadyClick(GameObject go)
	{
		// 发送消息通知服务器玩家已经准备
		CSReady packetReady = mSocketManager.createPacket<CSReady>();
		packetReady.mReady.mValue = false;
		mSocketManager.sendMessage(packetReady);
	}
	protected void onLeaveRoomClick(GameObject go)
	{
		mSocketManager.sendMessage<CSLeaveRoom>();
	}
	protected void onButtonPress(GameObject go, bool press)
	{
		txUIObject obj = mLayout.getUIObject(go);
		LT.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}