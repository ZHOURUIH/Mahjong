using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System;

public class SQLite : FrameComponent
{
	protected SqliteConnection mConnection;
	protected SqliteCommand mCommand;
	protected Dictionary<Type, SQLiteTable> mTableList;
	public SQLite(string name)
		:base(name)
	{
		mTableList = new Dictionary<Type, SQLiteTable>();
		string fullPath = CommonDefine.F_DATA_BASE_PATH + GameDefine.DATA_BASE_FILE_NAME;
		if(isFileExist(fullPath))
		{
			mConnection = new SqliteConnection("DATA SOURCE = " + fullPath);   // 创建SQLite对象的同时，创建SqliteConnection对象  
			mConnection.Open();                         // 打开数据库链接	
		}
		registeTable<SQLiteSound>();
	}
	public T registeTable<T>() where T : SQLiteTable, new()
	{
		T table = new T();
		mTableList.Add(typeof(T), table);
		return table;
	}
	public override void init()
	{
		base.init();
		if(mConnection != null)
		{
			mCommand = mConnection.CreateCommand();
		}
	}
	public override void destroy()
	{
		base.destroy();
		if(mConnection != null)
		{
			mConnection.Close();
			mConnection = null;
		}
		if(mCommand != null)
		{
			mCommand.Cancel();
			mCommand = null;
		}
	}
	// 数据库文件不存在时,创建数据库文件
	public void createDataBase()
	{
		if(mConnection == null)
		{
			string fullPath = CommonDefine.F_DATA_BASE_PATH + GameDefine.DATA_BASE_FILE_NAME;
			mConnection = new SqliteConnection("DATA SOURCE = " + fullPath);   // 创建SQLite对象的同时，创建SqliteConnection对象  
			mConnection.Open();                         // 打开数据库链接
			mCommand = mConnection.CreateCommand();
		}
	}
	public void createTable(string tableName, string format)
	{
		queryNonReader("CREATE TABLE IF NOT EXISTS " + tableName + "(" + format + ");");
	}
	public SqliteDataReader queryReader(string queryString)
	{
		if (mCommand == null)
		{
			return null;
		}
		mCommand.CommandText = queryString;
		SqliteDataReader reader = null;
		try
		{
			reader = mCommand.ExecuteReader();
		}
		catch(Exception){}
		return reader;
	}
	public void queryNonReader(string queryString)
	{
		if (mCommand == null)
		{
			return;
		}
		mCommand.CommandText = queryString;
		try
		{
			mCommand.ExecuteNonQuery();
		}
		catch(Exception){}
	}
	public void getTable<T>(out T table) where T : SQLiteTable
	{
		table = null;
		if (mTableList.ContainsKey(typeof(T)))
		{
			table = mTableList[typeof(T)] as T;
		}
	}
}