using System;
using System.Collections;
using System.Collections.Generic;

public class SCCreateRoomRet : SocketPacket
{
	protected byte mResult;  // 0表示成功,1表示失败
	protected int mRoomID;
	public SCCreateRoomRet(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mResult = BinaryUtility.readByte(data, ref index);
		mRoomID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeByte(data, ref index, mResult);
		BinaryUtility.writeInt(data, ref index, mRoomID);
	}
	public override int getSize()
	{
		return sizeof(byte) + sizeof(int);
	}
	public override void execute()
	{
		// 创建房间成功,进入麻将场景
		if(mResult == 0)
		{
			CommandGameSceneManagerEnter cmd = new CommandGameSceneManagerEnter();
			cmd.mSceneType = GAME_SCENE_TYPE.GST_MAHJONG;
			mCommandSystem.pushDelayCommand(cmd, mGameSceneManager);
		}
		else
		{
			UnityUtility.logInfo("创建房间失败!");
		}
	}
}