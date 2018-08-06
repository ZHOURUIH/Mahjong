using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class txUGUICanvas : txUIObject
{
	protected Canvas mCanvas;
	protected CanvasScaler mCanvasScaler;
	protected GraphicRaycaster mGraphicRaycaster;
	protected GameObject mConnectedParent;  // 重新指定挂接到的父节点
	protected RectTransform mRectTransform;
	public txUGUICanvas()
	{
		mType = UI_TYPE.UT_UGUI_CANVAS;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mCanvas = mObject.GetComponent<Canvas>();
		if (mCanvas == null)
		{
			mCanvas = mObject.AddComponent<Canvas>();
		}
		mRectTransform = mObject.GetComponent<RectTransform>();
		// 因为添加Canvas组件后,原来的Transform会被替换为RectTransform,所以需要重新设置Transform组件
		mTransform = mRectTransform;
	}
	public void setConnectParent(GameObject obj)
	{
		setConnectParent(obj, Vector3.zero, Vector3.zero);
	}
	public void setConnectParent(GameObject obj, Vector3 pos)
	{
		setConnectParent(obj, pos, Vector3.zero);
	}
	public void setConnectParent(GameObject obj, Vector3 pos, Vector3 rot)
	{
		mConnectedParent = obj;
		// 把窗口挂到该节点下,并且保留缩放属性
		mObject.transform.SetParent(mConnectedParent.transform);
		mObject.transform.localPosition = pos;
		mObject.transform.localEulerAngles = rot;
		UnityUtility.setGameObjectLayer(this, mConnectedParent.layer);
	}
	public override void setDepth(int depth){mCanvas.sortingOrder = depth;}
	public override int getDepth(){return mCanvas.sortingOrder;}
	public GameObject getConnectParent() { return mConnectedParent; }
}