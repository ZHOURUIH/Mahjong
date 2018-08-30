using UnityEngine;
using System.Collections;

public class CommandMainSceneNotifyAutoRequest : Command
{
	public bool mAutoRequest;
	public override void init()
	{
		base.init();
		mAutoRequest = true;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAIN)
		{
			return;
		}
		MainScene mainScene = gameScene as MainScene;
		if(!mainScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST))
		{
			return;
		}
		MainSceneRoomList roomListProcedure = mainScene.getCurOrParentProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
		roomListProcedure.setAutoRequestRoomList(mAutoRequest);
		// 通知布局
		mScriptRoomList.confirmAutoRequest(mAutoRequest, roomListProcedure.getCurRequestTime());
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mAutoRequest : " + mAutoRequest;
	}
}