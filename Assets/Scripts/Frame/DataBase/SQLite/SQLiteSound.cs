using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SoundData : TableData
{
	public static string COL_ID = "ID";
	public static string COL_FILE_NAME = "FileName";
	public static string COL_DESC = "Desc";
	public static string COL_VOLUME_SCALE = "VolumeScale";
	public int mID;
	public string mFileName;
	public string mDescribe;
	public float mVolumeScale;
	public override void parse(SqliteDataReader reader)
	{
		mID = stringToInt(reader[COL_ID].ToString());
		mFileName = reader[COL_FILE_NAME].ToString();
		mDescribe = reader[COL_DESC].ToString();
		mVolumeScale = stringToFloat(reader[COL_VOLUME_SCALE].ToString());
	}
}

public class SQLiteSound : SQLiteTable
{
	public SQLiteSound()
		:base("Sound")
	{}
	public void query(int id, out SoundData data)
	{
		string condition = "";
		appendConditionInt(ref condition, SoundData.COL_ID, id, "");
		parseReader(doQuery(condition), out data);
	}
	public void queryAll(out List<SoundData> dataList)
	{
		parseReader(doQuery(), out dataList);
	}
	public void insert(SoundData data)
	{
		string valueString = "";
		appendValueInt(ref valueString, data.mID);
		appendValueString(ref valueString, data.mFileName);
		appendValueString(ref valueString, data.mDescribe);
		appendValueFloat(ref valueString, data.mVolumeScale, true);
		doInsert(valueString);
	}
};