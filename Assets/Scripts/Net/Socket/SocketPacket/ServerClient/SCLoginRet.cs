using System;
using System.Collections;
using System.Collections.Generic;

public class SCLoginRet : SocketPacket
{
	protected byte mLoginRet;  // -1表示已经在其他地方登陆,0表示账号密码错误,1表示登陆成功
	protected int mRetGUID;		// 登录成功后返回的玩家的guid
	public SCLoginRet(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mLoginRet = BinaryUtility.readByte(data, ref index);
		mRetGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeByte(data, ref index, mLoginRet);
		BinaryUtility.writeInt(data, ref index, mRetGUID);
	}
	public override int getSize()
	{
		return sizeof(byte) + sizeof(int);
	}
	public override void execute()
	{
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_EXIT;
		mCommandSystem.pushCommand(cmd, mGameSceneManager.getCurScene());
	}
}