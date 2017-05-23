using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScriptFactoryManager
{
	protected Dictionary<LAYOUT_TYPE, ScriptFactory> mFactoryList;
	public ScriptFactoryManager()
	{
		mFactoryList = new Dictionary<LAYOUT_TYPE, ScriptFactory>();
	}
	public ScriptFactory addFactory(Type classType, LAYOUT_TYPE type)
	{
		ScriptFactory factory = createFactory(classType, type);
		mFactoryList.Add(factory.getType(), factory);
		return factory;
	}
	public ScriptFactory getFactory(LAYOUT_TYPE type)
	{
		if (mFactoryList.ContainsKey(type))
		{
			return mFactoryList[type];
		}
		return null;
	}
	//------------------------------------------------------------------------------------------------------------
	protected ScriptFactory createFactory(Type classType, LAYOUT_TYPE type)
	{
		return UnityUtility.createInstance<ScriptFactory>(typeof(ScriptFactory), new object[] { type, classType });
	}
}