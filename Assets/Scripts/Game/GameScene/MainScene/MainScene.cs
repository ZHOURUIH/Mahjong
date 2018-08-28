using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScene : GameScene
{
	public MainScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{ }
	public override void assignStartExitProcedure()
	{
		mStartProcedure = PROCEDURE_TYPE.PT_MAIN_LOADING;
		mExitProcedure = PROCEDURE_TYPE.PT_MAIN_EXIT;
	}
	public override void createSceneProcedure()
	{
		addProcedure<MainSceneLoading>(PROCEDURE_TYPE.PT_MAIN_LOADING);
		addProcedure<MainSceneMainHall>(PROCEDURE_TYPE.PT_MAIN_MAIN_HALL);
		addProcedure<MainSceneRoomList>(PROCEDURE_TYPE.PT_MAIN_ROOM_LIST);
		addProcedure<MainSceneExit>(PROCEDURE_TYPE.PT_MAIN_EXIT);
		if (mSceneProcedureList.Count != (int)PROCEDURE_TYPE.PT_MAIN_MAX - (int)PROCEDURE_TYPE.PT_MAIN_MIN - 1)
		{
			Debug.LogError("error : not all procedure added!");
		}
	}
	public override void update(float elapsedTime)
	{ 
		base.update(elapsedTime); 
	}
}