using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerJoinRoom : SocketPacket
{
	protected int mPlayerGUID;
	protected byte[] mName = new byte[16];
	protected int mMoney;
	protected int mHead;
	protected int mPosition;
	protected bool mReady;
	protected bool mBanker;
	public SCOtherPlayerJoinRoom(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mPlayerGUID = BinaryUtility.readInt(data, ref index);
		BinaryUtility.readBytes(data, ref index, -1, mName, -1, -1);
		mMoney = BinaryUtility.readInt(data, ref index);
		mHead = BinaryUtility.readInt(data, ref index);
		mPosition = BinaryUtility.readInt(data, ref index);
		mReady = BinaryUtility.readBool(data, ref index);
		mBanker = BinaryUtility.readBool(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mPlayerGUID);
		BinaryUtility.writeBytes(data, ref index, -1, mName, -1, -1);
		BinaryUtility.writeInt(data, ref index, mMoney);
		BinaryUtility.writeInt(data, ref index, mHead);
		BinaryUtility.writeInt(data, ref index, mPosition);
		BinaryUtility.writeBool(data, ref index, mReady);
		BinaryUtility.writeBool(data, ref index, mBanker);
	}
	public override int getSize()
	{
		return sizeof(int) + sizeof(byte) * mName.Length + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool);
	}
	public override void execute()
	{
		string name = BinaryUtility.byteArrayToUTF8String(mName);
		UnityUtility.logInfo("获得玩家数据 : " + mPlayerGUID + ", 名字 : " + name);
		// 创建该玩家的实例
		CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
		cmdCreate.mName = name;
		cmdCreate.mGUID = mPlayerGUID;
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
		mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
		CharacterData data = cmdCreate.mResultCharacter.getCharacterData();
		data.mMoney = mMoney;
		data.mHead = mHead;
		data.mServerPosition = (PLAYER_POSITION)mPosition;
		data.mBanker = mBanker;
		data.mReady = mReady;
		// 将该玩家加入房间
		GameScene gameScene = mGameSceneManager.getCurScene();
		CommandRoomJoin cmd = new CommandRoomJoin(true, true);
		cmd.mCharacter = cmdCreate.mResultCharacter;
		mCommandSystem.pushDelayCommand(cmd, (gameScene as MahjongScene).getRoom());
	}
}