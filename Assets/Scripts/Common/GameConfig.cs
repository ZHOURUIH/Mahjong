using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class FloatParameter
{
	public float mValue;
	public string mComment;
	public GAME_DEFINE_FLOAT mType;
	public string mTypeString;
};

public class StringParameter
{
	public string mValue;
	public string mComment;
	public GAME_DEFINE_STRING mType;
	public string mTypeString;
};
public class ConfigInfo
{
	public string mComment;
	public string mName;
	public string mValue;
}

public class GameConfig : GameBase
{
	protected static Dictionary<string, GAME_DEFINE_FLOAT> mFloatNameToDefine;
	protected static Dictionary<GAME_DEFINE_FLOAT, string> mFloatDefineToName;
	protected static Dictionary<string, GAME_DEFINE_STRING> mStringNameToDefine;
	protected static Dictionary<GAME_DEFINE_STRING, string> mStringDefineToName;
	protected Dictionary<GAME_DEFINE_FLOAT, FloatParameter> mFloatList;
	protected Dictionary<GAME_DEFINE_STRING, StringParameter> mStringList;
	public GameConfig() 
	{
		mFloatNameToDefine = new Dictionary<string, GAME_DEFINE_FLOAT>();
		mFloatDefineToName = new Dictionary<GAME_DEFINE_FLOAT, string>();
		mStringNameToDefine = new Dictionary<string, GAME_DEFINE_STRING>();
		mStringDefineToName = new Dictionary<GAME_DEFINE_STRING, string>();
		mFloatList = new Dictionary<GAME_DEFINE_FLOAT, FloatParameter>();
		mStringList = new Dictionary<GAME_DEFINE_STRING, StringParameter>();
	}
	public void init()
	{
		addFloatParam(GAME_DEFINE_FLOAT.GDF_HTTP_PORT);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_PORT);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_BROADCAST_PORT);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_LOAD_RESOURCES);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_SHOW_COMMAND_DEBUG_INFO);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_LOAD_ASYNC);
		if(mFloatNameToDefine.Count != (int)GAME_DEFINE_FLOAT.GDF_MAX)
		{
			UnityUtility.logError("not all float param added!");
		}
		readConfig(CommonDefine.F_CONFIG_PATH + "GameFloatConfig.txt", true);
		readConfig(CommonDefine.F_CONFIG_PATH + "GameStringConfig.txt", false);
	}
	public void readConfig(string fileName, bool floatParam)
	{
		string text = "";
		FileUtility.openTxtFile(fileName, ref text);

		string[] lineList = text.Split(new char[] { '\r', '\n' });
		Dictionary<string, ConfigInfo> valueList = new Dictionary<string, ConfigInfo>();
		string comment = "";
		// 前4行需要被丢弃
		int dropLine = 4;
		for (int i = 0; i < lineList.Length; ++i)
		{
			if (i < dropLine)
			{
				continue;
			}
			string line = lineList[i];
			// 去除所有空白字符
			line = Regex.Replace(line, @"\s", "");
			// 如果该行是空的,或者是注释,则不进行处理
			if (line.Length > 0)
			{
				if(line.Substring(0, 2) == "//")
				{
					comment = line.Substring(2, line.Length - 2);
				}
				else
				{
					string[] value = line.Split('=');
					ConfigInfo info = new ConfigInfo();
					info.mComment = comment;
					info.mName = value[0];
					info.mValue = value[1];
					valueList.Add(info.mName, info);
				}
			}
		}

		List<string> keys = new List<string>(valueList.Keys);
		List<ConfigInfo> values = new List<ConfigInfo>(valueList.Values);
		for (int i = 0; i < keys.Count; ++i)
		{
			if (floatParam)
			{
				GAME_DEFINE_FLOAT def = floatNameToType(keys[i]);
				if (def != GAME_DEFINE_FLOAT.GDF_MAX)
				{
					setFloatParam(def, StringUtility.stringToFloat(values[i].mValue), values[i].mComment);
				}
			}
			else
			{
				GAME_DEFINE_STRING def = stringNameToType(keys[i]);
				if (def != GAME_DEFINE_STRING.GDS_MAX)
				{
					setStringParam(def, values[i].mValue, values[i].mComment);
				}
			}
		}
	}
	public void destory()
	{
		;
	}
	public float getFloatParam(GAME_DEFINE_FLOAT param)
	{
		if (mFloatList.ContainsKey(param))
		{
			return mFloatList[param].mValue;
		}

		return 0.0f;
	}
	public string getStringParam(GAME_DEFINE_STRING param)
	{
		if (mStringList.ContainsKey(param))
		{
			return mStringList[param].mValue;
		}
		return "";
	}
	public void setFloatParam(GAME_DEFINE_FLOAT param, float value, string comment = "")
	{
		if (!mFloatList.ContainsKey(param))
		{
			FloatParameter floatParam = new FloatParameter();
			floatParam.mValue = value;
			floatParam.mComment = comment;
			floatParam.mType = param;
			floatParam.mTypeString = param.ToString();
			mFloatList.Add(param, floatParam);
		}
		else
		{
			mFloatList[param].mValue = value;
			if(comment != "")
			{
				mFloatList[param].mComment = comment;
			}
		}
	}
	public void setStringParam(GAME_DEFINE_STRING param, string value, string comment = "")
	{
		if (!mStringList.ContainsKey(param))
		{
			StringParameter strParam = new StringParameter();
			strParam.mValue = value;
			strParam.mComment = comment;
			strParam.mType = param;
			strParam.mTypeString = param.ToString();
			mStringList.Add(param, strParam);
		}
		else
		{
			mStringList[param].mValue = value;
			if (comment != "")
			{
				mStringList[param].mComment = comment;
			}
		}
	}
	public void writeConfig()
	{
		string preString = "// 注意\r\n// 每个参数上一行必须是该参数的注释\r\n// 可以添加任意的换行和空格\r\n// 变量命名应与代码中枚举命名相同\r\n\r\n";
		// 浮点数配置文件
		string nextLineStr = "\r\n\r\n";
		string fileString = preString;
		foreach(var info in mFloatList)
		{
			fileString += "// ";
			fileString += info.Value.mComment;
			fileString += "\r\n";
			fileString += info.Value.mTypeString;
			fileString += " = ";
			fileString += StringUtility.floatToString(info.Value.mValue, 2);
			fileString += nextLineStr;
		}
		// 移除最后的\r\n\r\n
		fileString = fileString.Substring(0, fileString.Length - nextLineStr.Length);
		FileUtility.writeFile(CommonDefine.F_CONFIG_PATH + "GameFloatConfig.txt", fileString);
	
		// 字符串配置文件
		fileString = preString;
		foreach(var info in mStringList)
		{
			fileString += "// ";
			fileString += info.Value.mComment;
			fileString += "\r\n";
			fileString += info.Value.mTypeString;
			fileString += " = ";
			fileString += info.Value.mValue;
			fileString += nextLineStr;
		}
		// 移除最后的\r\n\r\n
		fileString = fileString.Substring(0, fileString.Length - nextLineStr.Length);
		FileUtility.writeFile(CommonDefine.F_CONFIG_PATH + "GameStringConfig.txt", fileString);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------
	protected void addFloatParam(GAME_DEFINE_FLOAT type)
	{
		mFloatNameToDefine.Add(type.ToString(), type);
		mFloatDefineToName.Add(type, type.ToString());
	}
	protected void addStringParam(GAME_DEFINE_STRING type)
	{
		mStringNameToDefine.Add(type.ToString(), type);
		mStringDefineToName.Add(type, type.ToString());
	}
	static string floatTypeToName(GAME_DEFINE_FLOAT type)
	{
		if(mFloatDefineToName.ContainsKey(type))
		{
			return mFloatDefineToName[type];
		}
		return "";
	}
	static GAME_DEFINE_FLOAT floatNameToType(string name)
	{
		if(mFloatNameToDefine.ContainsKey(name))
		{
			return mFloatNameToDefine[name];
		}
		return GAME_DEFINE_FLOAT.GDF_MAX;
	}
	static string stringTypeToName(GAME_DEFINE_STRING type)
	{
		if(mStringDefineToName.ContainsKey(type))
		{
			return mStringDefineToName[type];
		}
		return "";
	}
	static GAME_DEFINE_STRING stringNameToType(string name)
	{
		if(mStringNameToDefine.ContainsKey(name))
		{
			return mStringNameToDefine[name];
		}
		return GAME_DEFINE_STRING.GDS_MAX;
	}
	bool hasParameter(GAME_DEFINE_FLOAT param)
	{
		return mFloatList.ContainsKey(param);
	}
	bool hasParameter(GAME_DEFINE_STRING param)
	{
		return mStringList.ContainsKey(param);
	}
}