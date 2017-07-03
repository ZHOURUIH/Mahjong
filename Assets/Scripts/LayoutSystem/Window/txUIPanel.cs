using UnityEngine;
using System.Collections;

public class txUIPanel : txUIObject
{
	protected UIPanel mPanel;
	public txUIPanel()
	{
		mType = UI_OBJECT_TYPE.UBT_PANEL;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
		mPanel = mObject.GetComponent<UIPanel>();
		if (mPanel == null)
		{
			mPanel = mObject.AddComponent<UIPanel>();
		}
	}
	public void setDepth(int depth)
	{
		mPanel.depth = depth;
	}
	public int getDepth()
	{
		return mPanel.depth;
	}
}