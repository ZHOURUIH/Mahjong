using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class GameComponent : GameBase
{
	protected ComponentOwner			mComponentOwner;		// 该组件的拥有者
	protected Type						mBaseType;				// 基础组件类型,指定该组件属于什么基础类型的组件,如果不属于任何基础类型,则填实际组件类型
	protected Type						mType;					// 实际组件类型
	protected string					mName;					// 组件名
	protected bool						mActive;				// 是否激活组件
	protected bool						mLockOneFrame;			// 是否将组件锁定一次
	protected bool						mPreUpdate;				// 是否为预更新组件
	protected GameComponent				mParent;				// 父级组件
	Dictionary<string, GameComponent>	mChildComponentMap;		// 用于查找组件
	List<GameComponent>					mChildComponentList; // 该组件下的子组件列表,保存了子组件之间的顺序
	public GameComponent(Type type, string name)
	{
		mType = type;
		mName = name;
		mComponentOwner = null;
		mActive = true;
		mLockOneFrame = false;
		mParent = null;
		mChildComponentMap = new Dictionary<string, GameComponent>();
		mChildComponentList = new List<GameComponent>();			
	}
	~GameComponent() { destroy(); }
	public virtual void init(ComponentOwner owner)
	{
		mComponentOwner = owner;
		// setBaseType不能在构造中调用,因为构造时对象还没有完全创建出,所以无法正确调用派生类的函数
		setBaseType();
	}
	public virtual void preUpdate(float elapsedTime)
	{
		if (mLockOneFrame || !isActive())
		{
			return;
		}

		// 预更新子组件
		int childCount = mChildComponentList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			mChildComponentList[i].preUpdate(elapsedTime);
		}
	}
	public virtual void update(float elapsedTime)
	{
		if (mLockOneFrame || !isActive())
		{
			return;
		}
		// 更新子组件
		int childCount = mChildComponentList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			mChildComponentList[i].update(elapsedTime);
		}
	}
	public virtual void lateUpdate(float elapsedTime)
	{
		if (mLockOneFrame)
		{
			mLockOneFrame = false;
			return;
		}
		if (!isActive())
		{
			return;
		}
		// 后更新子组件
		int childCount = mChildComponentList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			mChildComponentList[i].lateUpdate(elapsedTime);
		}
	}
	public virtual void destroy()
	{
		// 首先通知所有的子组件
		int childCount = mChildComponentList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			mChildComponentList[i].notifyParentDestroied();
		}
		mChildComponentList.Clear();
		mChildComponentMap.Clear();

		// 通知父组件
		if (mParent != null)
		{
			mParent.notifyChildDestroied(this);
			mParent = null;
		}

		if (mComponentOwner != null)
		{
			mComponentOwner.notifyComponentDestroied(this);
		}
	}
	public bool rename(string newName)
	{
		if (mName == newName)
		{
			return false;
		}
		string oldName = mName;
		mName = newName;
		if (mComponentOwner != null)
		{
			// 通知Layout自己的名字改变了
			bool ret = mComponentOwner.notifyComponentNameChanged(oldName, this);
			// 如果父窗口不允许自己改名,则需要将名字还原
			if (!ret)
			{
				mName = oldName;
				return false;
			}
		}
		// 通知父窗口自己的名字改变了
		if (null != mParent)
		{
			mParent.notifyChildNameChanged(oldName, this);
		}
		return true;
	}
	// 将一个组件添加作为当前组件的子组件
	public bool addChild(GameComponent component)
	{
		if (component == null || mChildComponentMap.ContainsValue(component))
		{
			return false;
		}
		component.setParentComponent(this);
		mChildComponentList.Add(component);
		mChildComponentMap.Add(component.getName(), component);
		return true;
	}
	// 将一个组件从当前组件的子组件列表中移除
	public bool removeChild(GameComponent component)
	{
		if (component == null)
		{
			return false;
		}
		if (mChildComponentMap.ContainsValue(component))
		{
			return false;
		}
		mChildComponentMap.Remove(component.getName());

		mChildComponentList.Remove(component);
		return true;
	}
	// 拷贝当前组件的所有属性到目标组件中,返回值表示当前组件是否已经链接了预设
	public bool isActive()
	{
		if (mParent != null)
		{
			return mParent.isActive() && mActive;
		}
		return mActive;
	}
	// 断开与拥有者和父组件的联系,使该组件成为一个独立的组件,该组件下的所有子组件也会断开与拥有者的联系,但是父子关系仍然存在
	// detachOwnerOnly表示是否只断开与拥有者的联系,不断开组件之间的父子关系,外部使用时一般填false
	public virtual void detachOwnerParentComponent(bool detachOwnerOnly = false)
	{
		if (mComponentOwner != null)
		{
			mComponentOwner.notifyComponentDetached(this);
			mComponentOwner = null;
		}
		// 如果不是只断开与组件拥有者的联系,则还需要断开与父组件的联系
		if (!detachOwnerOnly && mParent != null)
		{
			mParent.notifyChildDetached(this);
			setParentComponent(null);
		}
		// 使自己所有的子组件都断开与拥有者的联系,但是不能打断子组件的父子关系
		int childCount = mChildComponentList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			mChildComponentList[i].detachOwnerParentComponent(true);
		}
	}

	// 建立与拥有者和父组件的联系,使该组件成为拥有者中的一个组件,该组件下的所有子组件也会重建与拥有者的联系,父子关系仍然存在
	public virtual void attachOwnerParentComponent(ComponentOwner owner, GameComponent parent, int childPos)
	{
		// 先建立与父组件的联系,避免在建立与组件拥有者的联系时被当作第一级组件
		if (parent != null && mParent == null)
		{
			parent.addChild(this);
			parent.moveChildPos(this, childPos);
		}
		if (owner != null && mComponentOwner == null)
		{
			mComponentOwner = owner;
			mComponentOwner.notifyComponentAttached(this);
			// 使自己所有的子窗口都建立与布局的联系
			int childCount = mChildComponentList.Count;
			for (int i = 0; i < childCount; ++i)
			{
				mChildComponentList[i].attachOwnerParentComponent(owner, null, -1);
			}
		}
	}
	// 得到指定组件在当前组件中的位置
	public int getChildPos(GameComponent component)
	{
		if (component == null)
		{
			return -1;
		}
		// 首先查找当前窗口的位置
		int childCount = mChildComponentList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			if (component == mChildComponentList[i])
			{
				return i;
			}
		}
		return -1;
	}
	// 将指定组件移动到当前组件中的指定位置
	public bool moveChildPos(GameComponent component, int destPos)
	{
		if (component == null || destPos < 0 || destPos >= mChildComponentList.Count)
		{
			return false;
		}
		// 首先查找当前窗口的位置
		int pos = getChildPos(component);
		if (pos < 0 || pos == destPos)
		{
			return false;
		}
		mChildComponentList.RemoveAt(pos);
		mChildComponentList.Insert(destPos, component);
		return true;
	}
	public bool moveChildPos(string name, int destPos)
	{
		return moveChildPos(getChildComponent(name), destPos);
	}
	public virtual void setParentComponent(GameComponent component){mParent = component;}
	public GameComponent getChildComponent(string childName)
	{
		if (mChildComponentMap.ContainsKey(childName))
		{
			return mChildComponentMap[childName];
		}
		return null;
	}
	// 设置成员变量
	public virtual void setActive(bool active) { mActive = active; }
	public void setLockOneFrame(bool lockOneFrame) { mLockOneFrame = lockOneFrame; }
	public void setPreUpdate(bool pre) { mPreUpdate = pre; }
	// 获得成员变量
	public ComponentOwner getOwner() { return mComponentOwner; }
	public GameComponent getParentComponent() { return mParent; }
	public bool isPreUpdate() { return mPreUpdate; }
	public Type getType() { return mType; }
	public Type getBaseType() { return mBaseType; }
	public string getName() { return mName; }
	public bool isLockOneFrame() { return mLockOneFrame; }
	public bool isComponentActive() { return mActive; }		// 组件自己是否激活,不考虑父组件
	public List<GameComponent> getChildComponentList() { return mChildComponentList; }
	public Dictionary<string, GameComponent> getChildComponentMap() { return mChildComponentMap; }
	// 通知
	public virtual void notifyParentDestroied() { mParent = null; }
	public virtual void notifyChildDestroied(GameComponent component) { removeChild(component); }
	public virtual void notifyChildDetached(GameComponent component) { removeChild(component); }	// 通知该组件有子组件断开了联系
	public virtual void notifyChildNameChanged(string oldName, GameComponent component)
	{
		// 修改全部子窗口查找列表中的名字
		if (mChildComponentMap.ContainsKey(oldName))
		{
			mChildComponentMap.Remove(oldName);
			if (!mChildComponentMap.ContainsValue(component))
			{
				mChildComponentMap.Add(component.getName(), component);
			}
			else
			{
				UnityUtility.logError("error : there is a child named");
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------
	// 判断该组件是否是指定类型的,包括所有继承的类型
	protected virtual bool isType(Type type) { return false; }
	protected abstract void setBaseType();
}
