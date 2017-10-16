using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataRegister : GameBase
{
	public void registeAllData()
	{
		registeData(typeof(DataGameSound), DATA_TYPE.DT_GAME_SOUND);
	}
	//-------------------------------------------------------------------------------------------------
	protected void registeData(Type data, DATA_TYPE type)
	{
		mDataBase.registeData(data, type);
	}
}