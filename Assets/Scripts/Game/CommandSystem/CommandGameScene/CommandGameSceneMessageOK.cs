using UnityEngine;
using System.Collections;

public class CommandGameSceneMessageOK : Command
{
	public string mMessage;
	public string mButtonLabel;
	public override void init()
	{
		base.init();
		mMessage = "";
		mButtonLabel = "";
	}
	public override void execute()
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MESSAGE_OK);
		mScriptMessageOK.setMessage(mMessage);
		if(mButtonLabel != "")
		{
			mScriptMessageOK.setButtonLabel(mButtonLabel);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : mMessage : " + mMessage + ", mButtonLabel" + mButtonLabel;
	}
}