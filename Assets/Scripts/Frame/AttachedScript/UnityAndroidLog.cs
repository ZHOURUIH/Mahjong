using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnityAndroidLog : MonoBehaviour
{
	public void log(string info)
	{
		UnityUtility.logInfo("android : " + info, LOG_LEVEL.LL_FORCE);
	}
	public void logError(string info)
	{
		GameUtility.messageOK(info, true);
		UnityUtility.logError("android : " + info);
	}
}