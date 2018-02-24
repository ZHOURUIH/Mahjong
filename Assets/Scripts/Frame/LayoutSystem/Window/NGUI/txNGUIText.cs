using UnityEngine;
using System.Collections;

public class txNGUIText : txUIObject
{	
	protected UILabel mLabel;
	public txNGUIText()
	{
		mType = UI_TYPE.UT_NGUI_TEXT;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mLabel = mObject.GetComponent<UILabel>();
		if (mLabel == null)
		{
			mLabel = mObject.AddComponent<UILabel>();
		}
	}
	public string getLabel()
	{
		return mLabel.text;
	}
	public void setLabel(string label)
	{
		mLabel.text = label;
	}
	public override void setAlpha(float alpha)
	{
		mLabel.alpha = alpha;
	}
	public override void setDepth(int depth)
	{
		mLabel.depth = depth;
		base.setDepth(depth);
	}
	public override int getDepth()
	{
		return mLabel.depth;
	}
}