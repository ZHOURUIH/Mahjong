using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SCLoginRet : SocketPacket
{
	public BYTE mLoginRet = new BYTE();  // 0表示登陆成功,1表示账号密码错误,2表示已经在其他地方登陆
	public BYTES mName = new BYTES(16);
	public INT mMoney = new INT();
	public SHORT mHead = new SHORT();
	public INT mPlayerGUID = new INT();
	public SCLoginRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mLoginRet);
		pushParam(mName);
		pushParam(mMoney);
		pushParam(mHead);
		pushParam(mPlayerGUID);
	}
	public override void execute()
	{
		if(mLoginRet.mValue == 0)
		{
			// 创建玩家
			CommandCharacterManagerCreateCharacter cmdCreate = mCommandSystem.newCmd<CommandCharacterManagerCreateCharacter>();
			cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
			cmdCreate.mName = BinaryUtility.bytesToString(mName.mValue, Encoding.UTF8);
			cmdCreate.mID = mPlayerGUID.mValue;
			mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
			// 设置角色数据
			CharacterMyself myself = mCharacterManager.getMyself();
			CharacterData data = myself.getCharacterData();
			data.mMoney = mMoney.mValue;
			data.mHead = mHead.mValue;

			// 进入到主场景
			CommandGameSceneManagerEnter cmdEnterMain = mCommandSystem.newCmd<CommandGameSceneManagerEnter>(true, true);
			cmdEnterMain.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			mCommandSystem.pushDelayCommand(cmdEnterMain, mGameSceneManager);
		}
		else if (mLoginRet.mValue == 1)
		{
			UnityUtility.logInfo("账号密码错误!");
		}
		else if (mLoginRet.mValue == 2)
		{
			UnityUtility.logInfo("已在其他地方登陆!");
		}
	}
}