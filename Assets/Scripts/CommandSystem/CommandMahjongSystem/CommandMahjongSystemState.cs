using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemState : Command
{
	public MAHJONG_PLAY_STATE mPlayState;
	public override void init()
	{
		base.init();
		mPlayState = MAHJONG_PLAY_STATE.MPS_ENDING;
	}
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		mahjongSystem.notifyPlayState(mPlayState);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + "state : " + mPlayState;
	}
}