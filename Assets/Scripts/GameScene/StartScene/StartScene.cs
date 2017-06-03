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
	public override void setFirstProcedureName() { mFirstProcedure = PROCEDURE_TYPE.PT_START_LOADING; }
	public override void createSceneProcedure()
	{
		addProcedure<LogoSceneLoading>(PROCEDURE_TYPE.PT_START_LOADING);
		addProcedure<LogoSceneLogin>(PROCEDURE_TYPE.PT_START_LOGIN);
		addProcedure<LogoSceneRegister>(PROCEDURE_TYPE.PT_START_REGISTER);
	}
}
