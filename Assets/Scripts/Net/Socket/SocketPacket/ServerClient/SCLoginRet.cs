using System;
using System.Collections;
using System.Collections.Generic;

public class SCLoginRet : SocketPacket
{
	protected byte mLoginRet;  // 0表示登陆成功,1表示账号密码错误,2表示已经在其他地方登陆
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
		if(mLoginRet == 0)
		{
			// 创建玩家
			CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
			cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
			cmdCreate.mName = BinaryUtility.byteArrayToUTF8String(mName);
			cmdCreate.mGUID = mGUID;
			mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
			// 设置角色数据
			CharacterMyself myself = cmdCreate.mResultCharacter as CharacterMyself;
			CharacterData data = myself.getCharacterData();
			data.mMoney = mMoney;
			data.mHead = mHead;

			// 进入到主场景
			CommandGameSceneManagerEnter cmdEnterMain = new CommandGameSceneManagerEnter(true, true);
			cmdEnterMain.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			mCommandSystem.pushDelayCommand(cmdEnterMain, mGameSceneManager);
		}
		else if(mLoginRet == 1)
		{
			UnityUtility.logInfo("账号密码错误!");
		}
		else if (mLoginRet == 2)
		{
			UnityUtility.logInfo("已在其他地方登陆!");
		}
	}
}