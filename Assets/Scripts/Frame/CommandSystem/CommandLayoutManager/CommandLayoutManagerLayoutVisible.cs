using UnityEngine;
using System.Collections;

public class CommandLayoutManagerLayoutVisible : Command
{
	public LAYOUT_TYPE	mLayoutType = LAYOUT_TYPE.LT_MAX;
	public bool			mForce = false;
	public bool			mImmediately = false;
	public bool			mVisibility = true;
	public string		mParam = "";
	public override void init()
	{
		base.init();
		mLayoutType = LAYOUT_TYPE.LT_MAX;
		mForce = false;
		mImmediately = false;
		mVisibility = true;
		mParam = "";
	}
	public override void execute()
	{
		GameLayoutManager layoutManager = mReceiver as GameLayoutManager;
		GameLayout layout = layoutManager.getGameLayout(mLayoutType);
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
		return this.GetType().ToString() + " : type : " + mLayoutType + ", visibility : " + mVisibility;
	}
}