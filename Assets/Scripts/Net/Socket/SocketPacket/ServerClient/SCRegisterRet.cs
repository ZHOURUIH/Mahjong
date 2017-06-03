using System;
using System.Collections;
using System.Collections.Generic;

public class SCRegisterRet : SocketPacket
{
	public byte mResult;
	public SCRegisterRet(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mResult = BinaryUtility.readByte(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeByte(data, ref index, mResult);
	}
	public override int getSize()
	{
		return sizeof(byte);
	}
	public override void execute()
	{
		if (mResult == 0)
		{
			CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
			mCommandSystem.pushDelayCommand(cmd, mGameSceneManager.getCurScene());
		}
		else if(mResult == 1)
		{
			UnityUtility.logInfo("注册失败");
		}
	}
}