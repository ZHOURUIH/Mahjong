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
	PT_CS_HEART_BEAT,						// 向服务器发送的心跳
	PT_CS_REGISTER,							// 向服务器发送注册账号
	PT_CS_LOGIN,							// 向服务器发送登录请求
	PT_CS_CHECK_NAME,						// 向服务器请求判断名字是否已经存在
	PT_CS_CHECK_ACCOUNT,					// 向服务器请求判断账号是否已经存在
	PT_CS_CREATE_ROOM,						// 向服务器请求创建房间
	PT_CS_JOIN_ROOM,						// 向服务器请求加入房间
	PT_CS_MAX,

	// SC表示Server->Client
	PT_SC_MIN = 20000,
	PT_SC_HEART_BEAT_RET,					// 向客户端发回的心跳结果
	PT_SC_START_GAME,						// 向客户端发送的可以开始游戏的消息
	PT_SC_REGISTER_RET,						// 向客户端发回的注册账号的结果
	PT_SC_LOGIN_RET,						// 向客户端发回的登录结果	
	PT_SC_OTHER_PLAYER_OFFLINE,				// 通知客户端有其他玩家掉线
	PT_SC_CHECK_NAME_RET,					// 向客户端返回的检测名字的结果
	PT_SC_CHECK_ACCOUNT_RET,				// 向客户端返回的检测账号的结果
	PT_SC_CREATE_ROOM_RET,					// 向客户端返回创建房间的结果
	PT_SC_NOTIFY_BANKER,					// 通知客户端庄家变化
	PT_SC_OTHER_PLAYER_LEAVE_ROOM,			// 通知客户端有其他玩家离开房间
	PT_SC_OTHER_PLAYER_JOIN_ROOM,			// 通知客户端有其他玩家加入房间
	PT_SC_JOIN_ROOM_RET,					// 通知客户端加入房间的结果
	PT_SC_MAX,

	PT_MAX,
};