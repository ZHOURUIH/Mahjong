using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyDice : Command
{
	public byte[] mDice;
	public CommandMahjongSceneNotifyDice(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE))
		{
			return;
		}
		ScriptDice diceScript = mLayoutManager.getScript(LAYOUT_TYPE.LT_DICE) as ScriptDice;
		diceScript.setDiceResult(mDice);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : dice : " + mDice[0] + ", " + mDice[1];
	}
}