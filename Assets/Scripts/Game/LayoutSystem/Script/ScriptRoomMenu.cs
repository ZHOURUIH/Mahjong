using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptRoomMenu : LayoutScript
{
	protected txNGUISprite	mBackground;
	protected txNGUIButton	mCreateRoomButton;
	protected txNGUIText	mCreateLabel;
	protected txNGUIButton	mJoinRoomButton;
	protected txNGUIText	mJoinLabel;
	protected txNGUIButton	mFreeMatchButton;
	protected txNGUIText	mFreeMatchLabel;
	protected txNGUIButton	mRoomListButton;
	protected txNGUIText	mRoomListLabel;
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
		newObject(out mCreateLabel, mCreateRoomButton, "Label");
		newObject(out mJoinRoomButton, mBackground, "JoinRoomButton");
		newObject(out mJoinLabel, mJoinRoomButton, "Label");
		newObject(out mFreeMatchButton, mBackground, "FreeMatchButton");
		newObject(out mFreeMatchLabel, mFreeMatchButton, "Label");
		newObject(out mRoomListButton, mBackground, "RoomListButton");
		newObject(out mRoomListLabel, mRoomListButton, "Label");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mCreateRoomButton, onCreateClicked, onButtonPress);
		registeBoxColliderNGUI(mJoinRoomButton, onJoinClicked, onButtonPress);
		registeBoxColliderNGUI(mFreeMatchButton, onFreeMatchClicked, onButtonPress);
		registeBoxColliderNGUI(mRoomListButton, onRoomListClicked, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mCreateRoomButton);
		LayoutTools.SCALE_WINDOW(mJoinRoomButton);
		LayoutTools.SCALE_WINDOW(mFreeMatchButton);
		LayoutTools.SCALE_WINDOW(mRoomListButton);
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
	protected void onCreateClicked(GameObject obj)
	{
		// 向服务器发送创建房间的消息
		mSocketManager.sendMessage<CSCreateRoom>();
	}
	protected void onJoinClicked(GameObject obj)
	{
		// 显示加入房间对话框
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG);
	}
	protected void onFreeMatchClicked(GameObject obj)
	{
		// 显示正在自由匹配的提示界面
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_FREE_MATCH_TIP);
		// 发送消息自由匹配
		mSocketManager.sendMessage<CSFreeMatch>();
	}
	protected void onRoomListClicked(GameObject obj)
	{
		// 进入到房间列表流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_ROOM_LIST;
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
	protected void onButtonPress(GameObject obj, bool press)
	{
		txUIObject window = mLayout.getUIObject(obj);
		LayoutTools.SCALE_WINDOW(window, window.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}