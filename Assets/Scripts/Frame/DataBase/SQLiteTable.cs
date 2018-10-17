using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;

public class TableData
{
	public virtual void parse(SqliteDataReader reader) { }
}

public class SQLiteTable
{
	protected string mTableName;
	protected SQLite mSQLite;
	public SQLiteTable(string name)
	{
		mTableName = name;
	}
	public virtual void init(SQLite sqlite)
	{
		mSQLite = sqlite;
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
	public void appendValueString(ref string queryStr, string str, bool isEnd = false)
	{
		queryStr += "\"" + str + "\"";
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public void appendValueInt(ref string queryStr, int value, bool isEnd = false)
	{
		queryStr += StringUtility.intToString(value);
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public void appendValueFloat(ref string queryStr, float value, bool isEnd = false)
	{
		queryStr += StringUtility.floatToString(value);
		if (!isEnd)
		{
			queryStr += ",";
		}
	}
	public void appendValueFloatArray(ref string queryStr, List<float> floatArray, bool isEnd = false)
	{
		appendValueString(ref queryStr, StringUtility.floatArrayToString(floatArray), isEnd);
	}
	public void appendValueIntArray(ref string queryStr, List<int> intArray, bool isEnd = false)
	{
		appendValueString(ref queryStr, StringUtility.intArrayToString(intArray), isEnd);
	}
	public void appendConditionString(ref string condition, string col, string str, string operate)
	{
		condition += col + " = " + "\"" + str + "\"" + operate;
	}
	public void appendConditionInt(ref string condition, string col, int value, string operate)
	{
		condition += col + " = " + StringUtility.intToString(value) + operate;
	}
	public void appendUpdateString(ref string updateInfo, string col, string str, bool isEnd = false)
	{
		updateInfo += col + " = " + "\"" + str + "\"";
		if (!isEnd)
		{
			updateInfo += ",";
		}
	}
	public void appendUpdateInt(ref string updateInfo, string col, int value, bool isEnd = false)
	{
		updateInfo += col + " = " + StringUtility.intToString(value);
		if (!isEnd)
		{
			updateInfo += ",";
		}
	}
	public void appendUpdateIntArray(ref string updateInfo, string col, List<int> intArray, bool isEnd = false)
	{
		appendUpdateString(ref updateInfo, col, StringUtility.intArrayToString(intArray), isEnd);
	}
	public void appendUpdateFloatArray(ref string updateInfo, string col, List<float> floatArray, bool isEnd = false)
	{
		appendUpdateString(ref updateInfo, col, StringUtility.floatArrayToString(floatArray), isEnd);
	}
	//---------------------------------------------------------------------------------------------------------------------------
	protected void parseReader<T>(SqliteDataReader reader, out T data) where T : TableData, new()
	{
		data = new T();
		data.parse(reader);
		if (reader != null)
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