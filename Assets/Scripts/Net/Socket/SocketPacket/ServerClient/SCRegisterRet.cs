using System;
using System.Collections;
using System.Collections.Generic;

public class SCRegisterRet : SocketPacket
{
	protected BYTE mResult = new BYTE();
	public SCRegisterRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mResult);
	}
	public override void execute()
	{
		if (mResult.mValue == 0)
		{
			CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
			mCommandSystem.pushDelayCommand(cmd, mGameSceneManager.getCurScene());
		}
		else if (mResult.mValue == 1)
		{
			UnityUtility.logInfo("注册失败");
		}
	}
}