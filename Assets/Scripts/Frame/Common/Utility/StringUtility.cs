using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class StringUtility : BinaryUtility
{
	public static bool startWith(string oriString, string pattern, bool sensitive = true)
	{
		if (oriString.Length < pattern.Length)
		{
			return false;
		}
		string startString = oriString.Substring(0, pattern.Length);
		if (sensitive)
		{
			return startString == pattern;
		}
		else
		{
			return startString.ToLower() == pattern.ToLower();
		}
	}
	public static bool endWith(string oriString, string pattern, bool sensitive = true)
	{
		if (oriString.Length < pattern.Length)
		{
			return false;
		}
		string endString = oriString.Substring(oriString.Length - pattern.Length, pattern.Length);
		if (sensitive)
		{
			return endString == pattern;
		}
		else
		{
			return endString.ToLower() == pattern.ToLower();
		}
	}
	public static int getLastNotNumberPos(string str)
	{
		int strLen = str.Length;
		for (int i = 0; i < strLen; ++i)
		{
			if (str[strLen - i - 1] > '9' || str[strLen - i - 1] < '0')
			{
				return strLen - i - 1;
			}
		}
		return -1;
	}
	public static string getNotNumberSubString(string str)
	{
		int notNumPos = getLastNotNumberPos(str);
		return str.Substring(0, notNumPos + 1);
	}
	public static int getLastNumber(string str)
	{
		int lastPos = getLastNotNumberPos(str);
		if (lastPos == -1)
		{
			return -1;
		}
		string numStr = str.Substring(lastPos + 1, str.Length - lastPos - 1);
		if (numStr == "")
		{
			return 0;
		}
		return stringToInt(numStr);
	}
	public static int stringToInt(string str)
	{
		str = checkIntString(str);
		if(str == "")
		{
			return 0;
		}
		return int.Parse(str);
	}
	public static uint stringToUInt(string str)
	{
		str = checkUIntString(str);
		if (str == "")
		{
			return 0;
		}
		return uint.Parse(str);
	}
	public static Vector2 stringToVector2(string value, string seperate = ",")
	{
		string[] spitList = split(value, true, seperate);
		if (spitList.Length < 2)
		{
			return Vector2.zero;
		}
		Vector2 v = new Vector2();
		v.x = stringToFloat(spitList[0]);
		v.y = stringToFloat(spitList[1]);
		return v;
	}
	public static Vector3 stringToVector3(string value, string seperate = ",")
	{
		string[] spitList = split(value, true, seperate);
		if (spitList.Length < 3)
		{
			return Vector3.zero;
		}
		Vector3 v = new Vector3();
		v.x = stringToFloat(spitList[0]);
		v.y = stringToFloat(spitList[1]);
		v.z = stringToFloat(spitList[2]);
		return v;
	}
	public static Vector4 stringToVector4(string value, string seperate = ",")
	{
		string[] spitList = split(value, true, seperate);
		if (spitList.Length < 4)
		{
			return Vector4.zero;
		}
		Vector4 v = new Vector4();
		v.x = stringToFloat(spitList[0]);
		v.y = stringToFloat(spitList[1]);
		v.z = stringToFloat(spitList[2]);
		v.w = stringToFloat(spitList[3]);
		return v;
	}
	public static void removeLast(ref string stream, char key)
	{
		int lastCommaPos = stream.LastIndexOf(key);
		if (lastCommaPos != -1)
		{
			stream = stream.Remove(lastCommaPos, 1);
		}
	}
	// 去掉最后一个逗号
	public static void removeLastComma(ref string stream)
	{
		removeLast(ref stream, ',');
	}
	// json
	public static void jsonStartArray(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "[";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonEndArray(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		removeLastComma(ref str);
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "],";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonStartStruct(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "{";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonEndStruct(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		removeLastComma(ref str);
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "},";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonAddPair(ref string str, string name, string value, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "\"" + name + "\": \"" + value + "\",";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonAddObject(ref string str, string name, string value, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "\"" + name + "\": " + value + ",";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static string removeSuffix(string str)
	{
		int dotPos = str.IndexOf('.');
		if (dotPos != -1)
		{
			return str.Substring(0, dotPos);
		}
		return str;
	}
	public static string getFirstFolderName(string str)
	{
		rightToLeft(ref str);
		string ret = "";
		int firstPos = str.IndexOf('/');
		if(firstPos != -1)
		{
			ret = str.Substring(0, firstPos);
		}
		return ret;
	}
	// 从文件路径中得到最后一级的文件夹名
	public static string getFolderName(string str)
	{
		rightToLeft(ref str);
		string ret = str;
		// 如果有文件名,则先去除文件名
		int namePos = ret.LastIndexOf('/');
		int dotPos = ret.LastIndexOf('.');
		if (dotPos > namePos)
		{
			ret = ret.Substring(0, namePos + 1);
		}
		// 再去除当前目录的父级目录
		namePos = ret.LastIndexOf('/');
		if (namePos != -1)
		{
			ret = ret.Substring(namePos + 1);
		}
		return ret;
	}
	// 得到文件路径
	public static string getFilePath(string fileName)
	{
		rightToLeft(ref fileName);
		int lastPos = fileName.LastIndexOf('/');
		if(lastPos != -1)
		{
			return fileName.Substring(0, lastPos);
		}
		return "";
	}
	public static string getFileName(string str)
	{
		rightToLeft(ref str);
		int dotPos = str.LastIndexOf('/');
		if (dotPos != -1)
		{
			return str.Substring(dotPos + 1);
		}
		return str;
	}
	public static string getFileSuffix(string file)
	{
		int filePos = file.LastIndexOf('/');
		int dotPos = file.IndexOf('.', filePos);
		if(dotPos != -1)
		{
			return file.Substring(dotPos);
		}
		return "";
	}
	public static string getFileNameNoSuffix(string str, bool removeDir = false)
	{
		rightToLeft(ref str);
		int namePos = str.LastIndexOf('/');
		string ret = str;
		// 先判断是否移除目录
		if(removeDir && namePos != -1)
		{
			ret = str.Substring(namePos + 1);
		}
		// 移除后缀
		int dotPos = ret.LastIndexOf('.');
		if (dotPos != -1)
		{
			ret = ret.Substring(0, dotPos);
		}
		return ret;
	}
	public static void rightToLeft(ref string str)
	{
		str = str.Replace('\\', '/');
	}
	public static void leftToRight(ref string str)
	{
		str = str.Replace('/', '\\');
	}
	public static string[] split(string str, bool removeEmpty, params string[] keyword)
	{
		StringSplitOptions option = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
		string[] strList = str.Split(keyword, option);
		return strList;
	}
	public static void split(string str, ref List<string> strList, bool removeEmpty, params string[] keyword)
	{
		string[] strArray = split(str, removeEmpty, keyword);
		if (strList == null)
		{
			strList = new List<string>(strArray);
		}
		else
		{
			strList.Clear();
			int strCount = strArray.Length;
			for (int i = 0; i < strCount; ++i)
			{
				strList.Add(strArray[i]);
			}
		}
	}
	public static void stringToFloatArray(string str, ref float[] values, string seperate = ",")
	{
		string[] rangeList = split(str, true, seperate);
		int len = rangeList.Length;
		if (values != null && len != values.Length)
		{
			UnityUtility.logError("count is not equal " + str.Length);
			return;
		}
		if (values == null)
		{
			values = new float[len];
		}

		for (int i = 0; i < len; ++i)
		{
			values[i] = stringToFloat(rangeList[i]);
		}
	}
	public static void stringToFloatArray(string str, ref List<float> values, string seperate = ",")
	{
		string[] rangeList = split(str, true, seperate);
		int len = rangeList.Length;
		if (values != null && len != values.Count)
		{
			UnityUtility.logError("count is not equal " + str.Length);
			return;
		}
		if (values == null)
		{
			values = new List<float>();
		}

		for (int i = 0; i < len; ++i)
		{
			values.Add(stringToFloat(rangeList[i]));
		}
	}
	public static string floatArrayToString(float[] values, string seperate = ",")
	{
		string str = "";
		int count = values.Length;
		for (int i = 0; i < count; ++i)
		{
			str += floatToString(values[i], 2);
			if (i != count - 1)
			{
				str += seperate;
			}
		}
		return str;
	}
	public static string floatArrayToString(List<float> values, string seperate = ",")
	{
		string str = "";
		int count = values.Count;
		for (int i = 0; i < count; ++i)
		{
			str += floatToString(values[i], 2);
			if (i != count - 1)
			{
				str += seperate;
			}
		}
		return str;
	}
	public static void stringToIntArray(string str, ref List<int> values, string seperate = ",")
	{
		string[] rangeList = split(str, true, seperate);
		int len = rangeList.Length;
		if (values != null && len != values.Count)
		{
			UnityUtility.logError("count is not equal " + str.Length);
			return;
		}
		if (values == null)
		{
			values = new List<int>();
		}

		for (int i = 0; i < len; ++i)
		{
			values.Add(stringToInt(rangeList[i]));
		}
	}
	public static void stringToIntArray(string str, ref int[] values, string seperate = ",")
	{
		string[] rangeList = split(str, true, seperate);
		int len = rangeList.Length;
		if (values != null && len != values.Length)
		{
			UnityUtility.logError("count is not equal " + str.Length);
			return;
		}
		if (values == null)
		{
			values = new int[len];
		}
		for (int i = 0; i < len; ++i)
		{
			values[i] = stringToInt(rangeList[i]);
		}
	}
	public static string intArrayToString(int[] values, string seperate = ",")
	{
		string str = "";
		int count = values.Length;
		for(int i = 0; i < count; ++i)
		{
			str += intToString(values[i]);
			if(i != count - 1)
			{
				str += seperate;
			}
		}
		return str;
	}
	public static string intArrayToString(List<int> values, string seperate = ",")
	{
		string str = "";
		int count = values.Count;
		for (int i = 0; i < count; ++i)
		{
			str += intToString(values[i]);
			if (i != count - 1)
			{
				str += seperate;
			}
		}
		return str;
	}
	// precision表示小数点后保留几位小数,removeTailZero表示是否去除末尾的0
	public static string floatToString(float value, int precision = 4, bool removeTailZero = true)
	{
		string str = value.ToString("f" + precision.ToString());
		// 去除末尾的0
		if (removeTailZero)
		{
			int removeCount = 0;
			int curLen = str.Length;
			// 从后面开始遍历
			for (int i = 0; i < curLen; ++i)
			{
				char c = str[curLen - 1 - i];
				// 遇到不是0的就退出循环
				if (c != '0' && c != '.')
				{
					removeCount = i;
					break;
				}
				// 遇到小数点就退出循环并且需要将小数点一起去除
				else if (c == '.')
				{
					removeCount = i + 1;
					break;
				}
			}
			str = str.Substring(0, curLen - removeCount);
		}
		return str;
	}
	// 给数字字符串以千为单位添加逗号
	public static void insertNumberComma(ref string str)
	{
		int length = str.Length;
		int commaCount = length / 3;
		if(length > 0 && length % 3 == 0)
		{
			commaCount -= 1;
		}
		int insertStart = length % 3;
		if(insertStart == 0)
		{
			insertStart = 3;
		}
		insertStart += 3 * (commaCount - 1);
		// 从后往前插入
		for (int i = 0; i < commaCount; ++i)
		{
			str = str.Insert(insertStart, ",");
			insertStart -= 3;
		}
	}
	public static string boolToString(bool value, bool firstUpper = false, bool fullUpper = false)
	{
		if(fullUpper)
		{
			return value ? "TRUE" : "FALSE";
		}
		if (firstUpper)
		{
			return value ? "True" : "False";
		}
		return value ? "true" : "false";
	}
	public static bool stringToBool(string str)
	{
		return str == "true" || str == "True" || str == "TRUE";
	}
	public static string intToString(int value, int limitLen = 0)
	{
		string retString = value.ToString();
		int addLen = limitLen - retString.Length;
		if (addLen > 0)
		{
			for (int i = 0; i < addLen; ++i)
			{
				retString = "0" + retString;
			}
		}
		return retString;
	}
	public static string uintToString(uint value, int limitLen = 0)
	{
		string retString = value.ToString();
		int addLen = limitLen - retString.Length;
		if (addLen > 0)
		{
			for (int i = 0; i < addLen; ++i)
			{
				retString = "0" + retString;
			}
		}
		return retString;
	}
	public static string vector2ToString(Vector2 value, int precision = 4)
	{
		return floatToString(value.x, precision) + "," + floatToString(value.y, precision);
	}
	public static string vector3ToString(Vector3 value, int precision = 4)
	{
		return floatToString(value.x, precision) + "," + floatToString(value.y, precision) + "," + floatToString(value.z, precision);
	}
	// 将str中的[begin,end)替换为reStr
	public static string strReplace(string str, int begin, int end, string reStr)
	{
		string sub1 = str.Substring(0, begin);
		string sub2 = str.Substring(end, str.Length - end);
		return sub1 + reStr + sub2;
	}
	public static string strReplaceAll(string str, string key, string newWords)
	{
		int startPos = 0;
		while (true)
		{
			INT pos = new INT();
			if (!findSubString(str, key, false, pos, startPos))
			{
				break;
			}
			str = strReplace(str, pos.mValue, pos.mValue + key.Length, newWords);
			startPos = pos.mValue + newWords.Length;
		}
		return str;
	}
	public static float stringToFloat(string str)
	{
		str = checkFloatString(str, "-");
		if(str == "")
		{
			return 0.0f;
		}
		return float.Parse(str);
	}
	public static int getStringLength(string str)
	{
		byte[] bytes = stringToBytes(str);
		for (int i = 0; i < bytes.Length; ++i)
		{
			if (bytes[i] == 0)
			{
				return i;
			}
		}
		return bytes.Length;
	}
	public static string checkString(string str, string valid)
	{
		string newString = "";
		int validCount = valid.Length;
		int oldStrLen = str.Length;
		for (int i = 0; i < oldStrLen; ++i)
		{
			bool keep = false;
			for (int j = 0; j < validCount; ++j)
			{
				if (str[i] == valid[j])
				{
					keep = true;
					break;
				}
			}
			if (keep)
			{
				newString += str[i];
			}
		}
		return newString;
	}
	public static string checkFloatString(string str, string valid = "")
	{
		return checkIntString(str, "." + valid);
	}
	public static string checkIntString(string str, string valid = "")
	{
		return checkString(str, "-0123456789" + valid);
	}
	public static string checkUIntString(string str, string valid = "")
	{
		return checkString(str, "0123456789" + valid);
	}
	public static string bytesToHEXString(byte[] byteList, bool addSpace = true, bool upperOrLower = true, int count = 0)
	{
		string byteString = "";
		int byteCount = count > 0 ? count : byteList.Length;
		byteCount = MathUtility.getMin(byteList.Length, byteCount);
		for (int i = 0; i < byteCount; ++i)
		{
			if (addSpace)
			{
				byteString += byteToHEXString(byteList[i], upperOrLower) + " ";
			}
			else
			{
				byteString += byteToHEXString(byteList[i], upperOrLower);
			}
		}
		if (addSpace)
		{
			byteString = byteString.Substring(0, byteString.Length - 1);
		}
		return byteString;
	}
	public static string byteToHEXString(byte value, bool upperOrLower = true)
	{
		string hexString = "";
		char[] hexChar = null;
		if (upperOrLower)
		{
			hexChar = new char[] { 'A', 'B', 'C', 'D', 'E', 'F' };
		}
		else
		{
			hexChar = new char[] { 'a', 'b', 'c', 'd', 'e', 'f' };
		}
		int high = value / 16;
		int low = value % 16;
		if (high < 10)
		{
			hexString += (char)('0' + high);
		}
		else
		{
			hexString += hexChar[high - 10];
		}
		if (low < 10)
		{
			hexString += (char)('0' + low);
		}
		else
		{
			hexString += hexChar[low - 10];
		}
		return hexString;
	}
	public static byte hexStringToByte(string str, int start = 0)
	{
		byte highBit = 0;
		byte lowBit = 0;
		byte[] strBytes = stringToBytes(str);
		byte highBitChar = strBytes[start];
		byte lowBitChar = strBytes[start + 1];
		if (highBitChar >= 'A' && highBitChar <= 'F')
		{
			highBit = (byte)(10 + highBitChar - 'A');
		}
		else if (highBitChar >= 'a' && highBitChar <= 'f')
		{
			highBit = (byte)(10 + highBitChar - 'a');
		}
		else if (highBitChar >= '0' && highBitChar <= '9')
		{
			highBit = (byte)(highBitChar - '0');
		}
		if (lowBitChar >= 'A' && lowBitChar <= 'F')
		{
			lowBit = (byte)(10 + lowBitChar - 'A');
		}
		else if (lowBitChar >= 'a' && lowBitChar <= 'f')
		{
			lowBit = (byte)(10 + lowBitChar - 'a');
		}
		else if (lowBitChar >= '0' && lowBitChar <= '9')
		{
			lowBit = (byte)(lowBitChar - '0');
		}
		return (byte)(highBit << 4 | lowBit);
	}
	public static byte[] hexStringToBytes(string str)
	{
		str = checkString(str, "ABCDEFabcdef0123456789");
		if (str == "" || str.Length % 2 != 0)
		{
			return null;
		}
		int dataCount = str.Length / 2;
		byte[] data = new byte[dataCount];
		for (int i = 0; i < dataCount; ++i)
		{
			data[i] = hexStringToByte(str, i * 2);
		}
		return data;
	}
	public static bool findSubString(string source, string subStr, bool sensitive, INT pos = null, int startPos = 0)
	{
		if (source.Length < subStr.Length)
		{
			return false;
		}
		// 如果不区分大小写
		if (!sensitive)
		{
			// 全转换为小写
			source = source.ToLower();
			subStr = subStr.ToLower();
		}
		int findPos = -1;
		for (int i = startPos; i < source.Length; ++i)
		{
			// 剩余长度不足子字符串,则没找到
			if (source.Length - i < subStr.Length)
			{
				continue;
			}
			int j = 0;
			for (j = 0; j < subStr.Length; ++j)
			{
				if (i + j >= 0 && i + j < source.Length && source[i + j] != subStr[j])
				{
					break;
				}
			}
			if (j == subStr.Length)
			{
				findPos = i;
				break;
			}
		}
		if (pos != null)
		{
			pos.mValue = findPos;
		}
		return findPos != -1;
	}
}