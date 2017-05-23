using UnityEngine;
using System.Collections;

public class CommandLayoutManagerLoadLayout : Command 
{
	public LAYOUT_TYPE	mType;
	public bool			mVisible;
	public GameLayout	mResultLayout;
	public int			mRenderOrder;
	public bool			mAsync;
	public LayoutAsyncDone mCallback;
	public string		mParam;

	public CommandLayoutManagerLoadLayout(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
		mType = LAYOUT_TYPE.LT_MAX;
		mVisible = true;
		mResultLayout = null;
		mAsync = false;
		mCallback = null;
	}
	public override void execute()
	{
		GameLayoutManager layoutManager = mReceiver as GameLayoutManager;
		mResultLayout = layoutManager.createLayout(mType, mRenderOrder, mAsync, mCallback);
		// 只有同步加载时才能立即设置布局的显示和渲染顺序
		if (mResultLayout != null && !mAsync)
		{
			// 加载时设置显示状态是需要立即生效的,显示时会立即生效,隐藏时需要设置强制隐藏,不通知脚本
			if (mVisible)
			{
				mResultLayout.setVisible(mVisible, true, mParam);
			}
			else
			{
				mResultLayout.setVisibleForce(mVisible);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString()+"layout type : " + mType + ", visible : " + mVisible;
	}
}