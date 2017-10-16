using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBaseData
{
	public CharacterBaseData()
	{
		mGUID = -1;
		mName = "";
	}
	public int				mGUID;		// 玩家唯一ID,由服务器发送过来的
	public string			mName;
}
