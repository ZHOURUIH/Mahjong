using UnityEngine;
using System.Collections;

public class CommandCharacterManagerDestroy : Command
{
	public int mGUID = 0;
	public string mName;
	public override void init()
	{
		base.init();
		mGUID = 0;
		mName = "";
	}
	public override void execute()
	{
		CharacterManager characterManager = (mReceiver) as CharacterManager;
		if(mGUID != 0)
		{
			characterManager.destroyCharacter(mGUID);
		}
		else if(mName != null && mName != "")
		{
			characterManager.destroyCharacter(mName);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : Name : " + mName + ", guid : " + mGUID;
	}
}