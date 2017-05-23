using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemDrop : Command
{
	public Character mPlayer;
	public MAHJONG mMahjong;
	public CommandMahjongSystemDrop(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		mahjongSystem.notifyPlayerDrop(mPlayer, mMahjong);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong + ", player : " + mPlayer.getName();
	}
}