using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public interface INGUIAnimation
{
	string getTextureSet();
	int getTextureFrameCount();
	void setTexturePosList(List<Vector2> posList);
	void setUseTextureSize(bool useSize, bool makeEvenSize);
	void setTextureSet(string textureSetName);
	void setTextureSet(string textureSetName, string subPath);
	LOOP_MODE getLoop();
	float getInterval();
	float getSpeed();
	int getStartIndex();
	PLAY_STATE getPlayState();
	bool getPlayDirection();
	int getEndIndex();
	bool getAutoHide();
	int getRealEndIndex();
	void setLoop(LOOP_MODE loop);
	void setInterval(float interval);
	void setPlayDirection(bool direction);
	void setAutoHide(bool autoHide);
	void setStartIndex(int startIndex);
	void setEndIndex(int endIndex);
	void setSpeed(float speed);
	void stop(bool resetStartIndex = true, bool callback = true, bool isBreak = true);
	void play();
	void pause();
	void addPlayEndCallback(TextureAnimCallBack callback, bool clear = true);
	void addPlayingCallback(TextureAnimCallBack callback);
	int getCurFrameIndex();
	void setCurFrameIndex(int index);
}