using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 通知已经获取了一页房间列表
public class CommandMainSceneNotifyRoomList : Command
{
	public List<RoomInfo> mRoomInfoList;
	public int mAllRoomCount;
	public override void init()
	{
		base.init();
		mRoomInfoList = null;
		mAllRoomCount = 0;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAIN)
		{
			return;
		}
		MainScene mainScene = gameScene as MainScene;
		if (!mainScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST))
		{
			return;
		}
		MainSceneRoomList roomListProcedure = mainScene.getCurOrParentProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
		roomListProcedure.notifyRoomList();
		// 通知房间系统获得了房间列表
		mRoomSystem.clearRoomInfo();
		int count = mRoomInfoList != null ? mRoomInfoList.Count : 0;
		for(int i = 0; i < count; ++i)
		{
			mRoomSystem.addRoomInfo(mRoomInfoList[i]);
		}
		// 通知界面显示房间列表
		mScriptRoomList.showRoomList(mRoomSystem.getRoomInfoList(), roomListProcedure.getCurPage(), mAllRoomCount);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo();
	}
}