using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowPool
{
	protected Dictionary<Type, List<txUIObject>> mInusedList;
	protected Dictionary<Type, List<txUIObject>> mUnusedList;
	protected LayoutScript mScript;
	protected txUIObject mPoolNode;
	public WindowPool(LayoutScript script)
	{
		mScript = script;
		mInusedList = new Dictionary<Type, List<txUIObject>>();
		mUnusedList = new Dictionary<Type, List<txUIObject>>();
	}
	public void assignWindow()
	{
		mPoolNode = mScript.createObject<txUIObject>("WindowPool");
	}
	public void destroy()
	{
		mScript.destroyObject(mPoolNode, true);
		mPoolNode = null;
	}
	public T createWindow<T>(string name, txUIObject parent) where T : txUIObject, new()
	{
		txUIObject window = null;
		Type type = typeof(T);
		// 从未使用列表中获取
		if (mUnusedList.ContainsKey(type) && mUnusedList[type].Count > 0)
		{
			window = mUnusedList[type][mUnusedList[type].Count - 1];
			mUnusedList[type].RemoveAt(mUnusedList[type].Count - 1);
		}
		// 未使用列表中没有就创建新窗口
		if (window == null)
		{
			window = mScript.createObject<T>(name);
		}
		// 加入到已使用列表中
		if (!mInusedList.ContainsKey(type))
		{
			mInusedList.Add(type, new List<txUIObject>());
		}
		mInusedList[type].Add(window);
		window.setActive(true);
		window.setName(name);
		window.setParent(parent);
		return window as T;
	}
	public void destroyWindow<T>(T window) where T : txUIObject
	{
		if (window == null)
		{
			return;
		}
		Type type = typeof(T);
		if (!mUnusedList.ContainsKey(type))
		{
			mUnusedList.Add(type, new List<txUIObject>());
		}
		mUnusedList[type].Add(window);
		mInusedList[type].Remove(window);
		window.setActive(false);
		window.setParent(mPoolNode);
	}
}