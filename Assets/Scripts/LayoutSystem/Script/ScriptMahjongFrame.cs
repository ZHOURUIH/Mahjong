using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMahjongFrame : LayoutScript
{
	protected txUIObject mRoomInfoRoot;
	protected txUIText mRoomIDLabel;
	protected txUIButton mLeaveRoomButton;
	protected txUIButton mReadyButton;
	protected txUIButton mCancelReadyButton;
	public ScriptMahjongFrame(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mRoomInfoRoot = newObject<txUIObject>("RoomInfoRoot");
		mRoomIDLabel = newObject<txUIText>(mRoomInfoRoot, "RoomID");
		mLeaveRoomButton = newObject<txUIButton>("LeaveRoom");
		mReadyButton = newObject<txUIButton>("Ready");
		mCancelReadyButton = newObject<txUIButton>("CancelReady");
	}
	public override void init()
	{
		mLeaveRoomButton.setClickCallback(onLeaveRoomClick);
		mLeaveRoomButton.setPressCallback(onButtonPress);
		mReadyButton.setClickCallback(onReadyClick);
		mReadyButton.setPressCallback(onButtonPress);
		mCancelReadyButton.setClickCallback(onCancelReadyClick);
		mCancelReadyButton.setPressCallback(onButtonPress);
	}
	public override void onReset()
	{
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
		mRoomIDLabel.setText(StringUtility.intToString(roomID));
	}
	public void notifyReady(bool ready)
	{
		LayoutTools.ACTIVE_WINDOW(mReadyButton, !ready);
		LayoutTools.ACTIVE_WINDOW(mCancelReadyButton, ready);
	}
	//-----------------------------------------------------------------------------------
	protected void onReadyClick(GameObject go)
	{
		CommandCharacterReady cmd = new CommandCharacterReady();
		cmd.mReady = true;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
	}
	protected void onCancelReadyClick(GameObject go)
	{
		CommandCharacterReady cmd = new CommandCharacterReady();
		cmd.mReady = false;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
	}
	protected void onLeaveRoomClick(GameObject go)
	{
		CommandGameSceneManagerEnter cmd = new CommandGameSceneManagerEnter();
		cmd.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
		mCommandSystem.pushCommand(cmd, mGameSceneManager);
	}
	protected void onButtonPress(GameObject go, bool press)
	{
		txUIObject button = mLayout.getUIObject(go);
		LayoutTools.SCALE_WINDOW(button, button.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}