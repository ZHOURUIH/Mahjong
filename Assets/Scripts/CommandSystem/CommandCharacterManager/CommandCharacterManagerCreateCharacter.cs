using UnityEngine;
using System.Collections;

public class CommandCharacterManagerCreateCharacter : Command
{
	public CHARACTER_TYPE	mCharacterType;
	public string			mName;
	public int				mGUID;
	public override void init()
	{
		base.init();
		mCharacterType = CHARACTER_TYPE.CT_MAX;
		mName = null;
		mGUID = GameDefine.INVALID_ID;
	}
	public override void execute()
	{
		CharacterManager characterManager = mReceiver as CharacterManager;
		characterManager.createCharacter(mName, mCharacterType, mGUID);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : Name : " + mName + ", CharacterType : " + mCharacterType + ", GUID : " + mGUID;
	}
}
