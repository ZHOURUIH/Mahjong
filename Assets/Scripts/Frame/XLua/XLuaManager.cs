#if USE_XLUA
using XLua;
#endif

public class XLuaManager : FrameComponent
{
#if USE_XLUA
	LuaEnv mLuaEnv;

	public XLuaManager(string name)
		:base(name)
	{
		mLuaEnv = new LuaEnv();
	}
	public override void init()
	{
#if HOTFIX_ENABLE
		mLuaEnv.DoString("require 'Main'");
#endif
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mLuaEnv.Tick();
	}
	public override void destroy()
	{
		base.destroy();
		mLuaEnv.Dispose();
		mLuaEnv = null;
	}
#else
	public XLuaManager(string name)
		: base(name)
	{
		;
	}
#endif
}