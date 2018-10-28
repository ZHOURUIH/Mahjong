using System;
using System.Collections.Generic;
using System.Reflection;

public class GamePluginManager : FrameComponent
{
	protected Dictionary<string, IGamePlugin> mPluginList;
	public GamePluginManager(string name)
		:base(name)
	{
		mPluginList = new Dictionary<string, IGamePlugin>();
	}
	public override void init()
	{
		loadAllPlugin();
		foreach(var item in mPluginList)
		{
			item.Value.init();
		}
	}
	public override void update(float elapsedTime)
	{
		foreach (var item in mPluginList)
		{
			item.Value.update(elapsedTime);
		}
	}
	public override void destroy()
	{
		foreach (var item in mPluginList)
		{
			item.Value.destroy();
		}
		mPluginList.Clear();
		base.destroy();
	}
	//-----------------------------------------------------------------------------------------------------------
	protected void loadAllPlugin()
	{
#if UNITY_STANDALONE_WIN
		if(!isDirExist(CommonDefine.F_GAME_PLUGIN_PATH))
		{
			return;
		}
		List<string> fileList = new List<string>();
		findFiles(CommonDefine.F_GAME_PLUGIN_PATH, ref fileList, CommonDefine.DLL_PLUGIN_SUFFIX);
		int count = fileList.Count;
		for(int i = 0; i < count; ++i)
		{
			byte[] fileBuffer = null;
			openFile(fileList[i], ref fileBuffer);
			loadPlugin(fileBuffer);
		}
#endif
	}
	protected bool loadPlugin(byte[] rawDll)
	{
		try
		{
			var assembly = Assembly.Load(rawDll);
			var types = assembly.GetTypes();
			foreach (var type in types)
			{
				if (type.GetInterfaces().Length > 0)
				{
					IGamePlugin instance = assembly.CreateInstance(type.FullName) as IGamePlugin;
					if (instance != null)
					{
						mPluginList.Add(instance.getPluginName(), instance);
					}
				}
			}
		}
		catch
		{
			return false;
		}
		return true;
	}
}