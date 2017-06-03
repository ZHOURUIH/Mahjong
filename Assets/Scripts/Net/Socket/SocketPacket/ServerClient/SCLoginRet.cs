using System;
using System.Collections;
using System.Collections.Generic;

public class SCLoginRet : SocketPacket
{
	protected byte mLoginRet;  // -1表示已经在其他地方登陆,0表示账号密码错误,1表示登陆成功
	protected byte[] mName = new byte[16];
	protected int mMoney;
	protected short mHead;
	protected int mGUID;
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
		BinaryUtility.readBytes(data, ref index, -1, mName, -1, -1);
		mMoney = BinaryUtility.readInt(data, ref index);
		mHead = BinaryUtility.readShort(data, ref index);
		mGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeByte(data, ref index, mLoginRet);
		BinaryUtility.writeBytes(data, ref index, -1, mName, -1, -1);
		BinaryUtility.writeInt(data, ref index, mMoney);
		BinaryUtility.writeShort(data, ref index, mHead);
		BinaryUtility.writeInt(data, ref index, mGUID);
	}
	public override int getSize()
	{
		return sizeof(byte) + mName.Length * sizeof(byte) + sizeof(int) + sizeof(short) + sizeof(int);
	}
	public override void execute()
	{
		// 创建玩家
		CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
		cmdCreate.mName = BinaryUtility.byteArrayToUTF8String(mName);
		mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
		// 设置角色数据
		CharacterMyself myself = cmdCreate.mResultCharacter as CharacterMyself;
		CharacterData data = myself.getCharacterData();
		data.mGUID = mGUID;
		data.mMoney = mMoney;
		data.mHead = mHead;

		// 进入到主场景
		CommandGameSceneManagerEnter cmdEnterMain = new CommandGameSceneManagerEnter(true, true);
		cmdEnterMain.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
		mCommandSystem.pushDelayCommand(cmdEnterMain, mGameSceneManager);
	}
}