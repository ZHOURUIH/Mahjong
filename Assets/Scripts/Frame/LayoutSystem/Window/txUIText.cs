using UnityEngine;
using System.Collections;

public class txUIText : txUIObject
{	
	protected UILabel mLabel;
	public txUIText()
	{
		mType = UI_OBJECT_TYPE.UBT_TEXT;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
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
}