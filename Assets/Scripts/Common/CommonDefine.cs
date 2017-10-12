using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏枚举定义-----------------------------------------------------------------------------------------------
// UI物体类型
public enum UI_OBJECT_TYPE
{
	UBT_BASE,			// 窗口基类
	UBT_STATIC_SPRITE,	// 静态图片窗口,需要图集
	UBT_SPRITE_ANIM,	// 序列帧图片窗口,需要图集
	UBT_STATIC_TEXTURE,	// 静态图片窗口,不需要图集
	UBT_TEXTURE_ANIM,	// 序列帧图片窗口,不需要图集
	UBT_NUMBER,			// 数字窗口
	UBT_PARTICLE,		// 粒子特效窗口
	UBT_BUTTON,         // 按钮窗口
	UBT_SLIDER,			// 滑动条
	UBT_SCROLL_VIEW,	// 包含多个按钮的滚动条
	UBT_VIDEO,			// 用于播放视频的窗口
	UBT_TEXT,			// 文本
	UBT_EDITBOX,		// 文本编辑框
	UBT_PANEL,			// 面板
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
// 组件属性的类型
public enum PROPERTY_TYPE
{
	PT_BOOL,
	PT_INT,
	PT_STRING,
	PT_VECTOR2,
	PT_VECTOR3,
	PT_VECTOR4,
	PT_FLOAT,
	PT_ENUM,
	PT_TEXTURE,
	PT_DIM,
	PT_POINT,
};
// character 类型
public enum CHARACTER_TYPE
{
	CT_NORMAL,
	CT_NPC,
	CT_OTHER,
	CT_MYSELF,
	CT_MAX,
}

// 游戏委托定义-------------------------------------------------------------------------------------------------------------
public delegate void SpriteAnimCallBack(txUISpriteAnim window, object userData, bool isBreak);
public delegate void TextureAnimCallBack(txUITextureAnim window, object userData, bool isBreak);
public delegate void KeyFrameCallback(ComponentKeyFrame component, object userData, bool breakTremling, bool done);
public delegate void CommandCallback(object user_data, Command cmd);
public delegate void BoxColliderClickCallback(txUIButton obj);
public delegate void BoxColliderHoverCallback(txUIButton obj, bool hover);
public delegate void BoxColliderPressCallback(txUIButton obj, bool press);
public delegate void AssetLoadDoneCallback(UnityEngine.Object res, object userData);
public delegate void AssetBundleLoadDoneCallback(List<UnityEngine.Object> resList);
public delegate void LayoutAsyncDone(GameLayout layout);
public delegate void VideoPlayEndCallback(string videoName, bool isBreak);

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
	public const string LAYOUT_PREFAB = "LayoutPrefab";
	public const string LOWER_LAYOUT_PREFAB = "layoutprefab";
	public const string UI_PREFAB = "UIPrefab";
	public const string STREAMING_ASSETS = "StreamingAssets";
	public const string CONFIG = "Config";
	public const string VIDEO = "Video";
	public const string PARTICLE = "Particle";
	public const string HELPER_EXE = "HelperExe";
	public const string CUSTOM_SOUND = "CustomSound";
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
	public const string A_LAYOUT_PREFAB_PATH = A_LAYOUT_PATH + LAYOUT_PREFAB + "/";
	public const string A_BUNDLE_LAYOUT_PREFAB_PATH = A_BUNDLE_LAYOU_PATH + LAYOUT_PREFAB + "/";
	// 相对路径,相对于Resources,R_开头,表示Resources
	public const string R_SOUND_PATH = SOUND + "/";
	public const string R_LAYOUT_PATH = LAYOUT + "/";
	public const string R_KEY_FRAME_PATH = KEY_FRAME + "/";
	public const string R_LAYOUT_PREFAB_PATH = R_LAYOUT_PATH + LAYOUT_PREFAB + "/";
	public const string R_UI_PREFAB_PATH = R_LAYOUT_PATH + UI_PREFAB + "/";
	public const string R_TEXTURE_PATH = TEXTURE + "/";
	public const string R_GAME_TEXTURE_PATH = R_TEXTURE_PATH + GAME_TEXTURE + "/";
	public const string R_TEXTURE_ANIM_PATH = R_TEXTURE_PATH + TEXTURE_ANIM + "/";
	public const string R_MATERIAL_PATH = MATERIAL + "/";
	public const string R_PARTICLE_PATH = PARTICLE + "/";
	// 绝对路径,以F_开头,表示Full
	public static string F_ASSETS_PATH = Application.dataPath + "/";
	public static string F_STREAMING_ASSETS_PATH = F_ASSETS_PATH + STREAMING_ASSETS + "/";
	public static string F_VIDEO_PATH = F_STREAMING_ASSETS_PATH + VIDEO + "/";
	public static string F_CONFIG_PATH = F_STREAMING_ASSETS_PATH + CONFIG + "/";
	public static string F_GAME_DATA_FILE_PATH = F_STREAMING_ASSETS_PATH + GAME_DATA_FILE + "/";
	public static string F_HELPER_EXE_PATH = F_STREAMING_ASSETS_PATH + HELPER_EXE + "/";
	public static string F_CUSTOM_SOUND_PATH = F_STREAMING_ASSETS_PATH + CUSTOM_SOUND + "/";
	//-----------------------------------------------------------------------------------------------------------------
	// 常量定义
	// 常量数值定义
	public const uint WS_OVERLAPPED = 0x00000000;
	public const uint WS_POPUP = 0x80000000;
	public const uint WS_CHILD = 0x40000000;
	public const uint WS_MINIMIZE = 0x20000000;
	public const uint WS_VISIBLE = 0x10000000;
	public const uint WS_DISABLED = 0x08000000;
	public const uint WS_CLIPSIBLINGS = 0x04000000;
	public const uint WS_CLIPCHILDREN = 0x02000000;
	public const uint WS_MAXIMIZE = 0x01000000;
	public const uint WS_CAPTION = 0x00C00000;
	public const uint WS_BORDER = 0x00800000;
	public const uint WS_DLGFRAME = 0x00400000;
	public const uint WS_VSCROLL = 0x00200000;
	public const uint WS_HSCROLL = 0x00100000;
	public const uint WS_SYSMENU = 0x00080000;
	public const uint WS_THICKFRAME = 0x00040000;
	public const uint WS_GROUP = 0x00020000;
	public const uint WS_TABSTOP = 0x00010000;
	public const uint WS_MINIMIZEBOX = 0x00020000;
	public const uint WS_MAXIMIZEBOX = 0x00010000;
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
}