using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClassObjectPool : FrameComponent
{
	protected Dictionary<Type, List<ClassObject>> mInusedList;
	protected Dictionary<Type, List<ClassObject>> mUnusedList;
	public ClassObjectPool(string name)
		: base(name)
	{
		mInusedList = new Dictionary<Type, List<ClassObject>>();
		mUnusedList = new Dictionary<Type, List<ClassObject>>();
	}
	public T newClass<T>() where T : ClassObject, new()
	{
		T obj = null;
		Type t = typeof(T);
		// 先从未使用的列表中查找是否有可用的对象
		if (mUnusedList.ContainsKey(t) && mUnusedList[t].Count > 0)
		{
			obj = mUnusedList[t][0] as T;
		}
		// 未使用列表中没有,创建一个新的
		else
		{
			obj = new T();
		}
		// 标记为已使用
		markUsed(obj);
		// 重置实例
		obj.resetProperty();
		return obj;
	}
	public void destroyClass(ClassObject classObject)
	{
		markUnused(classObject);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------
	protected void markUsed(ClassObject classObject)
	{
		// 加入使用列表
		Type t = classObject.GetType();
		if (mInusedList.ContainsKey(t))
		{
			if(mInusedList[t].Contains(classObject))
			{
				logError("ClassObject is in Inused list! can not add again!");
				return;
			}
		}
		else
		{
			mInusedList.Add(t, new List<ClassObject>());
		}
		mInusedList[t].Add(classObject);
		// 从未使用列表移除
		if(mUnusedList.ContainsKey(t))
		{
			if(mUnusedList[t].Contains(classObject))
			{
				mUnusedList[t].Remove(classObject);
			}
		}
	}
	protected void markUnused(ClassObject classObject)
	{
		// 加入未使用列表
		Type t = classObject.GetType();
		if (mUnusedList.ContainsKey(t))
		{
			if (mUnusedList[t].Contains(classObject))
			{
				logError("ClassObject is in Unused list! can not add again!");
				return;
			}
		}
		else
		{
			mUnusedList.Add(t, new List<ClassObject>());
		}
		mUnusedList[t].Add(classObject);
		// 从使用列表移除,要确保操作的都是从本类创建的实例
		if (mInusedList.ContainsKey(t))
		{
			if (mInusedList[t].Contains(classObject))
			{
				mInusedList[t].Remove(classObject);
			}
			else
			{
				logError("Inused List not contains class object!");
			}
		}
		else
		{
			logError("can not find class type in Inused List! type : " + t.ToString());
		}
	}
}