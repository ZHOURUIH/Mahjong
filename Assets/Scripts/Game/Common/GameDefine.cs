using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏枚举定义-----------------------------------------------------------------------------------------------
// 界面布局定义
public enum LAYOUT_TYPE
{
	LT_GLOBAL_TOUCH,
	LT_LOGIN,
	LT_REGISTER,
	LT_MAIN_LOADING,
	LT_MAIN_FRAME,
	LT_CHARACTER,
	LT_BILLBOARD,
	LT_ROOM_MENU,
	LT_MAHJONG_LOADING,
	LT_MAHJONG_HAND_IN,
	LT_MAHJONG_DROP,
	LT_ALL_CHARACTER_INFO,
	LT_DICE,
	LT_MAHJONG_BACK_FRAME,
	LT_PLAYER_ACTION,
	LT_GAME_ENDING,
	LT_ADD_PLAYER,
	LT_MAHJONG_FRAME,
	LT_JOIN_ROOM_DIALOG,
	LT_MESSAGE_OK,
	LT_FREE_MATCH_TIP,
	LT_BACK_TO_MAIN_HALL,
	LT_MAX,
};
// 音效定义
public enum SOUND_DEFINE
{
	SD_MIN = -1,
	SD_MAX,
};
// 场景的类型
public enum GAME_SCENE_TYPE
{
	GST_START,
	GST_MAIN,
	GST_MAHJONG,
	GST_MAX,
};
// 游戏场景流程类型
public enum PROCEDURE_TYPE
{
	PT_NONE,

	PT_START_MIN,
	PT_START_LOADING,
	PT_START_LOGIN,
	PT_START_REGISTER,
	PT_START_EXIT,
	PT_START_MAX,

	PT_MAIN_MIN,
	PT_MAIN_LOADING,
	PT_MAIN_MAIN_HALL,
	PT_MAIN_ROOM_LIST,
	PT_MAIN_EXIT,
	PT_MAIN_MAX,

	PT_MAHJONG_MIN,
	PT_MAHJONG_LOADING,
	PT_MAHJONG_WAITING,
	PT_MAHJONG_RUNNING,
	PT_MAHJONG_RUNNING_DICE,
	PT_MAHJONG_RUNNING_GET_START,
	PT_MAHJONG_RUNNING_GAMING,
	PT_MAHJONG_ENDING,
	PT_MAHJONG_EXIT,
	PT_MAHJONG_MAX,
};
// 游戏中的公共变量定义
public enum GAME_DEFINE_FLOAT
{
	GDF_NONE,
	// 应用程序配置参数
	GDF_APPLICATION_MIN,
	GDF_FULL_SCREEN,                // 是否全屏,0为窗口模式,1为全屏,2为无边框窗口
	GDF_SCREEN_WIDTH,               // 分辨率的宽
	GDF_SCREEN_HEIGHT,              // 分辨率的高
	GDF_ADAPT_SCREEN,               // 屏幕自适应的方式,0为基于锚点的自适应,可以根据不同分辨率调整布局排列,1为简单拉伸,2为多屏拼接后复制显示
	GDF_SCREEN_COUNT,               // 显示屏数量,用于多屏横向组合为高分辨率,只能在GDF_ADAPT_SCREEN为2的情况下使用
	GDF_USE_FIXED_TIME,             // 是否将每帧的时间固定下来
	GDF_FIXED_TIME,                 // 每帧的固定时间,单位秒
	GDF_VSYNC,                      // 垂直同步,0为关闭垂直同步,1为开启垂直同步
	GDF_APPLICATION_MAX,

	// 框架配置参数
	GDF_FRAME_MIN,
	GDF_SOCKET_PORT,                // socket端口
	GDF_BROADCAST_PORT,             // 广播端口
	GDF_LOAD_RESOURCES,             // 游戏加载资源的路径,0代表在Resources中读取,1代表从AssetBundle中读取
	GDF_LOG_LEVEL,                  // 日志输出等级
	GDF_ENABLE_KEYBOARD,            // 是否响应键盘按键
	GDF_FRAME_MAX,

	// 游戏配置参数
	GDF_GAME_MIN,
	GDF_HEART_BEAT_NITERVAL,        // 心跳间隔时间
	GDF_GAME_MAX,
};
public enum GAME_DEFINE_STRING
{
	GDS_NONE,
	// 应用程序配置参数
	GDS_APPLICATION_MIN,
	GDS_APPLICATION_MAX,

	// 框架配置参数
	GDS_FRAME_MIN,
	GDS_FRAME_MAX,

	// 游戏配置参数
	GDS_GAME_MIN,
	GDS_TCP_SERVER_IP,  // 服务器IP
	GDS_TCP_HOST_NAME,	// 服务器域名,域名和IP只需要填一个,都填则使用IP
	GDS_ACCOUNT,
	GDS_PASSWORD,
	GDS_GAME_MAX,
};
// 网络状态
public enum NET_STATE
{
	NS_CONNECTED,       // 已连接
	NS_SERVER_CLOSE,    // 服务器已关闭
	NS_NET_CLOSE,       // 网络已断开
}
// 表格数据类型
public enum DATA_TYPE
{
	DT_GAME_SOUND,
	DT_MAX,
}
public enum ACTION_TYPE
{
	AT_HU,
	AT_GANG,
	AT_PENG,
	AT_PASS,
	AT_MAX,
}
public enum MAHJONG
{
	// 9个筒
	M_TONG1,
	M_TONG2,
	M_TONG3,
	M_TONG4,
	M_TONG5,
	M_TONG6,
	M_TONG7,
	M_TONG8,
	M_TONG9,
	// 9个条
	M_TIAO1,
	M_TIAO2,
	M_TIAO3,
	M_TIAO4,
	M_TIAO5,
	M_TIAO6,
	M_TIAO7,
	M_TIAO8,
	M_TIAO9,
	// 9个万
	M_WAN1,
	M_WAN2,
	M_WAN3,
	M_WAN4,
	M_WAN5,
	M_WAN6,
	M_WAN7,
	M_WAN8,
	M_WAN9,
	// 7个风
	M_FENG_DONG,
	M_FENG_NAN,
	M_FENG_XI,
	M_FENG_BEI,
	M_FENG_ZHONG,
	M_FENG_FA,
	M_FENG_BAI,
	// 8个花牌
	M_HUA_CHUN,
	M_HUA_XIA,
	M_HUA_QIU,
	M_HUA_DONG,
	M_HUA_MEI,
	M_HUA_LAN,
	M_HUA_ZHU,
	M_HUA_JU,

	M_MAX,
}
public enum MAHJONG_HUASE
{
	MH_FENG,    // 风牌
	MH_TONG,    // 筒
	MH_TIAO,    // 条
	MH_WAN,     // 万
	MH_HUA,     // 花
	MH_MAX,
}
public enum PLAYER_POSITION
{
	PP_MYSELF,      // 玩家自己
	PP_LEFT,        // 左边的玩家
	PP_OPPOSITE,    // 对面的玩家
	PP_RIGHT,       // 右边的玩家
	PP_MAX,
}
// 胡牌类型
public enum HU_TYPE
{
	HT_NORMAL,      // 平胡
	HT_QINGYISE,    // 清一色
	HT_QUESE,       // 缺一门
	HT_HUA,         // 花牌
	HT_GANG,        // 杠牌
	HT_ANGANG,      // 暗杠
	HT_MENQING,     // 门清
	HT_DUIDUIHU,    // 对对胡
	HT_ANQIDUI,     // 暗七对
	HT_LONGQIDUI,   // 龙七对
	HT_GANGSHANGHUA,// 杠上花
	HT_GANGSHANGPAO,// 杠上炮
	HT_HAIDIHUA,    // 海底花
	HT_HAIDIPAO,    // 海底炮
	HT_TIANHU,      // 天胡
	HT_MAX,
}
// 游戏常量定义-------------------------------------------------------------------------------------------------------------
public class GameDefine : CommonDefine
{
	// 路径定义
	//-----------------------------------------------------------------------------------------------------------------
	// 常量定义
	//-----------------------------------------------------------------------------------------------------------------
	// 每名玩家手里最多有14张牌,不包含碰,吃,杠
	public const int MAX_HAND_IN_COUNT = 14;
	// 花牌的最大数量
	public const int MAX_HUA_COUNT = MAHJONG.M_HUA_JU - MAHJONG.M_HUA_CHUN + 1;
	// 可以碰或者杠的最大次数
	public const int MAX_PENG_TIMES = MAX_HAND_IN_COUNT / 3;
	// 一局麻将游戏中玩家的最大数量
	public const int MAX_PLAYER_COUNT = (int)PLAYER_POSITION.PP_MAX;
	// 每个麻将的最大数量
	public const int MAX_SINGLE_COUNT = 4;
	// 一局中所有麻将的最大数量
	public const int MAX_MAHJONG_COUNT = (int)MAHJONG.M_MAX * MAX_SINGLE_COUNT;
	// 骰子的个数
	public const int DICE_COUNT = 2;
	// 每个骰子的最大值,骰子值的范围从0到MAX_DICE_VALUE
	public const int MAX_DICE_VALUE = 5;
	// 开局时发每一张牌的时间间隔
	public const float ASSIGN_MAHJONG_INTERVAL = 0.1f;
	// 无效ID值
	public const int INVALID_ID = ~0;
	// 胡牌类型的最大数量
	public const int MAX_HU_COUNT = 16;
	// 所有麻将的资源名字
	public static string[] MAHJONG_NAME = new string[(int)MAHJONG.M_MAX]
	{
		"Tong0", "Tong1", "Tong2", "Tong3", "Tong4", "Tong5", "Tong6", "Tong7", "Tong8",
		"Tiao0", "Tiao1", "Tiao2", "Tiao3", "Tiao4", "Tiao5", "Tiao6", "Tiao7", "Tiao8",
		"Wan0", "Wan1", "Wan2", "Wan3", "Wan4", "Wan5", "Wan6", "Wan7", "Wan8",
		"Feng0", "Feng1", "Feng2", "Feng3", "Feng4", "Feng5", "Feng6",
		"Hua0","Hua1","Hua2","Hua3","Hua4","Hua5","Hua6","Hua7",
	};
	// 所有胡牌类型的名字,必须与枚举一一对应
	public static string[] HU_NAME = new string[(int)HU_TYPE.HT_MAX]
	{
		"平胡", "清一色", "缺一门", "花牌", "杠牌", "暗杠", "门清", "对对胡",
		"暗七对", "龙七对", "杠上花", "杠上炮", "海底花", "海底炮", "天胡"
	};
	public static int[] HU_MULTIPLE = new int[(int)HU_TYPE.HT_MAX]
	{
		1, 4, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2
	};
	public static string[] mDropMahjongPreName = new string[MAX_PLAYER_COUNT] { "Drop_My_", "Drop_Side_", "Drop_Opposite_", "Drop_Side_" };
}