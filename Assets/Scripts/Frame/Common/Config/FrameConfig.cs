using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class FrameConfig : ConfigBase
{
	public FrameConfig(string name)
		:base(name)
	{ }
	public override void writeConfig()
	{
		FileUtility.writeTxtFile(CommonDefine.F_CONFIG_PATH + "FrameFloatConfig.txt", generateFloatFile());
	}
	//-----------------------------------------------------------------------------------------------------------------------
	protected override void addFloat()
	{
		addFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_PORT);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_BROADCAST_PORT);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_LOAD_RESOURCES);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_LOG_LEVEL);
		if (mFloatNameToDefine.Count != (int)GAME_DEFINE_FLOAT.GDF_FRAME_MAX - (int)GAME_DEFINE_FLOAT.GDF_FRAME_MIN - 1)
		{
			UnityUtility.logError("not all float parameter added!");
		}
	}
	protected override void addString()
	{
		if (mStringNameToDefine.Count != (int)GAME_DEFINE_STRING.GDS_FRAME_MAX - (int)GAME_DEFINE_STRING.GDS_FRAME_MIN - 1)
		{
			UnityUtility.logError("not all string parameter added!");
		}
	}
	protected override void readConfig()
	{
		readFile(CommonDefine.F_CONFIG_PATH + "FrameFloatConfig.txt", true);
	}
}