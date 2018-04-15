using UnityEngine;
using System.Collections;

public class LogoSceneRegister : SceneProcedure
{
	public LogoSceneRegister(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
    {
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_REGISTER);
	}
	protected override void onUpdate(float elapsedTime)
    {
		;
    }
	protected override void onExit(SceneProcedure nextProcedure)
    {
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_REGISTER);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}