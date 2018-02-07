using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class txUIScrollView : txUIObject
{
	protected UIScrollView     mScrollView;
	protected txUIObject       mGrid;
	public    List<txUIObject> mItemList = new List<txUIObject>();
	public txUIScrollView()
	{
		mType = UI_OBJECT_TYPE.UBT_SCROLL_VIEW;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mScrollView = go.GetComponent<UIScrollView>();
		int childCount = mScrollView.transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			GameObject gridGo = mScrollView.transform.GetChild(i).gameObject;
			if(gridGo.GetComponent<UIGrid>() != null)
			{
				mLayout.getScript().newObject(ref mGrid, this, gridGo.name);
				break;
			}
		}
		if (mGrid == null)
		{
			UnityUtility.logError("error : scroll view window must have a child with grid compoent!");
		}
		// 查找grid下已经挂接的物体
		int itemCount = mGrid.getChildCount();
		for(int i = 0; i < itemCount; ++i)
		{
			GameObject child = mGrid.getChild(i);
			txUIObject item = null;
			mLayout.getScript().newObject(ref item, mGrid, child.name);
			mItemList.Add(item);
		}
	}
	public void addItem<T>(string name) where T : txUIObject, new()
	{
		T item = mLayout.getScript().createObject<T>(mGrid, name, true);
		mItemList.Add(item);
	}
	public void addItem(txUIObject obj)
	{
		mItemList.Add(obj);
	}
	public void removeItem(int index)
	{
		if(index < 0 || index >= mItemList.Count)
		{
			return;
		}
		UnityUtility.destroyGameObject(mItemList[index].mObject);
		mItemList.RemoveAt(index);
	}
	public void clearItem()
	{
		int itemCount = mItemList.Count;
		for (int i = 0; i < itemCount; ++i)
		{
			UnityUtility.destroyGameObject(mItemList[i].mObject);
		}
		mItemList.Clear();
	}
}