using UnityEngine;
using System.Collections;

public class CommandCharacterManagerCreateCharacter : Command
{
	public CHARACTER_TYPE	mCharacterType;
	public int				mID;
	public string			mName;
	public string			mModelPath;
	public string			mAnimatorControllerPath;
	public string			mCharacterNode;
	public override void init()
	{
		base.init();
		mCharacterType = CHARACTER_TYPE.CT_MAX;
		mName = "";
		mID = -1;
		mModelPath = "";
		mAnimatorControllerPath = "";
		mCharacterNode = "";
	}
	public override void execute()
	{
		CharacterManager characterManager = mReceiver as CharacterManager;
		Character character = characterManager.createCharacter(mName, mCharacterType, mID);
		if(mCharacterNode != "")
		{
			GameObject charNode = UnityUtility.getGameObject(mCharacterManager.getManagerNode(), mCharacterNode);
			character.setObject(charNode);
		}
		character.initModel(mModelPath, mAnimatorControllerPath);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : name : " + mName + ", character type : " + mCharacterType + ", ID : " + mID;
	}
}
