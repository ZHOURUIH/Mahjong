using UnityEngine;
using System.Collections;

public class CommandCharacterManagerCreateCharacter : Command
{
	public CHARACTER_TYPE	mCharacterType;
	public int				mID;
	public string			mName;
	public bool				mCreateNode;
	public override void init()
	{
		base.init();
		mCharacterType = CHARACTER_TYPE.CT_MAX;
		mName = "";
		mID = -1;
		mCreateNode = true;
	}
	public override void execute()
	{
		CharacterManager characterManager = mReceiver as CharacterManager;
		if(mID == -1)
		{
			mID = UnityUtility.makeID();
		}
		characterManager.createCharacter(mName, mCharacterType, mID, mCreateNode);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : name : " + mName + ", character type : " + mCharacterType + ", ID : " + mID;
	}
}
