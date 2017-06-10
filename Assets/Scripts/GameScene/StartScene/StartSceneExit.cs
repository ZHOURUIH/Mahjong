using UnityEngine;
using System.Collections;

public class LogoSceneExit : SceneProcedure
{
    public LogoSceneExit()
    { }
	public LogoSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
    {
		// 隐藏该场景的所有布局
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_LOGIN, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_REGISTER, true);
	}
	protected override void onUpdate(float elapsedTime)
    {
		;
    }
	protected override void onExit(SceneProcedure nextProcedure)
    {
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}