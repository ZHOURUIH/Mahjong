using UnityEngine;
using System.Collections;

public class CommandCharacterManagerDestroy : Command
{
	public int mGUID = CommonDefine.INVALID_ID;
	public int mClientID = CommonDefine.INVALID_ID;
	public string mName;
	public override void init()
	{
		base.init();
		mGUID = CommonDefine.INVALID_ID;
		mClientID = CommonDefine.INVALID_ID;
		mName = "";
	}
	public override void execute()
	{
		CharacterManager characterManager = (mReceiver) as CharacterManager;
		if(mGUID != CommonDefine.INVALID_ID)
		{
			characterManager.destroyCharacterByGUID(mGUID);
		}
		else if(mClientID != CommonDefine.INVALID_ID)
		{
			characterManager.destroyCharacterByClientID(mClientID);
		}
		else if(mName != null && mName != "")
		{
			characterManager.destroyCharacter(mName);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : Name : " + mName + ", guid : " + mGUID + ", client id : %d" + mClientID;
	}
}