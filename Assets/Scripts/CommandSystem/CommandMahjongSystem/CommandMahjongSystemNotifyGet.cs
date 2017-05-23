using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemNotifyGet : Command
{
	public Character mCharacter;
	public MAHJONG mMahjong;
	public CommandMahjongSystemNotifyGet(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		mahjongSystem.notifyGet(mCharacter, mMahjong);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : player : " + mCharacter.getName() + ", mahjong : " + mMahjong;
	}
}