using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataFactoryManager
{
	protected Dictionary<DATA_TYPE, DataFactory>	mFactoryList;
	protected Dictionary<string, DATA_TYPE>			mDataFileDefine;
	protected Dictionary<DATA_TYPE, string>			mDataDefineFile;
	protected Dictionary<DATA_TYPE, int>			mDataSizeMap;
	public DataFactoryManager()
	{
		mFactoryList = new Dictionary<DATA_TYPE, DataFactory>();
		mDataFileDefine = new Dictionary<string,DATA_TYPE>();
		mDataDefineFile = new Dictionary<DATA_TYPE,string>();
		mDataSizeMap = new Dictionary<DATA_TYPE,int>();
	}
	public void init()
	{
		addFactory(typeof(DataGameSound), DATA_TYPE.DT_GAME_SOUND);
	}
	public DataFactory addFactory(Type classType, DATA_TYPE type)
	{
		DataFactory factory = DataFactory.createFactory(classType, type);
		mFactoryList.Add(factory.getType(), factory);
		Data data = factory.createData();
		string dataName = classType.ToString();
		mDataFileDefine.Add(dataName, type);
		mDataDefineFile.Add(type, dataName);
		mDataSizeMap.Add(type, data.getDataSize());
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
	// 根据数据名得到数据定义
	public string getDataNameByDataType(DATA_TYPE type)
	{
		if(mDataDefineFile.ContainsKey(type))
		{
			return mDataDefineFile[type];
		}
		return "";
	}

	// 根据数据定义得到数据名
	public DATA_TYPE getDataTypeByDataName(string name)
	{
		if(mDataFileDefine.ContainsKey(name))
		{
			return mDataFileDefine[name];
		}
		return DATA_TYPE.DT_MAX;
	}
	public int getDataSize(DATA_TYPE type)
	{
		if(mDataSizeMap.ContainsKey(type))
		{
			return mDataSizeMap[type];
		}
		return 0;
	}
}