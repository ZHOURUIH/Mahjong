using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class GameConfig : ConfigBase
{
	public override void writeConfig()
	{
		FileUtility.writeFile(CommonDefine.F_CONFIG_PATH + "GameFloatConfig.txt", generateFloatFile());
		FileUtility.writeFile(CommonDefine.F_CONFIG_PATH + "GameStringConfig.txt", generateStringFile());
	}
	//-----------------------------------------------------------------------------------------------------------------------
	protected override void addFloat()
	{
		addFloatParam(GAME_DEFINE_FLOAT.GDF_HEART_BEAT_NITERVAL);
		if (mFloatNameToDefine.Count != (int)GAME_DEFINE_FLOAT.GDF_GAME_MAX - (int)GAME_DEFINE_FLOAT.GDF_GAME_MIN - 1)
		{
			UnityUtility.logError("not all float parameter added!");
		}
	}
	protected override void addString()
	{
		addStringParam(GAME_DEFINE_STRING.GDS_TCP_SERVER_IP);
		addStringParam(GAME_DEFINE_STRING.GDS_ACCOUNT);
		addStringParam(GAME_DEFINE_STRING.GDS_PASSWORD);
		if (mStringNameToDefine.Count != (int)GAME_DEFINE_STRING.GDS_GAME_MAX - (int)GAME_DEFINE_STRING.GDS_GAME_MIN - 1)
		{
			UnityUtility.logError("not all string parameter added!");
		}
	}
	protected override void readConfig()
	{
		readFile(CommonDefine.F_CONFIG_PATH + "GameFloatConfig.txt", true);
		readFile(CommonDefine.F_CONFIG_PATH + "GameStringConfig.txt", false);
	}
}