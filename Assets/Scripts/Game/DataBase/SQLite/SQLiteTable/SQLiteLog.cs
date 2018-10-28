using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SQLiteLogData : TableData
{
	public static string COL_USER_ID = "UserID";
	public static string COL_LOG_TYPE = "LogType";
	public static string COL_TIME = "Time";
	public static string COL_LOG_INFO = "LogInfo";
	public static string COL_GUID = "GUID";
	public static string COL_UPLOADED = "uploaded";
	public string mUserID;
	public string mLogType;
	public string mTime;
	public string mLogInfo;
	public string mGUID;
	public int mUploaded;
	public override void parse(SqliteDataReader reader)
	{
		mUserID = reader[COL_USER_ID].ToString();
		mLogType = reader[COL_LOG_TYPE].ToString();
		mTime = reader[COL_TIME].ToString();
		mLogInfo = reader[COL_LOG_INFO].ToString();
		mGUID = reader[COL_GUID].ToString();
		mUploaded = stringToInt(reader[COL_UPLOADED].ToString());
	}
}

public class SQLiteLog : SQLiteTable
{
	public SQLiteLog()
		:base("Log")
	{}
	public void query(string guid, out SQLiteLogData data)
	{
		string condition = "";
		appendConditionString(ref condition, SQLiteLogData.COL_GUID, guid, "");
		parseReader(doQuery(condition), out data);
	}
	public void queryAll(out List<SQLiteLogData> dataList)
	{
		parseReader(doQuery(), out dataList);
	}
	public void insert(SQLiteLogData data)
	{
		string valueString = "";
		appendValueString(ref valueString, data.mUserID);
		appendValueString(ref valueString, data.mLogType);
		appendValueString(ref valueString, data.mTime);
		appendValueString(ref valueString, data.mLogInfo);
		appendValueString(ref valueString, data.mGUID);
		appendValueInt(ref valueString, data.mUploaded, true);
		doInsert(valueString);
	}
	public void updateUploadedState(string guid, int uploaded)
	{
		string updateString = "";
		appendUpdateInt(ref updateString, SQLiteLogData.COL_UPLOADED, uploaded, true);
		string conditionStr = "";
		appendConditionString(ref conditionStr, SQLiteLogData.COL_GUID, guid, "");
		doUpdate(updateString, conditionStr);
	}
};