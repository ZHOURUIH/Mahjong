using UnityEngine;
using System.Collections;

public enum MOUSE_BUTTON
{
	MB_LEFT,
	MB_RIGHT,
	MB_MIDDLE,
}

public enum FOCUS_MASK
{
	FM_NONE		= 0x00000000,
	FM_SCENE	= 0x00000001,
	FM_UI		= 0x00000010,
	FM_OTHER	= 0x00000100,
}

public class InputManager : FrameComponent
{
	protected Vector2 mLastMousePosition;
	protected Vector2 mCurMousePosition;
	protected Vector2 mMouseDelta;
	protected int mFocusMask;
	public InputManager(string name)
		:base(name)
	{ }
	public override void init()
	{
		mFocusMask = 0;
	}
	public override void destroy()
	{
		base.destroy();
	}
	public void addInputMask(FOCUS_MASK mask)
	{
		mFocusMask |= (int)mask;
	}
	public void removeInputMask(FOCUS_MASK mask)
	{
		mFocusMask &= ~(int)mask;
	}
	public void setMask(FOCUS_MASK mask)
	{
		mFocusMask = (int)mask;
	}
	public bool hasMask(FOCUS_MASK mask)
	{
		return (mask == FOCUS_MASK.FM_NONE || mFocusMask == 0 || (mFocusMask & (int)mask) != 0);
	}
	public override void update(float elapsedTime)
	{
		mCurMousePosition = Input.mousePosition;
		mMouseDelta = mCurMousePosition - mLastMousePosition;
		mLastMousePosition = mCurMousePosition;
	}
	public void setMouseVisible(bool visible)
	{
		Cursor.visible = visible;
	}
	public new Vector2 getMousePosition()
	{
		return mCurMousePosition;
	}
	public Vector2 getMouseDelta()
	{
		return mMouseDelta;
	}
	public float getMouseWheelDelta()
	{
		return Input.mouseScrollDelta.y;
	}
	public bool getMouseDown(MOUSE_BUTTON mouse, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return getMouseKeepDown(mouse, mask) || getMouseCurrentDown(mouse, mask);
	}
	public bool getMouseKeepDown(MOUSE_BUTTON mouse, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return Input.GetMouseButton((int)mouse) && hasMask(mask);
	}
	public bool getMouseCurrentDown(MOUSE_BUTTON mouse, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return Input.GetMouseButtonDown((int)mouse) && hasMask(mask);
	}
	public bool getMouseCurrentUp(MOUSE_BUTTON mouse, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return Input.GetMouseButtonUp((int)mouse) && hasMask(mask);
	}
	public new virtual bool getKeyCurrentDown(KeyCode key, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return Input.GetKeyDown(key) && hasMask(mask);
	}
	public new virtual bool getKeyCurrentUp(KeyCode key, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return Input.GetKeyUp(key) && hasMask(mask);
	}
	public new virtual bool getKeyDown(KeyCode key, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return Input.GetKey(key) && hasMask(mask);
	}
	public new virtual bool getKeyUp(KeyCode key, FOCUS_MASK mask = FOCUS_MASK.FM_NONE)
	{
		return !Input.GetKey(key) && hasMask(mask);
	}
}