using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMainFrame : LayoutScript
{
	protected txNGUIStaticSprite mBackground;
	protected txUIObject mBottomButtonRoot;
	protected txUIObject mLeftTopButtonRoot;
	protected txNGUIStaticSprite mFrameTitle;
	protected txNGUIButton mMailButton;
	protected txNGUIButton mCompetitionButton;
	protected txNGUIButton mShareButton;
	protected txNGUIButton mStandingButton;
	protected txNGUIButton mRuleButton;
	protected txNGUIButton mContactButton;
	protected txNGUIButton mRechargeButton;
	protected txNGUIButton mSettingButton;
	protected txNGUIButton mQuitButton;
	public ScriptMainFrame(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
		newObject(out mBottomButtonRoot, mBackground, "BottomButtonRoot");
		newObject(out mLeftTopButtonRoot, mBackground, "LeftTopButtonRoot");
		newObject(out mFrameTitle, mBackground, "FrameTitle");
		newObject(out mMailButton, mBottomButtonRoot, "MailButton");
		newObject(out mCompetitionButton, mBottomButtonRoot, "CompetitionButton");
		newObject(out mShareButton, mBottomButtonRoot, "ShareButton");
		newObject(out mStandingButton, mBottomButtonRoot, "StandingButton");
		newObject(out mRuleButton, mBottomButtonRoot, "RuleButton");
		newObject(out mContactButton, mBottomButtonRoot, "ContactButton");
		newObject(out mRechargeButton, mLeftTopButtonRoot, "RechargeButton");
		newObject(out mSettingButton, mLeftTopButtonRoot, "SettingButton");
		newObject(out mQuitButton, mLeftTopButtonRoot, "QuitButton");
	}
	public override void init()
	{
		mGlobalTouchSystem.registeBoxCollider(mMailButton, onMailButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mCompetitionButton, onCompetitionButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mShareButton, onShareButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mStandingButton, onStandingButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mRuleButton, onRuleButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mContactButton, onContactButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mRechargeButton, onRechargeButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mSettingButton, onSettingButton, null, onButtonPress);
		mGlobalTouchSystem.registeBoxCollider(mQuitButton, onQuitButton, null, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mMailButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mCompetitionButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mShareButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mStandingButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mRuleButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mContactButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mRechargeButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mSettingButton, Vector2.one);
		LayoutTools.SCALE_WINDOW(mQuitButton, Vector2.one);
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	//-------------------------------------------------------------------------------------------------------------------------
	protected void onMailButton(txUIObject obj)
	{
		;
	}
	protected void onCompetitionButton(txUIObject obj)
	{
		;
	}
	protected void onShareButton(txUIObject obj)
	{
		;
	}
	protected void onStandingButton(txUIObject obj)
	{
		;
	}
	protected void onRuleButton(txUIObject obj)
	{
		;
	}
	protected void onContactButton(txUIObject obj)
	{
		;
	}
	protected void onRechargeButton(txUIObject obj)
	{
		;
	}
	protected void onSettingButton(txUIObject obj)
	{
		;
	}
	protected void onQuitButton(txUIObject obj)
	{
		;
	}
	protected void onButtonPress(txUIObject obj, bool press)
	{
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}