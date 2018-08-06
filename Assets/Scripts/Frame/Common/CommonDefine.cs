using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏枚举定义-----------------------------------------------------------------------------------------------
// UI物体类型
public enum UI_TYPE
{
	UT_BASE,				// 窗口基类
	UT_PARTICLE,			// 粒子特效窗口
	// NGUI
	UT_NGUI_STATIC_SPRITE,	// 静态图片窗口,需要图集
	UT_NGUI_SPRITE_ANIM,	// 序列帧图片窗口,需要图集
	UT_NGUI_STATIC_TEXTURE,	// 静态图片窗口,不需要图集
	UT_NGUI_TEXTURE_ANIM,	// 序列帧图片窗口,不需要图集
	UT_NGUI_NUMBER,			// 数字窗口
	UT_NGUI_BUTTON,         // 按钮窗口
	UT_NGUI_BUTTON_SPRITE,         // 带图片的按钮窗口,需要图集
	UT_NGUI_BUTTON_SPRITE_ANIM,    // 带序列帧的按钮窗口,需要图集
	UT_NGUI_BUTTON_TEXTURE,        // 带图片的按钮窗口,不需要图集
	UT_NGUI_BUTTON_TEXTURE_ANIM,   // 带序列帧的按钮窗口,不需要图集
	UT_NGUI_SLIDER,			// 滑动条
	UT_NGUI_SCROLL_VIEW,	// 包含多个按钮的滚动条
	UT_NGUI_VIDEO,			// 用于播放视频的窗口
	UT_NGUI_TEXT,			// 文本
	UT_NGUI_EDITBOX,        // 文本编辑框
	UT_NGUI_PANEL,          // 面板
	UI_NGUI_DRAG_VIEW,		// 可拖动的窗口
	// UGUI
	UT_UGUI_STATIC_IMAGE,   // 静态图片
	UT_UGUI_CANVAS,			// 画布
	UT_UGUI_NUMBER,			// 数字
    UT_UGUI_TEXT,           // 文本
	
}
// 停靠位置
public enum DOCKING_POSITION
{
	DP_LEFT,
	DP_CENTER,
	DP_RIGHT,
}
// 循环方式
public enum LOOP_MODE
{
	LM_ONCE,
	LM_LOOP,
	LM_PINGPONG,
}
// 播放状态
public enum PLAY_STATE
{
	PS_NONE,
	PS_PLAY,
	PS_PAUSE,
	PS_STOP,
}
// 音效所有者类型
public enum SOUND_OWNER
{
	SO_WINDOW,
	SO_SCENE,
}
// character 类型
public enum CHARACTER_TYPE
{
	CT_NORMAL,
	CT_AI,
	CT_OTHER,
	CT_MYSELF,
	CT_MAX,
}

// 屏幕适配方式
public enum ADAPT_SCREEN
{
	AS_BASE_ON_ANCHOR,	// 基于NGUI锚点的自适应
	AS_SIMPLE_STRETCH,	// 简单拉伸
	AS_MULTI_SCREEN,	// 多屏拼接后复制显示
}

// 游戏委托定义-------------------------------------------------------------------------------------------------------------
public delegate void SpriteAnimCallBack(txNGUISpriteAnim window, object userData, bool isBreak);
public delegate void TextureAnimCallBack(txNGUITextureAnim window, object userData, bool isBreak);
public delegate void KeyFrameCallback(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done);
public delegate void CommandCallback(object user_data, Command cmd);
public delegate void BoxColliderClickCallback(txUIObject obj);
public delegate void BoxColliderHoverCallback(txUIObject obj, bool hover);
public delegate void BoxColliderPressCallback(txUIObject obj, bool press);
public delegate void AssetLoadDoneCallback(UnityEngine.Object res, object userData);
public delegate void SceneLoadCallback(float progress, bool done, object userData);
public delegate void SceneActiveCallback(object userData);
public delegate void AssetBundleLoadDoneCallback(List<UnityEngine.Object> resList);
public delegate void LayoutAsyncDone(GameLayout layout);
public delegate void VideoCallback(string videoName, bool isBreak);
public delegate void TrackDoneCallback(ComponentTrackTargetBase component);
public delegate Vector3 CheckPosition(txUIObject obj);

// 游戏常量定义-------------------------------------------------------------------------------------------------------------
public class CommonDefine
{
	// 路径定义
	// 文件夹名
	public const string ASSETS = "Assets";
	public const string RESOURCES = "Resources";
	public const string ATLAS = "Atlas";
	public const string FONT = "Font";
	public const string GAME_DATA_FILE = "GameDataFile";
	public const string KEY_FRAME = "KeyFrame";
	public const string LOWER_KEY_FRAME = "keyframe";
	public const string LAYOUT = "Layout";
	public const string LOWER_LAYOUT = "layout";
	public const string SCENE = "Scene";
	public const string SHADER = "Shader";
	public const string SKYBOX = "Skybox";
	public const string SOUND = "Sound";
	public const string GAME_SOUND = "GameSound";
	public const string MATERIAL = "Material";
	public const string TEXTURE = "Texture";
	public const string GAME_ATLAS = "GameAtlas";
	public const string GAME_TEXTURE = "GameTexture";
	public const string NUMBER_STYLE = "NumberStyle";
	public const string TEXTURE_ANIM = "TextureAnim";
	public const string NGUI_SUB_PREFAB = "NGUISubPrefab";
	public const string UGUI_SUB_PREFAB = "UGUISubPrefab";
	public const string NGUI_PREFAB = "NGUIPrefab";
	public const string UGUI_PREFAB = "UGUIPrefab";
	public const string STREAMING_ASSETS = "StreamingAssets";
	public const string CONFIG = "Config";
	public const string VIDEO = "Video";
	public const string PARTICLE = "Particle";
	public const string HELPER_EXE = "HelperExe";
	public const string CUSTOM_SOUND = "CustomSound";
	public const string DATA_BASE = "DataBase";
	public const string MODEL = "Model";
	public const string GAME_PLUGIN = "GamePlugin";
	// 相对路径,相对于项目,以P_开头,表示Project
	public const string P_ASSETS_PATH = ASSETS + "/";
	public const string P_RESOURCE_PATH = P_ASSETS_PATH + RESOURCES + "/";
	// 相对路径,相对于Assets,以A_开头,表示Assets
	public const string A_RESOURCE_PATH = RESOURCES + "/";
	public const string A_STREAMING_ASSETS_PATH = STREAMING_ASSETS + "/";
	public const string A_CONFIG_PATH = A_STREAMING_ASSETS_PATH + CONFIG + "/";
	public const string A_VIDEO_PATH = A_STREAMING_ASSETS_PATH + VIDEO + "/";
	public const string A_GAME_DATA_FILE_PATH = A_STREAMING_ASSETS_PATH + GAME_DATA_FILE + "/";
	public const string A_BUNDLE_KEY_FRAME_PATH = A_STREAMING_ASSETS_PATH + LOWER_KEY_FRAME + "/";
	public const string A_BUNDLE_LAYOU_PATH = A_STREAMING_ASSETS_PATH + LOWER_LAYOUT + "/";
	public const string A_CUSTOM_SOUND_PATH = A_STREAMING_ASSETS_PATH + CUSTOM_SOUND + "/";
	public const string A_SOUND_PATH = A_RESOURCE_PATH + SOUND + "/";
	public const string A_KEY_FRAME_PATH = A_RESOURCE_PATH + KEY_FRAME + "/";
	public const string A_LAYOUT_PATH = A_RESOURCE_PATH + LAYOUT + "/";
	public const string A_NGUI_SUB_PREFAB_PATH = A_LAYOUT_PATH + NGUI_SUB_PREFAB + "/";
	public const string A_BUNDLE_NGUI_SUB_PREFAB_PATH = A_BUNDLE_LAYOU_PATH + NGUI_SUB_PREFAB + "/";
	public const string A_GAME_PLUGIN = GAME_PLUGIN + "/";
	// 相对路径,相对于Resources,R_开头,表示Resources
	public const string R_SOUND_PATH = SOUND + "/";
	public const string R_LAYOUT_PATH = LAYOUT + "/";
	public const string R_KEY_FRAME_PATH = KEY_FRAME + "/";
	public const string R_NGUI_SUB_PREFAB_PATH = R_LAYOUT_PATH + NGUI_SUB_PREFAB + "/";
	public const string R_UGUI_SUB_PREFAB_PATH = R_LAYOUT_PATH + UGUI_SUB_PREFAB + "/";
	public const string R_NGUI_PREFAB_PATH = R_LAYOUT_PATH + NGUI_PREFAB + "/";
	public const string R_UGUI_PREFAB_PATH = R_LAYOUT_PATH + UGUI_PREFAB + "/";
	public const string R_TEXTURE_PATH = TEXTURE + "/";
	public const string R_GAME_TEXTURE_PATH = R_TEXTURE_PATH + GAME_TEXTURE + "/";
	public const string R_TEXTURE_ANIM_PATH = R_TEXTURE_PATH + TEXTURE_ANIM + "/";
	public const string R_NUMBER_STYLE_PATH = R_TEXTURE_PATH + NUMBER_STYLE + "/";
	public const string R_MATERIAL_PATH = MATERIAL + "/";
	public const string R_PARTICLE_PATH = PARTICLE + "/";
	public const string R_MODEL_PATH = MODEL + "/";
	// 绝对路径,以F_开头,表示Full
	public static string F_ASSETS_PATH = Application.dataPath + "/";
	public static string F_STREAMING_ASSETS_PATH = F_ASSETS_PATH + STREAMING_ASSETS + "/";
	public static string F_DATA_BASE_PATH = F_STREAMING_ASSETS_PATH + DATA_BASE + "/";
	public static string F_VIDEO_PATH = F_STREAMING_ASSETS_PATH + VIDEO + "/";
	public static string F_CONFIG_PATH = F_STREAMING_ASSETS_PATH + CONFIG + "/";
	public static string F_GAME_DATA_FILE_PATH = F_STREAMING_ASSETS_PATH + GAME_DATA_FILE + "/";
	public static string F_HELPER_EXE_PATH = F_STREAMING_ASSETS_PATH + HELPER_EXE + "/";
	public static string F_CUSTOM_SOUND_PATH = F_STREAMING_ASSETS_PATH + CUSTOM_SOUND + "/";
	//-----------------------------------------------------------------------------------------------------------------
	// 常量定义
	// 常量数值定义
	public const long WS_OVERLAPPED = 0x00000000;
	public const long WS_POPUP = 0x80000000;
	public const long WS_CHILD = 0x40000000;
	public const long WS_MINIMIZE = 0x20000000;
	public const long WS_VISIBLE = 0x10000000;
	public const long WS_DISABLED = 0x08000000;
	public const long WS_CLIPSIBLINGS = 0x04000000;
	public const long WS_CLIPCHILDREN = 0x02000000;
	public const long WS_MAXIMIZE = 0x01000000;
	public const long WS_BORDER = 0x00800000;
	public const long WS_DLGFRAME = 0x00400000;
	public const long WS_CAPTION = WS_BORDER | WS_DLGFRAME;
	public const long WS_VSCROLL = 0x00200000;
	public const long WS_HSCROLL = 0x00100000;
	public const long WS_SYSMENU = 0x00080000;
	public const long WS_THICKFRAME = 0x00040000;
	public const long WS_GROUP = 0x00020000;
	public const long WS_TABSTOP = 0x00010000;
	public const long WS_MINIMIZEBOX = 0x00020000;
	public const long WS_MAXIMIZEBOX = 0x00010000;
	public const int GWL_STYLE = -16;
	// UI的制作标准,所有UI都是按1920*1080标准分辨率制作的
	public const int STANDARD_WIDTH = 1920;
	public const int STANDARD_HEIGHT = 1080;
	//-----------------------------------------------------------------------------------------------------------------
	// 表格数据文件后缀名
	public const string DATA_SUFFIX = ".bytes";
	public const string ASSET_BUNDLE_SUFFIX = ".unity3d";
	// 常用关键帧名定义
	public const string ONE_ZERO = "OneZero";
	public const string ONE_ZERO_ONE = "OneZeroOne";
	public const string ONE_ZERO_ONE_CURVE = "OneZeroOne_Curve";
	public const string QUADRATIC_CURVE = "Quadratic_Curve";
	public const string SIN_CURVE = "Sin_Curve";
	public const string ZERO_ONE = "ZeroOne";
	public const string ZERO_ONE_ZERO = "ZeroOneZero";
	public const string ZERO_ONE_DELAY = "ZeroOne_Delay";
	// 音效所有者类型名,应该与SOUND_OWNER一致
	public static string[] SOUND_OWNER_NAME = new string[] { "Window", "Scene" };
	public const string NGUI_DEFAULT_MATERIAL = "NGUIDefault";
	public const string UGUI_DEFAULT_MATERIAL = "UGUIDefault";
}