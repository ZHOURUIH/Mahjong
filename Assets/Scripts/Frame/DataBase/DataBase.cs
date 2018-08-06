using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DataBase : FrameComponent
{
	protected Dictionary<DATA_TYPE, Type> mDataRegisteList;
	protected Dictionary<DATA_TYPE, List<Data>> mDataStructList;
	protected Dictionary<string, DATA_TYPE> mDataFileDefine;
	protected Dictionary<DATA_TYPE, string> mDataDefineFile;
	protected Dictionary<DATA_TYPE, int> mDataSizeMap;
	public DataBase(string name)
		:base(name)
	{
		mDataRegisteList = new Dictionary<DATA_TYPE, Type>();
		mDataStructList = new Dictionary<DATA_TYPE,List<Data>>();
		mDataFileDefine = new Dictionary<string, DATA_TYPE>();
		mDataDefineFile = new Dictionary<DATA_TYPE, string>();
		mDataSizeMap = new Dictionary<DATA_TYPE, int>();
	}
	// 初始化所有数据
	public override void init()
	{
		loadAllDataFromFile();
	}
	public override void destroy() 
	{
		base.destroy();
		mDataStructList.Clear();
	}
	public Data createData(DATA_TYPE type)
	{
		return UnityUtility.createInstance<Data>(mDataRegisteList[type], type);
	}
	public void loadAllDataFromFile()
	{
		foreach (var item in mDataFileDefine)
		{
			string filePath = CommonDefine.F_GAME_DATA_FILE_PATH + item.Key + CommonDefine.DATA_SUFFIX;
			byte[] file = null;
			FileUtility.openFile(filePath, ref file);
			if (file != null && file.Length != 0)
			{
				parseFile(file, item.Value);
			}
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
	public List<Data> getAllData(DATA_TYPE type)
	{
		if(mDataStructList.ContainsKey(type))
		{
			return mDataStructList[type];
		}
		return null;
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
	public void registeData(Type classType, DATA_TYPE type)
	{
		mDataRegisteList.Add(type, classType);
		Data temp = createData(type);
		string dataName = classType.ToString();
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
	protected void parseFile(byte[] file, DATA_TYPE type)
	{
		// 解析文件
		List<Data> dataList = new List<Data>();
		int dataSize = getDataSize(type);
		byte[] dataBuffer = new byte[dataSize];
		int fileSize = file.Length;
		int dataCount = fileSize / dataSize;
		for (int i = 0; i < dataCount; ++i)
		{
			Data newData = createData(type);
			if (newData == null)
			{
				UnityUtility.logError("error : can not create data ,type : " + type);
				return;
			}
			BinaryUtility.memcpy(dataBuffer, file, 0, i * dataSize, dataSize);
			newData.read(dataBuffer, dataSize);
			dataList.Add(newData);
		}
		mDataStructList.Add(type, dataList);
	}
};