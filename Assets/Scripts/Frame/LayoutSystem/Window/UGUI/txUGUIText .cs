using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class txUGUIText : txUIObject
{
    protected Text mText;
    public txUGUIText()
	{
		mType = UI_TYPE.UT_UGUI_TEXT;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
        mText = mObject.GetComponent<Text>();
        if (mText == null)
		{
            mText = mObject.AddComponent<Text>();
		}
	}
    public string getText()
	{
        return mText.text;
	}
    public void setText(string label)
	{
        mText.text = label;
	}
	public override int getDepth()
	{
        return mText.depth;
	}
}