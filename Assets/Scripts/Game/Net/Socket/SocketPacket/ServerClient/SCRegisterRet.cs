using System;
using System.Collections;
using System.Collections.Generic;

public class SCRegisterRet : SocketPacket
{
	public BYTE mResult = new BYTE();
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
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
			pushDelayCommand(cmd, mGameSceneManager.getCurScene());
		}
		else if (mResult.mValue == 1)
		{
			string info = "注册失败!";
			GameUtility.messageOK(info);
			UnityUtility.logInfo(info);
		}
	}
}