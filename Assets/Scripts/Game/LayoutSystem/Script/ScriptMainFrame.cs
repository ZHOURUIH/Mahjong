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
	}
	public override void init()
	{
		mMailButton.setClickCallback(onMailButton);
		mMailButton.setPressCallback(onButtonPress);
		mCompetitionButton.setClickCallback(onCompetitionButton);
		mCompetitionButton.setPressCallback(onButtonPress);
		mShareButton.setClickCallback(onShareButton);
		mShareButton.setPressCallback(onButtonPress);
		mStandingButton.setClickCallback(onStandingButton);
		mStandingButton.setPressCallback(onButtonPress);
		mRuleButton.setClickCallback(onRuleButton);
		mRuleButton.setPressCallback(onButtonPress);
		mContactButton.setClickCallback(onContactButton);
		mContactButton.setPressCallback(onButtonPress);
		mRechargeButton.setClickCallback(onRechargeButton);
		mRechargeButton.setPressCallback(onButtonPress);
		mSettingButton.setClickCallback(onSettingButton);
		mSettingButton.setPressCallback(onButtonPress);
		mQuitButton.setClickCallback(onQuitButton);
		mQuitButton.setPressCallback(onButtonPress);
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
	protected void onMailButton(GameObject obj)
	{
		;
	}
	protected void onCompetitionButton(GameObject obj)
	{
		;
	}
	protected void onShareButton(GameObject obj)
	{
		;
	}
	protected void onStandingButton(GameObject obj)
	{
		;
	}
	protected void onRuleButton(GameObject obj)
	{
		;
	}
	protected void onContactButton(GameObject obj)
	{
		;
	}
	protected void onRechargeButton(GameObject obj)
	{
		;
	}
	protected void onSettingButton(GameObject obj)
	{
		;
	}
	protected void onQuitButton(GameObject obj)
	{
		;
	}
	protected void onButtonPress(GameObject obj, bool press)
	{
		txUIObject button = mLayout.getUIObject(obj);
		LayoutTools.SCALE_WINDOW(button, button.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}