using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SQLiteRegister : GameBase
{
	public static void registeAllTable()
	{
		registeTable(ref mSQLiteLog);
	}
	//-------------------------------------------------------------------------------------------------------------
	protected static void registeTable<T>(ref T table) where T : SQLiteTable, new()
	{
		table = mSQLite.registeTable<T>();
	}
}