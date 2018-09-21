using UnityEngine;
using System.Collections;

public enum MOUSE_BUTTON
{
	MB_LEFT,
	MB_RIGHT,
	MB_MIDDLE,
}

public class InputManager : FrameComponent
{
	protected Vector2 mLastMousePosition;
	protected Vector2 mCurMousePosition;
	protected Vector2 mMouseDelta;
	public InputManager(string name)
		:base(name)
	{ }
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		base.destroy();
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
	public bool getMouseDown(MOUSE_BUTTON mouse)
	{
		return getMouseKeepDown(mouse) || getMouseCurrentDown(mouse);
	}
	public bool getMouseKeepDown(MOUSE_BUTTON mouse)
	{
		return Input.GetMouseButton((int)mouse);
	}
	public bool getMouseCurrentDown(MOUSE_BUTTON mouse)
	{
		return Input.GetMouseButtonDown((int)mouse);
	}
	public bool getMouseCurrentUp(MOUSE_BUTTON mouse)
	{
		return Input.GetMouseButtonUp((int)mouse);
	}
	public new virtual bool getKeyCurrentDown(KeyCode key)
	{
		return Input.GetKeyDown(key);
	}
	public new virtual bool getKeyCurrentUp(KeyCode key)
	{
		return Input.GetKeyUp(key);
	}
	public new virtual bool getKeyDown(KeyCode key)
	{
		return Input.GetKey(key);
	}
	public new virtual bool getKeyUp(KeyCode key)
	{
		return !Input.GetKey(key);
	}
}