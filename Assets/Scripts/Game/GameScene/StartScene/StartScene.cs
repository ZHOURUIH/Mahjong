using UnityEngine;
using System.Collections;

public class StartScene : GameScene
{
	public StartScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void assignStartExitProcedure()
	{
		mStartProcedure = PROCEDURE_TYPE.PT_START_LOADING;
		mExitProcedure = PROCEDURE_TYPE.PT_START_EXIT;
	}
	public override void createSceneProcedure()
	{
		addProcedure<LogoSceneLoading>(PROCEDURE_TYPE.PT_START_LOADING);
		addProcedure<LogoSceneLogin>(PROCEDURE_TYPE.PT_START_LOGIN);
		addProcedure<LogoSceneRegister>(PROCEDURE_TYPE.PT_START_REGISTER);
		addProcedure<LogoSceneExit>(PROCEDURE_TYPE.PT_START_EXIT);
		if (mSceneProcedureList.Count != (int)PROCEDURE_TYPE.PT_START_MAX - (int)PROCEDURE_TYPE.PT_START_MIN - 1)
		{
			Debug.LogError("error : not all procedure added!");
		}
	}
}
