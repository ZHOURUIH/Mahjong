using UnityEngine;
using System.Collections;

public class CommandLayoutManagerLayoutVisible : Command
{
	public LAYOUT_TYPE	mType;
	public bool			mForce;
	public bool			mImmediately;
	public bool			mVisibility;
	public string		mParam;
	public CommandLayoutManagerLayoutVisible(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
		mForce = false;
		mImmediately = false;
		mVisibility = true;
	}
	public override void execute()
	{
		GameLayoutManager layoutManager = mReceiver as GameLayoutManager;
		GameLayout layout = layoutManager.getGameLayout(mType);
		if (layout != null)
		{
			if (!mForce)
			{
				layout.setVisible(mVisibility, mImmediately, mParam);
			}
			else
			{
				layout.setVisibleForce(mVisibility);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : type : " + mType + ", visibility : " + mVisibility;
	}
}