using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMainFrame : LayoutScript
{
	protected txUIStaticSprite mBackground;
	protected txUIObject mBottomButtonRoot;
	protected txUIObject mLeftTopButtonRoot;
	protected txUIStaticSprite mFrameTitle;
	protected txUIButton mMailButton;
	protected txUIButton mCompetitionButton;
	protected txUIButton mShareButton;
	protected txUIButton mStandingButton;
	protected txUIButton mRuleButton;
	protected txUIButton mContactButton;
	protected txUIButton mRechargeButton;
	protected txUIButton mSettingButton;
	protected txUIButton mQuitButton;
	public ScriptMainFrame(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		;
	}
	public override void init()
	{
		mBackground = newObject<txUIStaticSprite>("Background");
		mBottomButtonRoot = newObject<txUIObject>(mBackground, "BottomButtonRoot");
		mLeftTopButtonRoot = newObject<txUIObject>(mBackground, "LeftTopButtonRoot");
		mFrameTitle = newObject<txUIStaticSprite>(mBackground, "FrameTitle");
		mMailButton = newObject<txUIButton>(mBottomButtonRoot, "MailButton");
		mCompetitionButton = newObject<txUIButton>(mBottomButtonRoot, "CompetitionButton");
		mShareButton = newObject<txUIButton>(mBottomButtonRoot, "ShareButton");
		mStandingButton = newObject<txUIButton>(mBottomButtonRoot, "StandingButton");
		mRuleButton = newObject<txUIButton>(mBottomButtonRoot, "RuleButton");
		mContactButton = newObject<txUIButton>(mBottomButtonRoot, "ContactButton");
		mRechargeButton = newObject<txUIButton>(mLeftTopButtonRoot, "RechargeButton");
		mSettingButton = newObject<txUIButton>(mLeftTopButtonRoot, "SettingButton");
		mQuitButton = newObject<txUIButton>(mLeftTopButtonRoot, "QuitButton");

		mGlobalTouchSystem.registerBoxCollider(mMailButton, onMailButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mCompetitionButton, onCompetitionButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mShareButton, onShareButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mStandingButton, onStandingButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mRuleButton, onRuleButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mContactButton, onContactButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mRechargeButton, onRechargeButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mSettingButton, onSettingButton, null, onButtonPress);
		mGlobalTouchSystem.registerBoxCollider(mQuitButton, onQuitButton, null, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mMailButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mCompetitionButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mShareButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mStandingButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mRuleButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mContactButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mRechargeButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mSettingButton, Vector2.one, Vector2.one, 0.0f);
		LayoutTools.SCALE_WINDOW(mQuitButton, Vector2.one, Vector2.one, 0.0f);
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
	protected void onMailButton(txUIButton obj)
	{
		;
	}
	protected void onCompetitionButton(txUIButton obj)
	{
		;
	}
	protected void onShareButton(txUIButton obj)
	{
		;
	}
	protected void onStandingButton(txUIButton obj)
	{
		;
	}
	protected void onRuleButton(txUIButton obj)
	{
		;
	}
	protected void onContactButton(txUIButton obj)
	{
		;
	}
	protected void onRechargeButton(txUIButton obj)
	{
		;
	}
	protected void onSettingButton(txUIButton obj)
	{
		;
	}
	protected void onQuitButton(txUIButton obj)
	{
		;
	}
	protected void onButtonPress(txUIButton obj, bool press)
	{
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}