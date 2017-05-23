using UnityEngine;
using System.Collections;

public class txUIText : txUIStaticSprite
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
	public void setText(string text)
	{
		mLabel.text = text;
	}
	public string getText()
	{
		return mLabel.text;
	}
}
