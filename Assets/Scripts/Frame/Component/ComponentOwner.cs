using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public abstract class ComponentOwner : CommandReceiver
{
	protected List<GameComponent>									mRootComponentList;				// 一级组件列表,保存着组件之间的更新顺序
	protected Dictionary<string, GameComponent>						mAllComponentList;				// 组件拥有者当前的所有组件列表
	protected Dictionary<Type, Dictionary<string, GameComponent>>	mAllComponentTypeList;		    // 组件类型列表,first是组件的类型名
	protected Dictionary<Type, Dictionary<string, GameComponent>>	mAllComponentBaseTypeList;      // 根据组件的基础类型分组的组件列表,first是基础组件类型名
	protected bool mUsePreUpdate = false;
	protected bool mUseLateUpdate = false;
	public ComponentOwner(string name)
		:
		base(name)
	{
		mRootComponentList = new List<GameComponent>();
		mAllComponentList = new Dictionary<string, GameComponent>();
		mAllComponentTypeList = new Dictionary<Type, Dictionary<string, GameComponent>>();
		mAllComponentBaseTypeList = new Dictionary<Type, Dictionary<string, GameComponent>>();
	}
	public abstract void initComponents();
	public virtual void preUpdateComponents(float elapsedTime)
	{
		if(!mUsePreUpdate)
		{
			return;
		}
		if (mAllComponentList.Count == 0)
		{
			return;
		}
		// 预更新基础类型组件
		int rootComponentCount = mRootComponentList.Count;
		for (int i = 0; i < rootComponentCount; ++i)
		{
			GameComponent component = mRootComponentList[i];
			if (component != null && component.isActive() && !component.isLockOneFrame())
			{
				component.preUpdate(elapsedTime);
			}
		}
	}
	// 更新正常更新的组件
	public virtual void updateComponents(float elapsedTime)
	{
		if (mAllComponentList.Count == 0)
		{
			return;
		}
		// 更新基础类型组件
		int rootComponentCount = mRootComponentList.Count;
		for (int i = 0; i < rootComponentCount; ++i)
		{
			GameComponent component = mRootComponentList[i];
			if (component != null && component.isActive() && !component.isLockOneFrame())
			{
				component.update(elapsedTime);
			}
		}
	}
	public virtual void lateUpdateComponents(float elapsedTime)
	{
		if(!mUseLateUpdate)
		{
			return;
		}
		if (mAllComponentList.Count == 0)
		{
			return;
		}
		// 补充更新基础类型组件
		int rootComponentCount = mRootComponentList.Count;
		for (int i = 0; i < rootComponentCount; ++i)
		{
			GameComponent component = mRootComponentList[i];
			if (component != null && component.isActive())
			{
				// 如果组件被锁定了一帧,则不更新,解除锁定
				if (component.isLockOneFrame())
				{
					component.setLockOneFrame(false);
				}
				else
				{
					component.lateUpdate(elapsedTime);
				}
			}
		}
	}
	// 物理更新
	public virtual void fixedUpdate(float elapsedTime)
	{
		if (mAllComponentList.Count == 0)
		{
			return;
		}
		int rootComponentCount = mRootComponentList.Count;
		for (int i = 0; i < rootComponentCount; ++i)
		{
			GameComponent component = mRootComponentList[i];
			if (component != null && component.isActive() && !component.isLockOneFrame())
			{
				component.fixedUpdate(elapsedTime);
			}
		}
	}
	public virtual void notifyAddComponent(GameComponent component) { }
	public virtual void notifyComponentDetached(GameComponent component) { removeComponentFromList(component); }
	public virtual void notifyComponentAttached(GameComponent component)
	{
		if (null == component)
		{
			return;
		}
		if (!mAllComponentList.ContainsKey(component.getName()))
		{
			addComponentToList(component);
		}
	}
	public virtual bool notifyComponentNameChanged(string oldName, GameComponent component)
	{
		// 先查找是否有该名字的组件
		if (!mAllComponentList.ContainsKey(oldName))
		{
			return false;
		}
		// 再查找改名后会不会重名
		if (mAllComponentList.ContainsKey(component.getName()))
		{
			return false;
		}
		removeComponentFromList(component);
		addComponentToList(component);
		return true;
	}
	public static GameComponent createIndependentComponent(string name, Type type, bool initComponent = true)
	{
		GameComponent component = UnityUtility.createInstance<GameComponent>(type, type, name);
		// 创建组件并且设置拥有者,然后初始化
		if (initComponent && component != null)
		{
			component.init(null);
		}
		return component;
	}
	public T addComponent<T>(string name, bool active = false) where T : GameComponent
	{
		// 不能创建重名的组件
		if (mAllComponentList.ContainsKey(name))
		{
			logError("there is component named " + name);
			return null;
		}
		GameComponent component = createIndependentComponent(name, typeof(T), false);
		if (component == null)
		{
			return null;
		}
		component.init(this);
		// 将组件加入列表
		addComponentToList(component);
		// 通知创建了组件
		notifyAddComponent(component);
		component.setActive(active);
		return component as T;
	}
	public static void destroyComponent(GameComponent component)
	{
		// 后序遍历销毁组件,从最底层组件开始销毁,此处不能用引用获得子组件列表,因为在销毁组件过程中会对列表进行修改
		List<GameComponent> children = component.getChildComponentList();
		int childCount = children.Count;
		for (int i = 0; i < childCount; ++i)
		{
			destroyComponent(children[i]);
		}
		component.destroy();
	}
	public virtual void destroyComponent(string name)
	{
		// 在总列表中查找}
		if (mAllComponentList.ContainsKey(name))
		{
			destroyComponent(mAllComponentList[name]);
		}
	}
	public void destroyAllComponents()
	{
		List<string> keys = new List<string>(mAllComponentList.Keys);
		for (int i = 0; i < keys.Count; ++i)
		{
			mAllComponentList[keys[i]].destroy();
		}

		mAllComponentList.Clear();
		mAllComponentTypeList.Clear();
	}
	// 得到组件在父组件(有父组件)或者组件拥有者(第一级组件)中的位置
	public int getComponentPosition(GameComponent component)
	{
		if (component == null)
		{
			return -1;
		}
		if (component.getParentComponent() != null)
		{
			return component.getParentComponent().getChildPos(component);
		}
		else
		{
			// 首先查找当前窗口的位置
			int childCount = mRootComponentList.Count;
			for (int i = 0; i < childCount; ++i)
			{
				if (component == mRootComponentList[i])
				{
					return i;
				}
			}
		}
		return -1;
	}
	public GameComponent getComponent(string name)
	{
		// 在总列表中查找}
		if (mAllComponentList.ContainsKey(name))
		{
			return mAllComponentList[name];
		}
		return null;
	}
	public Dictionary<string, GameComponent> getComponentsByBaseType(Type type)
	{
		if (mAllComponentBaseTypeList.ContainsKey(type))
		{
			return mAllComponentBaseTypeList[type];
		}
		return new Dictionary<string, GameComponent>();
	}
	public T getFirstActiveComponentByBaseType<T>() where T : GameComponent
	{
		Type type = typeof(T);
		if (mAllComponentBaseTypeList.ContainsKey(type))
		{
			Dictionary<string, GameComponent> comList = mAllComponentBaseTypeList[type];
			foreach (KeyValuePair<string, GameComponent> item in comList)
			{
				if (item.Value.isActive() && !item.Value.isLockOneFrame())
				{
					return item.Value as T;
				}
			}
		}
		return null;
	}
	public T getFirstComponentByBaseType<T>() where T : GameComponent
	{
		Type type = typeof(T);
		if (mAllComponentBaseTypeList.ContainsKey(type))
		{
			Dictionary<string, GameComponent> comList = mAllComponentBaseTypeList[type];
			foreach (KeyValuePair<string, GameComponent> item in comList)
			{
				return item.Value as T;
			}
		}
		return null;
	}
	public T getFirstActiveComponent<T>() where T : GameComponent
	{
		Type type = typeof(T);
		if (mAllComponentTypeList.ContainsKey(type))
		{
			Dictionary<string, GameComponent> comList = mAllComponentTypeList[type];
			foreach (KeyValuePair<string, GameComponent> item in comList)
			{
				if (item.Value.isActive() && !item.Value.isLockOneFrame())
				{
					return item.Value as T;
				}
			}
		}
		return null;
	}
	public T getFirstComponent<T>() where T : GameComponent
	{
		Type type = typeof(T);
		if (mAllComponentTypeList.ContainsKey(type))
		{
			Dictionary<string, GameComponent> comList = mAllComponentTypeList[type];
			foreach (KeyValuePair<string, GameComponent> item in comList)
			{
				return item.Value as T;
			}
		}
		return null;
	}
	public void activeFirstComponent<T>(bool active = true) where T : GameComponent
	{
		T component = getFirstComponent<T>();
		if(component != null)
		{
			component.setActive(active);
		}
	}
	public virtual void notifyComponentDestroied(GameComponent component)
	{
		removeComponentFromList(component);
	}
	public Dictionary<string, GameComponent> getAllComponent() { return mAllComponentList; }
	public Dictionary<Type, Dictionary<string, GameComponent>> getComponentTypeList() { return mAllComponentTypeList; }
	public List<GameComponent> getRootComponentList() { return mRootComponentList; }
	protected void addComponentToList(GameComponent component, int componentPos = -1)
	{
		string name = component.getName();
		Type type = component.getType();
		Type baseType = component.getBaseType();

		// 如果没有父组件,则将组件放入第一级组件列表中
		if (component.getParentComponent() == null)
		{
			if (componentPos == -1)
			{
				mRootComponentList.Add(component);
			}
			else
			{
				mRootComponentList.Insert(componentPos, component);
			}
		}

		// 添加到组件列表中
		mAllComponentList.Add(name, component);

		// 添加到组件类型分组列表中
		if (mAllComponentTypeList.ContainsKey(type))
		{
			mAllComponentTypeList[type].Add(name, component);
		}
		else
		{
			Dictionary<string, GameComponent> componentList = new Dictionary<string, GameComponent>();
			componentList.Add(name, component);
			mAllComponentTypeList.Add(type, componentList);
		}

		// 添加到基础组件类型分组列表中
		if (mAllComponentBaseTypeList.ContainsKey(baseType))
		{
			mAllComponentBaseTypeList[baseType].Add(name, component);
		}
		else
		{
			Dictionary<string, GameComponent> componentList = new Dictionary<string, GameComponent>();
			componentList.Add(name, component);
			mAllComponentBaseTypeList.Add(baseType, componentList);
		}
	}
	protected void removeComponentFromList(GameComponent component)
	{
		// 从第一级组件列表中移除
		if (component.getParentComponent() == null)
		{
			mRootComponentList.Remove(component);
		}

		// 从所有组件列表中移除
		string componentName = component.getName();
		if (mAllComponentList.ContainsKey(componentName))
		{
			mAllComponentList.Remove(componentName);
		}

		// 从组件类型分组列表中移除
		Type realType = component.getType();
		if (mAllComponentTypeList.ContainsKey(realType))
		{
			if (mAllComponentTypeList[realType].ContainsKey(componentName))
			{
				mAllComponentTypeList[realType].Remove(componentName);
			}
		}

		// 从基础组件类型分组列表中移除
		Type baseType = component.getBaseType();
		if (mAllComponentBaseTypeList.ContainsKey(baseType))
		{
			if (mAllComponentBaseTypeList[baseType].ContainsKey(componentName))
			{
				mAllComponentBaseTypeList[baseType].Remove(componentName);
			}
		}
	}
}