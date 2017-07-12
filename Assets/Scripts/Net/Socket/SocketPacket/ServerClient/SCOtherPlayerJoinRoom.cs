using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SCOtherPlayerJoinRoom : SocketPacket
{
	public INT mPlayerGUID = new INT();
	public BYTES mName = new BYTES(16);
	public INT mMoney = new INT();
	public INT mHead = new INT();
	public INT mPosition = new INT();
	public BOOL mReady = new BOOL();
	public BOOL mBanker = new BOOL();
	public SCOtherPlayerJoinRoom(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
		pushParam(mName);
		pushParam(mMoney);
		pushParam(mHead);
		pushParam(mPosition);
		pushParam(mReady);
		pushParam(mBanker);
	}
	public override void execute()
	{
		string name = BinaryUtility.bytesToString(mName.mValue, Encoding.UTF8);
		UnityUtility.logInfo("获得玩家数据 : " + mPlayerGUID + ", 名字 : " + name);
		// 创建该玩家的实例
		CommandCharacterManagerCreateCharacter cmdCreate = mCommandSystem.newCmd<CommandCharacterManagerCreateCharacter>();
		cmdCreate.mName = name;
		cmdCreate.mGUID = mPlayerGUID.mValue;
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
		mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
		CharacterOther other = mCharacterManager.getCharacterByGUID(mPlayerGUID.mValue) as CharacterOther;
		CharacterData data = other.getCharacterData();
		data.mMoney = mMoney.mValue;
		data.mHead = mHead.mValue;
		data.mServerPosition = (PLAYER_POSITION)mPosition.mValue;
		data.mBanker = mBanker.mValue;
		data.mReady = mReady.mValue;
		// 将该玩家加入房间
		GameScene gameScene = mGameSceneManager.getCurScene();
		CommandRoomJoin cmd = mCommandSystem.newCmd<CommandRoomJoin>();
		cmd.mCharacter = other;
		mCommandSystem.pushCommand(cmd, (gameScene as MahjongScene).getRoom());
	}
}