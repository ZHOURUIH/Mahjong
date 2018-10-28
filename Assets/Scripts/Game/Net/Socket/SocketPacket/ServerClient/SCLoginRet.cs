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
		: base(type) { }
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
			CommandCharacterManagerCreateCharacter cmdCreate = newCmd(out cmdCreate);
			cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
			cmdCreate.mName = bytesToString(mName.mValue, getGB2312());
			cmdCreate.mID = mPlayerGUID.mValue;
			pushCommand(cmdCreate, mCharacterManager);
			// 设置角色数据
			CharacterMyself myself = mCharacterManager.getMyself();
			CharacterData data = myself.getCharacterData();
			data.mMoney = mMoney.mValue;
			data.mHead = mHead.mValue;

			// 进入到主场景
			CommandGameSceneManagerEnter cmdEnterMain = newCmd(out cmdEnterMain, true, true);
			cmdEnterMain.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			pushDelayCommand(cmdEnterMain, mGameSceneManager);
		}
		else if (mLoginRet.mValue == 1)
		{
			string info = "账号密码错误!";
			GameUtility.messageOK(info);
			UnityUtility.logInfo(info);
		}
		else if (mLoginRet.mValue == 2)
		{
			string info = "已在其他地方登陆!";
			GameUtility.messageOK(info);
			UnityUtility.logInfo(info);
		}
	}
}