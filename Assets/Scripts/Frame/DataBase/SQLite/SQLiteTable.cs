using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;

public class TableData : GameBase
{
	public virtual void parse(SqliteDataReader reader) { }
}

public class SQLiteTable : GameBase
{
	protected string mTableName;
	public SQLiteTable(string name)
	{
		mTableName = name;
	}
	public SqliteDataReader doQuery()
	{
		string queryStr = "SELECT * FROM " + mTableName;
		return mSQLite.queryReader(queryStr);
	}
	public SqliteDataReader doQuery(string condition)
	{
		string queryStr = "SELECT * FROM " + mTableName + " WHERE " + condition;
		return mSQLite.queryReader(queryStr);
	}
	public void doUpdate(string updateString, string conditionString)
	{
		string queryStr = "UPDATE " + mTableName + " SET " + updateString + " WHERE " + conditionString;
		mSQLite.queryNonReader(queryStr);
	}
	public void doInsert(string valueString)
	{
		string queryString = "INSERT INTO " + mTableName + " VALUES (" + valueString + ")";
		mSQLite.queryNonReader(queryString);
	}
	public SqliteDataReader queryReader(string queryString)
	{
		return mSQLite.queryReader(queryString);
	}
	public static void appendValueString(ref string queryStr, string str, bool isEnd = false)
	{
		queryStr += "\"" + str + "\"";
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public static void appendValueInt(ref string queryStr, int value, bool isEnd = false)
	{
		queryStr += intToString(value);
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public static void appendValueUInt(ref string queryStr, uint value, bool isEnd = false)
	{
		queryStr += uintToString(value);
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public static void appendValueFloat(ref string queryStr, float value, bool isEnd = false)
	{
		queryStr += floatToString(value);
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public static void appendValueFloatArray(ref string queryStr, List<float> floatArray, bool isEnd = false)
	{
		appendValueString(ref queryStr, floatArrayToString(floatArray), isEnd);
	}
	public static void appendValueIntArray(ref string queryStr, List<int> intArray, bool isEnd = false)
	{
		appendValueString(ref queryStr, intArrayToString(intArray), isEnd);
	}
	public static void appendConditionString(ref string condition, string col, string str, string operate)
	{
		condition += col + " = " + "\"" + str + "\"" + operate;
	}
	public static void appendConditionInt(ref string condition, string col, int value, string operate)
	{
		condition += col + " = " + intToString(value) + operate;
	}
	public static void appendUpdateString(ref string updateInfo, string col, string str, bool isEnd = false)
	{
		updateInfo += col + " = " + "\"" + str + "\"";
		if (!isEnd)
		{
			updateInfo += ",";
		}
	}
	public static void appendUpdateInt(ref string updateInfo, string col, int value, bool isEnd = false)
	{
		updateInfo += col + " = " + intToString(value);
		if (!isEnd)
		{
			updateInfo += ",";
		}
	}
	public static void appendUpdateIntArray(ref string updateInfo, string col, List<int> intArray, bool isEnd = false)
	{
		appendUpdateString(ref updateInfo, col, intArrayToString(intArray), isEnd);
	}
	public static void appendUpdateFloatArray(ref string updateInfo, string col, List<float> floatArray, bool isEnd = false)
	{
		appendUpdateString(ref updateInfo, col, floatArrayToString(floatArray), isEnd);
	}
	//---------------------------------------------------------------------------------------------------------------------------
	protected void parseReader<T>(SqliteDataReader reader, out T data) where T : TableData, new()
	{
		data = null;
		if (reader != null && reader.Read())
		{
			data = new T();
			data.parse(reader);
		}
		if(reader != null)
		{
			reader.Close();
		}
	}
	protected void parseReader<T>(SqliteDataReader reader, out List<T> dataList) where T : TableData, new()
	{
		dataList = new List<T>();
		if (reader != null)
		{
			while (reader.Read())
			{
				T data = new T();
				data.parse(reader);
				dataList.Add(data);
			}
			reader.Close();
		}
	}
}