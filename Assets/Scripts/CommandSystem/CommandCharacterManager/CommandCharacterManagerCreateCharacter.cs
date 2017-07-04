using UnityEngine;
using System.Collections;

public class CommandCharacterManagerCreateCharacter : Command
{
	public CHARACTER_TYPE	mCharacterType;
	public string			mName;
	public Character		mResultCharacter;
	public int				mGUID;
	public override void init()
	{
		base.init();
		mCharacterType = CHARACTER_TYPE.CT_MAX;
		mName = null;
		mResultCharacter = null;
		mGUID = CommonDefine.INVALID_ID;
	}
	public override void execute()
	{
		CharacterManager characterManager = mReceiver as CharacterManager;
		mResultCharacter = characterManager.createCharacter(mName, mCharacterType, GameUtility.makeID(), mGUID);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : Name : " + mName + ", CharacterType : " + mCharacterType + ", GUID : " + mGUID;
	}
}
