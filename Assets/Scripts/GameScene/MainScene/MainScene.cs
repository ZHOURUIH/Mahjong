using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScene : GameScene
{
	public MainScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{ }
	public override void setFirstProcedureName()
	{
		mFirstProcedure = PROCEDURE_TYPE.PT_MAIN_LOADING;
	}
	public override void createSceneProcedure()
	{
		addProcedure<MainSceneLoading>(PROCEDURE_TYPE.PT_MAIN_LOADING);
		addProcedure<MainSceneRunning>(PROCEDURE_TYPE.PT_MAIN_RUNNING);
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