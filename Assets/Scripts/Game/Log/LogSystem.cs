using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public enum LOG_TYPE
{
	LOG_PROCEDURE,          // 流程跳转
	LOG_USER_OPERATION,     // 用户操作
	LOG_START_GAME,         // 开始游戏
	LOG_GAME_ERROR,         // 游戏中的错误日志
	LOG_HTTP_OVERTIME,      // HTTP请求超时
	LOG_OTHER,              // 其他
}

public enum LOG_STATE
{
	LS_UNUPLOAD,
	LS_UPLOADING,
	LS_UPLOADED,
}

public class LogData
{
	public LOG_TYPE mType;
	public DateTime mTime;
	public string mInfo;
	public Guid mGuid;
	public LOG_STATE mState;
}

public class LogSystem : FrameComponent, IFrameLogSystem
{
	protected Dictionary<string, LogData> mLogSendList;     // 需要发送的日志列表
	protected List<LogData> mLogBufferList;   // 临时存放日志的列表
	protected ThreadLock mBufferLock;
	protected ThreadLock mSqlLiteLock;
	protected ThreadLock mSendLock;
	protected string mTableName = "Log";
	protected string mUserID;
	protected CustomThread mSendThread;
	protected bool mUseLog;
	public LogSystem(string name)
		: base(name)
	{
		mLogSendList = new Dictionary<string, LogData>();
		mLogBufferList = new List<LogData>();
		mBufferLock = new ThreadLock();
		mSqlLiteLock = new ThreadLock();
		mSendLock = new ThreadLock();
		mSendThread = new CustomThread("SendLog");
		mUseLog = true;
	}
	public override void init()
	{
		mUseLog = false;
		//mUseLog = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_UPLOAD_LOG) != 0;
		if (mUseLog)
		{
			try
			{
				// 创建数据库文件,数据表格,如果已经存在则不创建
				mSQLite.createDataBase();
				string tableParam = SQLiteLogData.COL_USER_ID + " varchar(64), " +
									SQLiteLogData.COL_LOG_TYPE + " varchar(64), " +
									SQLiteLogData.COL_TIME + " varchar(32), " +
									SQLiteLogData.COL_LOG_INFO + " varchar(256), " +
									SQLiteLogData.COL_GUID + " varchar(64), " +
									SQLiteLogData.COL_UPLOADED + " integer";
				mSQLite.createTable(mTableName, tableParam);
			}
			catch (Exception e)
			{
				logError("初始化日志系统失败! " + e.Message);
				return;
			}
			mSendThread.start(sendLog, 150);
		}
	}
	public override void destroy()
	{
		base.destroy();
		mBufferLock.unlock();
		mSqlLiteLock.unlock();
		mSendThread.destroy();
		logInfo("完成退出日志");
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void logUserOperation(string info)
	{
		log(info, LOG_TYPE.LOG_USER_OPERATION);
	}
	public void logProcedure(string info)
	{
		log(info, LOG_TYPE.LOG_PROCEDURE);
	}
	public void logStartGame(string info)
	{
		log(info, LOG_TYPE.LOG_START_GAME);
	}
	public void logGameError(string info)
	{
		log(info, LOG_TYPE.LOG_GAME_ERROR);
	}
	public void logHttpOverTime(string info)
	{
		log(info, LOG_TYPE.LOG_HTTP_OVERTIME);
	}
	public void logOther(string info)
	{
		log(info, LOG_TYPE.LOG_OTHER);
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
	protected void log(string info, LOG_TYPE type)
	{
		if (!mUseLog)
		{
			return;
		}
		LogData data = new LogData();
		data.mType = type;
		data.mTime = DateTime.Now;
		data.mInfo = info;
		data.mGuid = Guid.NewGuid();
		data.mState = LOG_STATE.LS_UNUPLOAD;
		mBufferLock.waitForUnlock();
		mLogBufferList.Add(data);
		mBufferLock.unlock();
	}
	protected void sendLog(ref bool run)
	{
		// 将日志缓存同步到发送列表中
		mSendLock.waitForUnlock();
		mBufferLock.waitForUnlock();
		if (mLogBufferList.Count > 0)
		{
			for (int i = 0; i < mLogBufferList.Count; ++i)
			{
				LogData data = mLogBufferList[i];
				mLogSendList.Add(data.mGuid.ToString(), data);
			}
			mLogBufferList.Clear();
		}
		mBufferLock.unlock();
		// 复制一份列表,然后解锁列表,避免其他线程阻塞
		Dictionary<string, LogData> tempList = new Dictionary<string, LogData>(mLogSendList);
		mSendLock.unlock();
		foreach (var item in tempList)
		{
			// 找到未上传的数据
			if (item.Value.mState == LOG_STATE.LS_UNUPLOAD)
			{
				LogData data = item.Value;
				// 设置为正在上传状态
				data.mState = LOG_STATE.LS_UPLOADING;
				if (data.mType.ToString().Length >= 64 || data.mTime.ToString("G").Length >= 32 || data.mInfo.Length >= 256 || data.mGuid.ToString().Length >= 64)
				{
					continue;
				}
				mSqlLiteLock.waitForUnlock();
				SQLiteLogData logData = new SQLiteLogData();
				logData.mUserID = mUserID;
				logData.mLogType = data.mType.ToString();
				logData.mTime = data.mTime.ToString("G");
				logData.mLogInfo = data.mInfo;
				logData.mGUID = data.mGuid.ToString();
				logData.mUploaded = 0;
				mSQLiteLog.insert(logData);
				mSqlLiteLock.unlock();

				// 将日志上传服务器,并且记录到本地数据库
				string uploadData = "";
				prepareData(data, ref uploadData);
				//HttpUtility.httpWebRequestPost(GameDefine.GAME_LOG_URL, GameDefine.HTTP_URL_GAMEDATA + uploadData, onDataUploadResult, data.mGuid.ToString(), false);
			}
		}
	}
	protected void prepareData(LogData logData, ref string str)
	{
		if (logData == null)
		{
			return;
		}
		str = string.Empty;
		int type = (int)logData.mType;
		StringUtility.jsonStartStruct(ref str, 0, true);
		StringUtility.jsonAddPair(ref str, "UserID", mUserID, 1, true);
		StringUtility.jsonAddPair(ref str, "logtype", StringUtility.intToString(type), 1, true);
		StringUtility.jsonAddPair(ref str, "date", logData.mTime.ToString("G"), 1, true);
		StringUtility.jsonAddPair(ref str, "info", logData.mInfo, 1, true);
		StringUtility.jsonEndStruct(ref str, 0, true);
		StringUtility.removeLastComma(ref str);
	}
	protected void onDataUploadResult(LitJson.JsonData data, object userData)
	{
		if (data == null || data.ToJson() == "")
		{
			return;
		}
		string guid = userData as string;
		string result = data["result"].ToString();
		if (result == "fail")
		{
			mSqlLiteLock.waitForUnlock();
			mSQLiteLog.updateUploadedState(guid, 0);
			mSqlLiteLock.unlock();
			// 上传失败,设置为未上传状态
			mSendLock.waitForUnlock();
			mLogSendList[guid].mState = LOG_STATE.LS_UNUPLOAD;
			mSendLock.unlock();
		}
		else if (result == "success")
		{
			mSqlLiteLock.waitForUnlock();
			mSQLiteLog.updateUploadedState(guid, 1);
			mSqlLiteLock.unlock();
			// 上传成功,移除该条信息
			mSendLock.waitForUnlock();
			mLogSendList.Remove(guid);
			mSendLock.unlock();
		}
	}
}