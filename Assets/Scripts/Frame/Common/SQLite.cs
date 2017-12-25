using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;

public class SQLite
{
	protected SqliteConnection mConnection;
	protected SqliteCommand mCommand;
	public SQLite(string path)
	{
		string fullPath = CommonDefine.F_DATA_BASE_PATH + path;
		FileUtility.createDir(StringUtility.getFilePath(fullPath));
		mConnection = new SqliteConnection("DATA SOURCE = " + fullPath);   // 创建SQLite对象的同时，创建SqliteConnection对象  
		mConnection.Open();                         // 打开数据库链接  
	}
	public void init()
	{
		mCommand = mConnection.CreateCommand();
	}
	public void destroy()
	{
		mConnection.Close();
		mCommand.Cancel();
	}
	public SqliteDataReader createTable(string tableName, string format, bool needReader = false)
	{
		mCommand.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + "(" + format + ");";
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
	// 增加数据
	public SqliteDataReader insertData(string table_name, object[] values, bool needReader = false)
	{
		mCommand.CommandText = "INSERT INTO " + table_name + " VALUES (";
		int valueCount = values.Length;
		for (int i = 0; i < valueCount; ++i)
		{
			if (values[i].GetType() == typeof(string))
			{
				values[i] = "\'" + values[i] + "\'";
			}
			mCommand.CommandText += values[i].ToString();
			if (i < values.Length - 1)
			{
				mCommand.CommandText += ",";
			}
		}
		mCommand.CommandText += ")";
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
	// 删除数据
	public SqliteDataReader deleteData(string table_name, string[] conditions, bool needReader = false)
	{
		mCommand.CommandText = "DELETE FROM " + table_name + " WHERE " + conditions[0];
		for (int i = 1; i < conditions.Length; i++)
		{
			// or：表示或者  
			// and：表示并且  
			mCommand.CommandText += " OR " + conditions[i];
		}
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
	// 修改数据
	public SqliteDataReader updateData(string table_name, string[] columns, object[] values, string[] conditions, bool needReader = false)
	{
		if (columns.Length != values.Length)
		{
			return null;
		}
		mCommand.CommandText = "UPDATE " + table_name + " SET ";
		for (int i = 0; i < values.Length; ++i)
		{
			if (values[i].GetType() == typeof(string))
			{
				values[i] = "\'" + values[i] + "\'";
			}
			mCommand.CommandText += columns[i] + " = " + values[i];
			if (i < values.Length - 1)
			{
				mCommand.CommandText += ",";
			}
		}
		mCommand.CommandText += " WHERE ";
		for (int i = 0; i < conditions.Length; ++i)
		{
			mCommand.CommandText += conditions[i];
			if (i < conditions.Length - 1)
			{
				mCommand.CommandText += " OR ";
			}
		}
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
	// 修改数据
	public SqliteDataReader updateData(string table_name, object[] values, string[] conditions, bool needReader = false)
	{
		mCommand.CommandText = "UPDATE " + table_name + " SET " + values[0];
		for (int i = 1; i < values.Length; i++)
		{
			mCommand.CommandText += "," + values[i];
		}
		mCommand.CommandText += " WHERE " + conditions[0];
		for (int i = 1; i < conditions.Length; i++)
		{
			mCommand.CommandText += " OR " + conditions[i];
		}
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
	// 查询数据
	public SqliteDataReader selectData(string table_name, string[] fields, bool needReader = false)
	{
		mCommand.CommandText = "SELECT " + fields[0];
		for (int i = 1; i < fields.Length; i++)
		{
			mCommand.CommandText += "," + fields[i];
		}
		mCommand.CommandText += " FROM " + table_name;
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
	// 查询整张表的数据
	public SqliteDataReader selectFullTableData(string table_name, bool needReader = false)
	{
		mCommand.CommandText = "SELECT * FROM " + table_name;
		if (!needReader)
		{
			mCommand.ExecuteNonQuery();
			return null;
		}
		else
		{
			return mCommand.ExecuteReader();
		}
	}
}