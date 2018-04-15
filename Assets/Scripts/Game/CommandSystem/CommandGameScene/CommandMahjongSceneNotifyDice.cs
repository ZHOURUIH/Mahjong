using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyDice : Command
{
	public byte[] mDice;
	public override void init()
	{
		base.init();
		mDice = null;
	}
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE))
		{
			return;
		}
		// 开始掷骰子
		mScriptDice.setDiceResult(mDice);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : dice : " + mDice[0] + ", " + mDice[1];
	}
}