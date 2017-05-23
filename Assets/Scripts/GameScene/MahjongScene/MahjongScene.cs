using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongScene : GameScene
{
	public MahjongScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{ }
	public override void setFirstProcedureName()
	{
		mFirstProcedure = PROCEDURE_TYPE.PT_MAHJONG_LOADING;
	}
	public override void createSceneProcedure()
	{
		addProcedure<MahjongSceneLoading>(PROCEDURE_TYPE.PT_MAHJONG_LOADING);
		addProcedure<MahjongSceneWaiting>(PROCEDURE_TYPE.PT_MAHJONG_WAITING);
		MahjongSceneRunning running = addProcedure<MahjongSceneRunning>(PROCEDURE_TYPE.PT_MAHJONG_RUNNING);
		addProcedure<MahjongSceneRunningDice>(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE, running);
		addProcedure<MahjongSceneRunningGetStart>(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GET_START, running);
		addProcedure<MahjongSceneRunningGaming>(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_GAMING, running);
		addProcedure<MahjongSceneEnding>(PROCEDURE_TYPE.PT_MAHJONG_ENDING);
		addProcedure<MahjongSceneExit>(PROCEDURE_TYPE.PT_MAHJONG_EXIT);
		if (mSceneProcedureList.Count != (int)PROCEDURE_TYPE.PT_MAHJONG_MAX - (int)PROCEDURE_TYPE.PT_MAHJONG_MIN - 1)
		{
			Debug.LogError("error : not all procedure added!");
		}
	}
	public override void update(float elapsedTime)
	{ 
		base.update(elapsedTime); 
	}
}