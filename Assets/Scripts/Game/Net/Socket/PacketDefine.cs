using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 作为客户端时接收以及发送的类型
public enum PACKET_TYPE
{
	PT_MIN,

	// CS表示Client->Server
	PT_CS_MIN = 10000,
	PT_CS_HEART_BEAT,                       // 向服务器发送的心跳
	PT_CS_REGISTER,                         // 向服务器发送注册账号
	PT_CS_LOGIN,                            // 向服务器发送登录请求
	PT_CS_CHECK_NAME,                       // 向服务器请求判断名字是否已经存在
	PT_CS_CHECK_ACCOUNT,                    // 向服务器请求判断账号是否已经存在
	PT_CS_CREATE_ROOM,                      // 向服务器请求创建房间
	PT_CS_JOIN_ROOM,                        // 向服务器请求加入房间
	PT_CS_READY,                            // 向服务器发送是否已准备
	PT_CS_LEAVE_ROOM,                       // 向服务器请求离开房间
	PT_CS_DICE_DONE,                        // 向服务器发送骰子
	PT_CS_REQUEST_DROP,                     // 向服务器请求打出一张牌
	PT_CS_CONFIRM_ACTION,                   // 向服务器确认麻将操作
	PT_CS_CONTINUE_GAME,                    // 向服务器请求继续游戏
	PT_CS_BACK_TO_MAHJONG_HALL,             // 向服务器请求返回大厅
	PT_CS_ADD_MAHJONG_ROBOT,                // 向服务器请求添加机器人打麻将,用作测试
	PT_CS_MAX,

	// SC表示Server->Client
	PT_SC_MIN = 20000,
	PT_SC_HEART_BEAT_RET,                   // 向客户端发回的心跳结果
	PT_SC_START_GAME,                       // 向客户端发送的可以开始游戏的消息
	PT_SC_REGISTER_RET,                     // 向客户端发回的注册账号的结果
	PT_SC_LOGIN_RET,                        // 向客户端发回的登录结果	
	PT_SC_OTHER_PLAYER_OFFLINE,             // 通知客户端有其他玩家掉线
	PT_SC_CHECK_NAME_RET,                   // 向客户端返回的检测名字的结果
	PT_SC_CHECK_ACCOUNT_RET,                // 向客户端返回的检测账号的结果
	PT_SC_CREATE_ROOM_RET,                  // 向客户端返回创建房间的结果
	PT_SC_NOTIFY_BANKER,                    // 通知客户端庄家变化
	PT_SC_OTHER_PLAYER_LEAVE_ROOM,          // 通知客户端有其他玩家离开房间
	PT_SC_OTHER_PLAYER_JOIN_ROOM,           // 通知客户端有其他玩家加入房间
	PT_SC_JOIN_ROOM_RET,                    // 通知客户端加入房间的结果
	PT_SC_READY_RET,                        // 通知客户端玩家是否准备
	PT_SC_OTHER_PLAYER_READY,               // 通知客户端有其他玩家准备
	PT_SC_LEAVE_ROOM_RET,                   // 通知客户端离开房间的结果
	PT_SC_DICE_DONE_RET,                    // 通知客户端掷骰子完毕,开始拿牌
	PT_SC_NOTIFY_GET_START_MAHJONG,         // 通知客户端有玩家开局拿牌
	PT_SC_NOTIFY_REORDER_MAHJONG,           // 通知客户端重新排列麻将
	PT_SC_NOTIFY_GET_START_DONE,            // 通知客户端开局拿牌完毕
	PT_SC_ASK_DROP,                         // 通知客户端需要打出一张牌
	PT_SC_NOTIFY_GET_MAHJONG,               // 通知客户端有玩家摸了一张牌
	PT_SC_ASK_ACTION,                       // 通知客户端选择麻将行为
	PT_SC_OTHER_PLAYER_DROP,                // 通知客户端有其他玩家打出一张牌
	PT_SC_REQUEST_DROP_RET,                 // 通知客户端请求打出一张牌的结果
	PT_SC_PLAYER_HU,                        // 通知客户端胡牌
	PT_SC_PLAYER_GANG,                      // 通知客户端杠牌
	PT_SC_PLAYER_PENG,                      // 通知客户端碰牌
	PT_SC_PLAYER_PASS,                      // 通知客户端过
	PT_SC_OTHER_PLAYER_GANG,                // 通知客户端其他玩家杠牌
	PT_SC_OTHER_PLAYER_PENG,                // 通知客户端其他玩家碰牌
	PT_SC_OTHER_PLAYER_PASS,                // 通知客户端其他玩家过
	PT_SC_OTHER_PLAYER_ASK_DROP,            // 通知客户端等待其他玩家打出牌
	PT_SC_OTHER_PLAYER_ASK_ACTION,          // 通知客户端等待其他玩家选择操作
	PT_SC_NOTIFY_MAHJONG_END,               // 通知客户端本局游戏结束
	PT_SC_CONTINUE_GAME_RET,                // 通知客户端继续游戏的结果
	PT_SC_OTHER_PLAYER_CONTINUE_GAME,       // 通知客户端有其他玩家选择继续游戏
	PT_SC_BACK_TO_MAHJONG_HALL_RET,         // 通知客户端返回大厅的结果
	PT_SC_OTHER_PLAYER_BACK_TO_MAHJONG_HALL,// 通知客户端有其他玩家选择返回大厅
	PT_SC_SHOW_HUA,                         // 通知客户端摆出花牌
	PT_SC_OTHER_PLAYER_SHOW_HUA,            // 通知客户端有其他玩家摆出花牌
	PT_SC_ADD_MAHJONG_ROBOT_RET,            // 通知客户端添加麻将机器人的结果
	PT_SC_MAX,

	PT_MAX,
};