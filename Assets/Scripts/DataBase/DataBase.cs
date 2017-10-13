using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DataBase : CommandReceiver
{
	protected Dictionary<DATA_TYPE, List<Data>> mDataStructList;
	protected Dictionary<string, DATA_TYPE> mDataFileDefine;
	protected Dictionary<DATA_TYPE, string> mDataDefineFile;
	protected Dictionary<DATA_TYPE, int> mDataSizeMap;
	protected DataFactoryManager mDataFactoryManager;
	public DataBase()
		:
		base(typeof(DataBase).ToString())
	{
		mDataStructList = new Dictionary<DATA_TYPE,List<Data>>();
		mDataFileDefine = new Dictionary<string, DATA_TYPE>();
		mDataDefineFile = new Dictionary<DATA_TYPE, string>();
		mDataSizeMap = new Dictionary<DATA_TYPE, int>();
		mDataFactoryManager = new DataFactoryManager();
	}
	// 初始化所有数据
	public virtual void init(bool loadAllData = true)
	{
		mDataFactoryManager.init();
		if (loadAllData)
		{
			loadAllDataFromFile();
		}
	}
	public override void destroy() 
	{
		base.destroy();
		mDataStructList.Clear();
		mDataFactoryManager = null;
	}
	public Data createData(DATA_TYPE type)
	{
		DataFactory factory = mDataFactoryManager.getFactory(type);
		if (factory != null)
		{
			return factory.createData();
		}
		return null;
	}
	public void loadAllDataFromFile()
	{
		// 读取配置文件，获得需要加载的所有数据列表
		// 遍历每一个文件名，加载相应的文件
		List<string> fileList = new List<string>();
		List<string> patterns = new List<string>();
		patterns.Add(CommonDefine.DATA_SUFFIX);
		FileUtility.findFiles(CommonDefine.A_GAME_DATA_FILE_PATH, ref fileList, patterns);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			loadData(fileList[i], true);
		}
	}
	public void destroyAllData()
	{
		mDataStructList.Clear();
	}
	public void destroyData(DATA_TYPE type)
	{
		if(mDataStructList.ContainsKey(type))
		{
			mDataStructList.Remove(type);
		}
	}
	public void loadData(string filePath, bool forceCover)
	{
		// 根据文件名查找工厂类型
		string fileName = StringUtility.getFileNameNoSuffix(filePath);
		DATA_TYPE type = getDataTypeByDataName(fileName);
		if (type == DATA_TYPE.DT_MAX)
		{
			UnityUtility.logError("error : can not find data file define, file name : " + fileName + ", filePath : " + filePath);
			return;
		}

		// 如果该数据已经存在,并且需要覆盖,则先删除数据
		if(mDataStructList.ContainsKey(type))
		{
			if (forceCover)
			{
				destroyData(type);
			}
			else
			{
				return;
			}
		}

		// 查找工厂
		DataFactory factory = mDataFactoryManager.getFactory(type);
		if(factory == null)
		{
			UnityUtility.logError("error : can not find factory, type : " + type + ", filename : " + fileName + ", filePath : " + filePath);
			return;
		}

		// 打开文件
		int fileSize = 0;
		byte[] fileBuffer = null;
		FileUtility.openFile(CommonDefine.F_GAME_DATA_FILE_PATH + filePath, ref fileBuffer, ref fileSize);

		// 解析文件
		List<Data> dataList = new List<Data>();
		int dataSize = getDataSize(type);
		byte[] dataBuffer = new byte[dataSize];
		int dataCount = fileSize / dataSize;
		for (int i = 0; i < dataCount; ++i)
		{
			Data newData = factory.createData();
			if(newData == null)
			{
				UnityUtility.logError("error : can not create data ,type : " + factory.getType());
				return;
			}
			BinaryUtility.memcpy(dataBuffer, fileBuffer, 0, i * dataSize, dataSize);
			newData.read(dataBuffer, dataSize);
			dataList.Add(newData);
		}
		mDataStructList.Add(factory.getType(), dataList);
	}
	// 得到数据数量
	public int getDataCount(DATA_TYPE type)
	{
		if (mDataStructList.ContainsKey(type))
		{
			return mDataStructList[type].Count;
		}
		return 0;
	}
	// 查询数据
	public Data queryData(DATA_TYPE type, int index)
	{
		if (mDataStructList.ContainsKey(type))
		{
			return mDataStructList[type][index];
		}
		return null;
	}
	public void addData(DATA_TYPE type, Data data, int pos = -1)
	{
		if (data == null)
		{
			return;
		}
		if(mDataStructList.ContainsKey(type))
		{
			if (pos == -1)
			{
				mDataStructList[type].Add(data);
			}
			else if (pos >= 0 && pos <= (int)mDataStructList[type].Count)
			{
				mDataStructList[type].Insert(pos, data);
			}
		}
		else
		{
			List<Data> datalist = new List<Data>();
			datalist.Add(data);
			mDataStructList.Add(type, datalist);
		}
	}
	public void registeData(Type data, DATA_TYPE type)
	{
		DataFactory factory = mDataFactoryManager.addFactory(data, type);
		Data temp = factory.createData();
		string dataName = data.ToString();
		mDataFileDefine.Add(dataName, type);
		mDataDefineFile.Add(type, dataName);
		mDataSizeMap.Add(type, temp.getDataSize());
	}
	// 根据数据名得到数据定义
	public string getDataNameByDataType(DATA_TYPE type)
	{
		if (mDataDefineFile.ContainsKey(type))
		{
			return mDataDefineFile[type];
		}
		return "";
	}
	// 根据数据定义得到数据名
	public DATA_TYPE getDataTypeByDataName(string name)
	{
		if (mDataFileDefine.ContainsKey(name))
		{
			return mDataFileDefine[name];
		}
		return DATA_TYPE.DT_MAX;
	}
	public int getDataSize(DATA_TYPE type)
	{
		if (mDataSizeMap.ContainsKey(type))
		{
			return mDataSizeMap[type];
		}
		return 0;
	}
};