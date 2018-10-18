using UnityEngine;
using System.Collections;

// 向服务器请求一页房间列表
public class CommandMainSceneRequestRoomList : Command
{
	public int mCurPageIndex;
	public override void init()
	{
		base.init();
		mCurPageIndex = 0;
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
		// 还未冷却请求CD,则不能请求
		if(!roomListProcedure.canRequestRoomList())
		{
			return;
		}
		MathUtility.clampMin(ref mCurPageIndex, 0);
		// 向服务器请求一页房间列表
		CSRequestRoomList requestRoomList = mSocketManager.createPacket<CSRequestRoomList>();
		requestRoomList.mMinIndex.mValue = (short)(mCurPageIndex * GameDefine.MAX_REQUEST_ROOM_COUNT);
		requestRoomList.mMaxIndex.mValue = (short)(requestRoomList.mMinIndex.mValue + GameDefine.MAX_REQUEST_ROOM_COUNT);
		mSocketManager.sendMessage(requestRoomList);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mCurPageIndex : " + mCurPageIndex;
	}
}