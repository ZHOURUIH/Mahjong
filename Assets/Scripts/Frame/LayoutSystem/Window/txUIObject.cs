using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIObject : ComponentOwner
{
	protected int mID;
	protected AudioSource mAudioSource;
	protected Transform mTransform;
	protected BoxCollider mBoxCollider;
	protected UIWidget mWidget;
	protected txUIObject mParent;
	protected List<txUIObject> mChildList;
	protected GameLayout mLayout;
	protected GameObject mObject;
	protected bool mPassRay = true;
	protected bool mMouseHovered = false;
	protected bool mReceiveScreenMouse = false;			// 是否接收屏幕范围内的鼠标按下和抬起事件,无论鼠标是否在当前窗口内,默认不接收
	protected OnReceiveDragCallback mReceiveDragCallback;	// 接收到有物体拖到当前窗口时的回调
	protected OnDragHoverCallback mDragHoverCallback;   // 有物体拖拽悬停到当前窗口时的回调
	protected OnMouseEnter mOnMouseEnter;
	protected OnMouseLeave mOnMouseLeave;
	protected OnMouseDown mOnMouseDown;
	protected OnMouseUp mOnMouseUp;
	protected OnMouseMove mOnMouseMove;
	protected OnMouseStay mOnMouseStay;
	protected OnScreenMouseUp mOnScreenMouseUp;
	protected OnLongPress mOnLongPress;
	protected float mLongPressThreshold;
	protected float mPressedTime = -1.0f;	// 为负表示未计时,大于等于0表示正在计时长按操作,防止长时间按下时总会每隔指定时间调用一次回调
	protected bool mPressing;
	protected Vector2 mMouseDownPosition;
	protected UIClickCallback mClickCallback;
	protected UIHoverCallback mHoverCallback;
	protected UIPressCallback mPressCallback;
	public txUIObject()
		:base("")
	{
		mID = UnityUtility.makeID();
		mChildList = new List<txUIObject>();
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
		destroyWindow(this);
	}
	protected static void destroyWindow(txUIObject window)
	{
		// 先销毁所有子节点
		int childCount = window.mChildList.Count;
		for (int i = 0; i < childCount; ++i)
		{
			destroyWindow(window.mChildList[i]);
		}
		// 再销毁自己
		if (window.mLayout != null)
		{
			window.mLayout.unregisterUIObject(window);
			window.mLayout = null;
		}
		UnityUtility.destroyGameObject(window.mObject);
		window.mObject = null;
	}
	public virtual void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		mLayout = layout;
		setGameObject(go);
		setParent(parent);
		initComponents();
		if (mLayout != null)
		{
			mLayout.registerUIObject(this);
		}
		mAudioSource = mObject.GetComponent<AudioSource>();
		mBoxCollider = mObject.GetComponent<BoxCollider>();
		mWidget = mObject.GetComponent<UIWidget>();
		if (mBoxCollider != null && mLayout.isCheckBoxAnchor())
		{
			string layoutName = "";
			if (mLayout != null)
			{
				layoutName = mLayout.getName();
			}
			// BoxCollider必须与UIWidget(或者UIWidget的派生类)一起使用,否则在自适应屏幕时BoxCollider会出现错误
			if (mWidget == null)
			{
				logError("BoxCollider must used with UIWidget! Otherwise can not adapt to the screen sometimes! name : " + mName + ", layout : " + layoutName);
			}
			else if (!mWidget.autoResizeBoxCollider)
			{
				logError("UIWidget's autoResizeBoxCollider must be true! Otherwise can not adapt to the screen sometimes! name : " + mName + ", layout : " + layoutName);
			}
			// BoxCollider的中心必须为0,因为UIWidget会自动调整BoxCollider的大小和位置,而且调整后位置为0,所以在制作时BoxCollider的位置必须为0
			if (!isFloatZero(mBoxCollider.center.sqrMagnitude))
			{
				logError("BoxCollider's center must be zero! Otherwise can not adapt to the screen sometimes! name : " + mName + ", layout : " + layoutName);
			}
			if (mObject.GetComponent<ScaleAnchor>() == null)
			{
				logError("Window with BoxCollider and Widget must has ScaleAnchor! Otherwise can not adapt to the screen sometimes! name : " + mName + ", layout : " + layoutName);
			}
		}
	}
	public override void initComponents()
	{
		addComponent<WindowComponentAudio>("Audio");
		addComponent<WindowComponentRotateSpeed>("RotateSpeed");
		addComponent<WindowComponentMove>("Move");
		addComponent<WindowComponentScale>("Scale");
		addComponent<WindowComponentAlpha>("Alpha");
		addComponent<WindowComponentRotate>("Rotate");
		addComponent<WindowComponentSlider>("slider");
		addComponent<WindowComponentFill>("fill");
		addComponent<WindowComponentRotateFixed>("RotateFixed");
		addComponent<WindowComponentHSL>("HSL");
		addComponent<WindowComponentLum>("Lum");
		addComponent<WindowComponentDrag>("Drag");
		addComponent<WindowComponentTrackTarget>("TrackTarget");
	}
	public void addChild(txUIObject child)
	{
		if (!mChildList.Contains(child))
		{
			mChildList.Add(child);
		}
	}
	public void removeChild(txUIObject child)
	{
		if (mChildList.Contains(child))
		{
			mChildList.Remove(child);
		}
	}
	public AudioSource createAudioSource()
	{
		mAudioSource = mObject.AddComponent<AudioSource>();
		return mAudioSource;
	}
	public virtual void update(float elapsedTime)
	{
		base.updateComponents(elapsedTime);
		if(mOnLongPress != null && mPressing && mPressedTime >= 0.0f)
		{
			mPressedTime += elapsedTime;
			if (mPressedTime >= mLongPressThreshold)
			{
				mOnLongPress();
				mPressedTime = -1.0f;
			}
		}
	}
	public Vector3 localToWorld(Vector3 point)
	{
		return mObject.transform.localToWorldMatrix.MultiplyPoint(point);
	}
	public Vector3 worldToLocal(Vector3 point)
	{
		return mObject.transform.worldToLocalMatrix.MultiplyPoint(point);
	}
	//get
	//-------------------------------------------------------------------------------------------------------------------------------------
	public int getID() { return mID; }
	public GameLayout getLayout() { return mLayout; }
	public GameObject getObject() { return mObject; }
	public List<txUIObject> getChildList() { return mChildList; }
	public bool getReceiveScreenMouse() { return mReceiveScreenMouse; }
	public txUIObject getParent() { return mParent; }
	public Transform getTransform() { return mTransform; }
	public AudioSource getAudioSource() { return mAudioSource; }
	public bool isActive() { return mObject.activeSelf; }
	public BoxCollider getBoxCollider(bool addIfNull = false)
	{
		if (mBoxCollider == null && addIfNull)
		{
			mBoxCollider = mObject.AddComponent<BoxCollider>();
		}
		return mBoxCollider;
	}
	public Vector3 getRotationEuler()
	{
		Vector3 vector3 = mTransform.localEulerAngles;
		adjustAngle180(ref vector3.z);
		return vector3;
	}
	public Vector3 getRotationRadian()
	{
		Vector3 vector3 = mTransform.localEulerAngles * Mathf.Deg2Rad;
		adjustRadian180(ref vector3.z);
		return vector3;
	}
	public virtual Vector3 getPosition() { return mTransform.localPosition; }
	public virtual Vector3 getWorldPosition() { return mTransform.position; }
	public Vector2 getScale() { return new Vector2(mTransform.localScale.x, mTransform.localScale.y); }
	public Vector2 getWorldScale()
	{
		Vector3 scale = getMatrixScale(mTransform.localToWorldMatrix);
		txUIObject root = mLayout.isNGUI() ? mLayoutManager.getNGUIRoot() : mLayoutManager.getUGUIRoot();
		Vector3 uiRootScale = root.getTransform().localScale;
		return new Vector2(scale.x / uiRootScale.x, scale.y / uiRootScale.y);
	}
	public int getChildCount() { return mTransform.childCount; }
	public GameObject getChild(int index) { return mTransform.GetChild(index).gameObject; }
	public virtual float getAlpha() { return 1.0f; }
	public virtual float getFillPercent() { return 1.0f; }
	public virtual int getDepth() { return 0; }
	public virtual bool getHandleInput() { return mBoxCollider != null && mBoxCollider.enabled; }
	public bool getPassRay() { return mPassRay; }
	public bool getMouseHovered() { return mMouseHovered; }
	//set
	//-------------------------------------------------------------------------------------------------------------------------------------
	public void setParent(txUIObject parent)
	{
		if (mParent == parent)
		{
			return;
		}
		// 从原来的父节点上移除
		if (mParent != null)
		{
			mParent.removeChild(this);
		}
		// 设置新的父节点
		mParent = parent;
		if (parent != null)
		{
			parent.addChild(this);
			if (mTransform.parent != parent.mObject.transform)
			{
				mTransform.SetParent(parent.mObject.transform);
			}
		}
	}
	protected void setGameObject(GameObject go)
	{
		setName(go.name);
		mObject = go;
		mTransform = mObject.transform;
	}
	public override void setName(string name)
	{
		base.setName(name);
		if (mObject != null && mObject.name != name)
		{
			mObject.name = name;
		}
	}
	public virtual void setDepth(int depth)
	{
		mGlobalTouchSystem.notifyButtonDepthChanged(this, depth);
	}
	public virtual void setHandleInput(bool enable)
	{
		if (mBoxCollider != null)
		{
			mBoxCollider.enabled = enable;
		}
	}
	public void setReceiveScreenMouse(bool receive) { mReceiveScreenMouse = receive; }
	public void setActive(bool active) { mObject.SetActive(active); }
	public void setLocalScale(Vector2 scale) { mTransform.localScale = new Vector3(scale.x, scale.y, 1.0f); }
	public virtual void setLocalPosition(Vector3 pos) { mTransform.localPosition = pos; }
	public void setLocalRotation(Vector3 rot) { mTransform.localEulerAngles = rot; }
	public void setWorldRotation(Vector3 rot) { mTransform.eulerAngles = rot; }
	public virtual void setWorldPosition(Vector3 pos) { mTransform.position = pos; }
	public virtual void setAlpha(float alpha) { }
	public virtual void setFillPercent(float percent) { }
	public void setPassRay(bool pass) { mPassRay = pass; }
	// 由NGUI调用的callback
	public void setClickCallback(UIEventListener.VoidDelegate callback) { UIEventListener.Get(mObject).onClick = callback; }
	public void setHoverCallback(UIEventListener.BoolDelegate callback) { UIEventListener.Get(mObject).onHover = callback; }
	public void setPressCallback(UIEventListener.BoolDelegate callback) { UIEventListener.Get(mObject).onPress = callback; }
	// 自己调用的callback,仅在启用自定义输入系统时生效
	public void setClickCallback(UIClickCallback callback) { mClickCallback = callback; }
	public void setHoverCallback(UIHoverCallback callback) { mHoverCallback = callback; }
	public void setPressCallback(UIPressCallback callback) { mPressCallback = callback; }
	public void setOnLongPress(OnLongPress callback, float pressTime) { mOnLongPress = callback; mLongPressThreshold = pressTime; }
	public void setReceiveDragCallback(OnReceiveDragCallback callback) { mReceiveDragCallback = callback; }
	public void setDragHoverCallbcak(OnDragHoverCallback callback) { mDragHoverCallback = callback; }
	public void setOnMouseEnter(OnMouseEnter callback) { mOnMouseEnter = callback; }
	public void setOnMouseLeave(OnMouseLeave callback) { mOnMouseLeave = callback; }
	public void setOnMouseDown(OnMouseDown callback) { mOnMouseDown = callback; }
	public void setOnMouseUp(OnMouseUp callback) { mOnMouseUp = callback; }
	public void setOnMouseMove(OnMouseMove callback) { mOnMouseMove  = callback; }
	public void setOnMouseStay(OnMouseStay callback) { mOnMouseStay = callback; }
	public void setOnScreenMouseUp(OnScreenMouseUp callback) { mOnScreenMouseUp = callback; }
	// callback
	//--------------------------------------------------------------------------------------------------------------------------------
	public virtual void onMouseEnter(Vector2 mousePos)
	{
		mMouseHovered = true;
		if(mHoverCallback != null)
		{
			mHoverCallback(this, true);
		}
		if (mOnMouseEnter != null)
		{
			mOnMouseEnter(mousePos);
		}
	}
	public virtual void onMouseLeave(Vector2 mousePos)
	{
		mMouseHovered = false;
		mPressing = false;
		mPressedTime = -1.0f;
		if (mHoverCallback != null)
		{
			mHoverCallback(this, false);
		}
		if (mOnMouseLeave != null)
		{
			mOnMouseLeave(mousePos);
		}
	}
	// 鼠标左键在窗口内按下
	public virtual void onMouseDown(Vector2 mousePos)
	{
		mPressing = true;
		mPressedTime = 0.0f;
		mMouseDownPosition = mousePos;
		if(mPressCallback != null)
		{
			mPressCallback(this, true);
		}
		if (mOnMouseDown != null)
		{
			mOnMouseDown(mousePos);
		}
	}
	// 鼠标左键在窗口内放开
	public virtual void onMouseUp(Vector2 mousePos)
	{
		mPressing = false;
		mPressedTime = -1.0f;
		if (mPressCallback != null)
		{
			mPressCallback(this, false);
		}
		if(mClickCallback != null && getSquaredLength(mMouseDownPosition - mousePos) < GlobalTouchSystem.mSquaredClickThreshhold)
		{
			mClickCallback(this);
		}
		if (mOnMouseUp != null)
		{
			mOnMouseUp(mousePos);
		}
	}
	// 鼠标在窗口内,并且有移动
	public virtual void onMouseMove(Vector2 mousePos, Vector2 moveDelta, float moveSpeed)
	{
		if (mOnMouseMove != null)
		{
			mOnMouseMove(mousePos, moveDelta, moveSpeed);
		}
	}
	// 鼠标在窗口内,但是不移动
	public virtual void onMouseStay(Vector2 mousePos)
	{
		if(mOnMouseStay != null)
		{
			mOnMouseStay(mousePos);
		}
	}
	// 鼠标在屏幕上抬起
	public virtual void onScreenMouseUp(Vector2 mousePos)
	{
		if(mOnScreenMouseUp != null)
		{
			mOnScreenMouseUp(mousePos);
		}
	}
	// 鼠标在屏幕上按下
	public virtual void onScreenMouseDown(Vector2 mousePos) { }
	// 有物体拖动到了当前窗口上
	public virtual void onReceiveDrag(txUIObject dragObj)
	{
		if(mReceiveDragCallback != null)
		{
			mReceiveDragCallback(dragObj);
		}
	}
	// 有物体拖动到了当前窗口上
	public virtual void onDragHoverd(txUIObject dragObj, bool hover)
	{
		if (mDragHoverCallback != null)
		{
			mDragHoverCallback(dragObj, hover);
		}
	}
}