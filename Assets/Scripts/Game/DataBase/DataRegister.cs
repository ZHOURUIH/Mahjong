using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataRegister : GameBase
{
	public static void registeAllData()
	{
		registeData<DataGameSound>(DATA_TYPE.DT_GAME_SOUND);
	}
	//-------------------------------------------------------------------------------------------------
	protected static void registeData<T>(DATA_TYPE type) where T : Data
	{
		mDataBase.registeData(typeof(T), type);
	}
}