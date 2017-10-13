using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataFactoryManager
{
	protected Dictionary<DATA_TYPE, DataFactory>	mFactoryList;
	public DataFactoryManager()
	{
		mFactoryList = new Dictionary<DATA_TYPE, DataFactory>();
	}
	public void init()
	{
		;
	}
	public DataFactory addFactory(Type classType, DATA_TYPE type)
	{
		DataFactory factory = DataFactory.createFactory(classType, type);
		mFactoryList.Add(factory.getType(), factory);
		return factory;
	}
	public DataFactory getFactory(DATA_TYPE type)
	{
		if (mFactoryList.ContainsKey(type))
		{
			return mFactoryList[type];
		}
		return null;
	}
}