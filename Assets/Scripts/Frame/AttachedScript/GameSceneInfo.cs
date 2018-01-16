using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameSceneInfo : MonoBehaviour
{
	public GameScene mGameScene;
	public PROCEDURE_TYPE mCurProcedure;
	public void Start()
	{
		mGameScene = GameBase.mGameSceneManager.getCurScene();
	}
	public void Update()
	{
		SceneProcedure sceneProcedure = mGameScene.getCurSceneProcedure();
		if(sceneProcedure != null)
		{
			mCurProcedure = sceneProcedure.getProcedureType();
		}
	}
}