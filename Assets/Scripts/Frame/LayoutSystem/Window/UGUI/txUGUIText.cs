using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
	public override int getDepth(){return mText.depth; }
	public void setText(string text) { mText.text = text; }
	public string getText() { return mText.text; }
}