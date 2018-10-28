using UnityEngine;
using System.Collections;

public class LogoSceneLogin : SceneProcedure
{
	public LogoSceneLogin(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
    {
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_LOGIN);
	}
	protected override void onUpdate(float elapsedTime)
    {
		;
    }
	protected override void onExit(SceneProcedure nextProcedure)
    {
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_LOGIN);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}