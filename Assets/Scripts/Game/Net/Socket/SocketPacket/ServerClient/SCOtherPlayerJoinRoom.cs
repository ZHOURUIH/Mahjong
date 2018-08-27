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
		: base(type) { }
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
		string name = BinaryUtility.bytesToString(mName.mValue, BinaryUtility.getGB2312());
		UnityUtility.logInfo("获得玩家数据 : " + mPlayerGUID + ", 名字 : " + name);
		// 创建该玩家的实例
		CommandCharacterManagerCreateCharacter cmdCreate = newCmd(out cmdCreate);
		cmdCreate.mName = name;
		cmdCreate.mID = mPlayerGUID.mValue;
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
		pushCommand(cmdCreate, mCharacterManager);
		CharacterOther other = mCharacterManager.getCharacter(mPlayerGUID.mValue) as CharacterOther;
		CharacterData data = other.getCharacterData();
		data.mMoney = mMoney.mValue;
		data.mHead = mHead.mValue;
		data.mServerPosition = (PLAYER_POSITION)mPosition.mValue;
		data.mBanker = mBanker.mValue;
		data.mReady = mReady.mValue;
		// 将该玩家加入房间
		GameScene gameScene = mGameSceneManager.getCurScene();
		CommandRoomJoin cmd = newCmd(out cmd);
		cmd.mCharacter = other;
		pushCommand(cmd, (gameScene as MahjongScene).getRoom());
	}
}