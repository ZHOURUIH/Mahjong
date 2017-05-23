using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemRequestPass : Command
{
	public Character mCharacter;
	public CommandMahjongSystemRequestPass(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		mahjongSystem.playerConfirmAction(mCharacter, ACTION_TYPE.AT_PASS);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : player : " + mCharacter.getName();
	}
}