using UnityEngine;
using System.Collections;

public class LogoSceneRunning : SceneProcedure
{
    public LogoSceneRunning()
    { }
	public LogoSceneRunning(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
    {
		;
    }
	protected override void onUpdate(float elapsedTime)
    {
		;
    }
	protected override void onExit(SceneProcedure nextProcedure)
    {
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_START);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}