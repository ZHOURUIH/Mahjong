#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
	#define UNITY_PLATFORM_SUPPORTS_LINEAR
#elif UNITY_IOS || UNITY_ANDROID
	#if UNITY_5_5_OR_NEWER || (UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_4)
		#define UNITY_PLATFORM_SUPPORTS_LINEAR
	#endif
#endif

using UnityEngine;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.Storage.Streams;
#endif

//-----------------------------------------------------------------------------
// Copyright 2015-2017 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	[System.Serializable]
	public class MediaPlayerEvent : UnityEngine.Events.UnityEvent<MediaPlayer, MediaPlayerEvent.EventType, ErrorCode>
	{
		public enum EventType
		{
			MetaDataReady,		// Called when meta data(width, duration etc) is available
			ReadyToPlay,		// Called when the video is loaded and ready to play
			Started,			// Called when the playback starts
			FirstFrameReady,	// Called when the first frame has been rendered
			FinishedPlaying,	// Called when a non-looping video has finished playing
			Closing,			// Called when the media is closed
			Error,				// Called when an error occurs
			SubtitleChange,		// Called when the subtitles change
			Stalled,			// Called when media is stalled (eg. when lost connection to media stream)
			Unstalled			// Called when media is resumed form a stalled state (eg. when lost connection is re-established)

			// TODO: 
			//FinishedSeeking,	// Called when seeking has finished
			//StartLoop,			// Called when the video starts and is in loop mode
			//EndLoop,			// Called when the video ends and is in loop mode
		}
	}

	public interface IMediaPlayer
	{
		void OnEnable();
		void Update();
		void Render();
	}

	public interface IMediaSubtitles
	{
		bool LoadSubtitlesSRT(string data);
		int GetSubtitleIndex();
		string GetSubtitleText();
	}

	public interface IMediaControl
	{
		// TODO: CanPreRoll() PreRoll()
		// TODO: audio panning

		/// <summary>
		/// Be careful using this method directly.  It is best to instead use the OpenVideoFromFile() method in the MediaPlayer component as this will set up the events correctly and also perform other checks
		/// </summary>
		bool	OpenVideoFromFile(string path, long offset, string httpHeaderJson);

#if NETFX_CORE
		bool OpenVideoFromFile(IRandomAccessStream ras, string path, long offset, string httpHeaderJson);
#endif

		void CloseVideo();

		void	SetLooping(bool bLooping);
		bool	IsLooping();

		bool	HasMetaData();
		bool	CanPlay();
		bool	IsPlaying();
		bool	IsSeeking();
		bool	IsPaused();
		bool	IsFinished();
		bool	IsBuffering();

		void	Play();
		void	Pause();
		void	Stop();
		void	Rewind();

		void	Seek(float timeMs);
		void	SeekFast(float timeMs);
		float	GetCurrentTimeMs();

		float	GetPlaybackRate();
		void	SetPlaybackRate(float rate);

		void	MuteAudio(bool bMute);
		bool	IsMuted();
		void	SetVolume(float volume);
		void	SetBalance(float balance);
		float	GetVolume();
		float	GetBalance();

		int		GetCurrentAudioTrack();
		void	SetAudioTrack(int index);

		int		GetCurrentVideoTrack();
		void	SetVideoTrack(int index);

		float	GetBufferingProgress();
		int		GetBufferedTimeRangeCount();
		bool	GetBufferedTimeRange(int index, ref float startTimeMs, ref float endTimeMs);

		ErrorCode GetLastError();

		void	SetTextureProperties(FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, int anisoLevel = 1);

		void	GrabAudio(float[] buffer, int floatCount, int channelCount);
	}

	public interface IMediaInfo
	{
		/// <summary>
		/// Returns media duration in milliseconds
		/// </summary>
		float	GetDurationMs();

		/// <summary>
		/// Returns video width in pixels
		/// </summary>
		int		GetVideoWidth();

		/// <summary>
		/// Returns video height in pixels
		/// </summary>
		int		GetVideoHeight();

		/// <summary>
		/// Returns the frame rate of the media.
		/// </summary>
		float	GetVideoFrameRate();

		/// <summary>
		/// Returns the current achieved display rate in frames per second
		/// </summary>
		float	GetVideoDisplayRate();

		/// <summary>
		/// Returns true if the media has a visual track
		/// </summary>
		bool	HasVideo();

		/// <summary>
		/// Returns true if the media has a audio track
		/// </summary>
		bool	HasAudio();

		/// <summary>
		/// Returns the number of audio tracks contained in the media
		/// </summary>
		int GetAudioTrackCount();

		/// <summary>
		/// Returns the current audio track identification
		/// </summary>
		string GetCurrentAudioTrackId();

		/// <summary>
		/// Returns the current audio track bitrate
		/// </summary>
		int GetCurrentAudioTrackBitrate();

		/// <summary>
		/// Returns the number of video tracks contained in the media
		/// </summary>
		int GetVideoTrackCount();

		/// <summary>
		/// Returns the current video track identification
		/// </summary>
		string GetCurrentVideoTrackId();

		/// <summary>
		/// Returns the current video track bitrate
		/// </summary>
		int GetCurrentVideoTrackBitrate();

		/// <summary>
		/// Returns the a description of which playback path is used internally.
		/// This can for example expose whether CPU or GPU decoding is being performed
		/// For Windows the available player descriptions are:
		///		"DirectShow" - legacy Microsoft API but still very useful especially with modern filters such as LAV
		///		"MF-MediaEngine-Software" - uses the Windows 8.1 features of the Microsoft Media Foundation API, but software decoding
		///		"MF-MediaEngine-Hardware" - uses the Windows 8.1 features of the Microsoft Media Foundation API, but GPU decoding
		///	Android just has "MediaPlayer"
		///	macOS / tvOS / iOS just has "AVfoundation"
		/// </summary>
		string GetPlayerDescription();

		/// <summary>
		/// Whether this MediaPlayer instance supports linear color space
		/// If it doesn't then a correction may have to be made in the shader
		/// </summary>
		bool PlayerSupportsLinearColorSpace();

		/// <summary>
		/// Checks if the playback is in a stalled state
		/// </summary>
		bool IsPlaybackStalled();

		/// <summary>
		/// The affine transform of the texture as an array of six floats: a, b, c, d, tx, ty.
		/// </summary>
		float[] GetTextureTransform();

		/*
		string GetMediaDescription();
		string GetVideoDescription();
		string GetAudioDescription();*/
	}

	public interface IMediaProducer
	{
		/// <summary>
		/// Gets the number of textures produced by the media player.
		/// </summary>
		int GetTextureCount();

		/// <summary>
		/// Returns the Unity texture containing the current frame image.
		/// The texture pointer will return null while the video is loading
		/// This texture usually remains the same for the duration of the video.
		/// There are cases when this texture can change, for instance: if the graphics device is recreated,
		/// a new video is loaded, or if an adaptive stream (eg HLS) is used and it switches video streams.
		/// </summary>
		Texture GetTexture(int index = 0);

		/// <summary>
		/// Returns a count of how many times the texture has been updated
		/// </summary>
		int GetTextureFrameCount();

		/// <summary>
		/// Returns the presentation time stamp of the current texture
		/// </summary>
		long GetTextureTimeStamp();

		/// <summary>
		/// Returns true if the image on the texture is upside-down
		/// </summary>
		bool RequiresVerticalFlip();
	}

	// TODO: complete this?
	public interface IMediaConsumer
	{
	}

	public enum Platform
	{
		Windows,
		MacOSX,
		iOS,
		tvOS,
		Android,
		WindowsPhone,
		WindowsUWP,
		WebGL,
		Count = 8,
		Unknown = 100,
	}

	public enum StereoPacking
	{
		None,
		TopBottom,				// Top is the left eye, bottom is the right eye
		LeftRight,              // Left is the left eye, right is the right eye
		CustomUV,				// Custom packing, use the mesh UV to unpack, uv0=left eye, uv1=right eye
	}

	public enum AlphaPacking
	{
		None,
		TopBottom,
		LeftRight,
	}

	public enum ErrorCode
	{
		None = 0,
		LoadFailed = 100,
		DecodeFailed = 200,
	}

	public enum Orientation
	{
		Landscape,				// Landscape Right (0 degrees)
		LandscapeFlipped,		// Landscape Left (180 degrees)
		Portrait,				// Portrait Up (90 degrees)
		PortraitFlipped,		// Portrait Down (-90 degrees)
	}

	public static class Windows
	{
		public enum VideoApi
		{
			MediaFoundation,
			DirectShow,
		};

		// WIP: Experimental feature to allow overriding audio device for VR headsets
		public const string AudioDeviceOutputName_Vive = "HTC VIVE USB Audio";
		public const string AudioDeviceOutputName_Rift = "Rift Audio";
	}

	public class Subtitle
	{
		public int index;
		public string text;
		public int timeStartMs, timeEndMs;

		public bool IsBefore(float time)
		{
			return (time > timeStartMs && time > timeEndMs);
		}

		public bool IsTime(float time)
		{
			return (time >= timeStartMs && time < timeEndMs);
		}
	}

	public static class Helper
	{
		public const string ScriptVersion = "1.6.6";

		public static string GetName(Platform platform)
		{
			string result = "Unknown";
			/*switch (platform)
			{
				case Platform.Windows:
					break;
			}*/
			result = platform.ToString();
			return result;
		}

		public static string GetErrorMessage(ErrorCode code)
		{
			string result = string.Empty;
			switch (code)
			{
				case ErrorCode.None:
					result = "No Error";
					break;
				case ErrorCode.LoadFailed:
					result = "Loading failed.  Codec not supported or video resolution too high or insufficient system resources.";
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
					// Add extra information for older Windows versions that don't have support for modern codecs
					if (SystemInfo.operatingSystem.StartsWith("Windows XP") ||
						SystemInfo.operatingSystem.StartsWith("Windows Vista"))
					{
						result += " NOTE: Windows XP and Vista don't have native support for H.264 codec.  Consider using an older codec such as DivX or installing 3rd party codecs such as LAV Filters.";
					}
#endif
					break;
				case ErrorCode.DecodeFailed:
					result = "Decode failed.  Possible codec not supported, video resolution too high or insufficient system resources.";
#if UNITY_ANDROID
					result += " On Android this is generally due to the hardware not having enough resources to decode the video. Most Android devices can only handle a maximum of one 4K video at once.";
#endif
					break;
			}
			return result;
		}

		public static string[] GetPlatformNames()
		{
			return new string[] {
				GetName(Platform.Windows),
				GetName(Platform.MacOSX),
				GetName(Platform.iOS),
				GetName(Platform.tvOS),
				GetName(Platform.Android),
				GetName(Platform.WindowsPhone),
				GetName(Platform.WindowsUWP),
				GetName(Platform.WebGL),
			};
		}

#if AVPROVIDEO_DISABLE_LOGGING
		[System.Diagnostics.Conditional("ALWAYS_FALSE")]
#endif
		public static void LogInfo(string message, Object context = null)
		{
			if (context == null)
			{
				Debug.Log("[AVProVideo] " + message);
			}
			else
			{
				Debug.Log("[AVProVideo] " + message, context);
			}
		}

		public static string GetTimeString(float totalSeconds, bool showMilliseconds = false)
		{
			int hours = Mathf.FloorToInt(totalSeconds / (60f * 60f));
			float usedSeconds = hours * 60f * 60f;

			int minutes = Mathf.FloorToInt((totalSeconds - usedSeconds) / 60f);
			usedSeconds += minutes * 60f;

			int seconds = Mathf.FloorToInt(totalSeconds - usedSeconds);

			string result;
			if (hours <= 0)
			{
				if (showMilliseconds)
				{
					int milliSeconds = (int)((totalSeconds - Mathf.Floor(totalSeconds)) * 1000f);
					result = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);
				}
				else
				{
					result = string.Format("{0:00}:{1:00}", minutes, seconds);
				}
			}
			else
			{
				if (showMilliseconds)
				{
					int milliSeconds = (int)((totalSeconds - Mathf.Floor(totalSeconds)) * 1000f);
					result = string.Format("{2}:{0:00}:{1:00}:{3:000}", minutes, seconds, hours, milliSeconds);
				}
				else
				{
					result = string.Format("{2}:{0:00}:{1:00}", minutes, seconds, hours);
				}
			}

			return result;
		}

		/// <summary>
		/// Convert texture transform matrix to an enum of orientation types
		/// </summary>
		public static Orientation GetOrientation(float[] t)
		{
			Orientation result = Orientation.Landscape;
			if (t[0] == 0f && t[1]== 1f && t[2] == -1f && t[3] == 0f)
			{
				result = Orientation.Portrait;
			} else
			if (t[0] == 0f && t[1] == -1f && t[2] == 1f && t[3] == 0f)
			{
				result = Orientation.PortraitFlipped;
			} else
			if (t[0]== 1f && t[1] == 0f && t[2] == 0f && t[3] == 1f)
			{
				result = Orientation.Landscape;
			} else
			if (t[0] == -1f && t[1] == 0f && t[2] == 0f && t[3] == -1f)
			{
				result = Orientation.LandscapeFlipped;
			}
			return result;
		}

		public static Matrix4x4 GetMatrixForOrientation(Orientation ori)
		{
			// TODO: cache these matrices
			Matrix4x4 portrait = Matrix4x4.TRS(new Vector3(0f, 1f, 0f), Quaternion.Euler(0f, 0f, -90f), Vector3.one);
			Matrix4x4 portraitFlipped = Matrix4x4.TRS(new Vector3(1f, 0f, 0f), Quaternion.Euler(0f, 0f, 90f), Vector3.one);
			Matrix4x4 landscapeFlipped = Matrix4x4.TRS(new Vector3(1f, 1f, 0f), Quaternion.identity, new Vector3(-1f, -1f, 1f));

			Matrix4x4 result = Matrix4x4.identity;
			switch (ori)
			{
				case Orientation.Landscape:
					break;
				case Orientation.LandscapeFlipped:
					result = landscapeFlipped;
					break;
				case Orientation.Portrait:
					result = portrait;
					break;
				case Orientation.PortraitFlipped:
					result = portraitFlipped;
					break;
			}
			return result;
		}

		public static void SetupStereoMaterial(Material material, StereoPacking packing, bool displayDebugTinting)
		{
			material.DisableKeyword("STEREO_CUSTOM_UV");
			material.DisableKeyword("STEREO_TOP_BOTTOM");
			material.DisableKeyword("STEREO_LEFT_RIGHT");
			material.DisableKeyword("MONOSCOPIC");

			// Enable the required mode
			switch (packing)
			{
				case StereoPacking.None:
					break;
				case StereoPacking.TopBottom:
					material.EnableKeyword("STEREO_TOP_BOTTOM");
					break;
				case StereoPacking.LeftRight:
					material.EnableKeyword("STEREO_LEFT_RIGHT");
					break;
				case StereoPacking.CustomUV:
					material.EnableKeyword("STEREO_CUSTOM_UV");
					break;
			}

			if (displayDebugTinting)
			{
				material.EnableKeyword("STEREO_DEBUG");
			}
			else
			{
				material.DisableKeyword("STEREO_DEBUG");
			}
		}

		public static void SetupAlphaPackedMaterial(Material material, AlphaPacking packing)
		{
			material.DisableKeyword("ALPHAPACK_TOP_BOTTOM");
			material.DisableKeyword("ALPHAPACK_LEFT_RIGHT");
			material.DisableKeyword("ALPHAPACK_NONE");

			// Enable the required mode
			switch (packing)
			{
				case AlphaPacking.None:
					break;
				case AlphaPacking.TopBottom:
					material.EnableKeyword("ALPHAPACK_TOP_BOTTOM");
					break;
				case AlphaPacking.LeftRight:
					material.EnableKeyword("ALPHAPACK_LEFT_RIGHT");
					break;
			}
		}

		public static void SetupGammaMaterial(Material material, bool playerSupportsLinear)
		{
#if UNITY_PLATFORM_SUPPORTS_LINEAR
			if (QualitySettings.activeColorSpace == ColorSpace.Linear && !playerSupportsLinear)
			{
				material.EnableKeyword("APPLY_GAMMA");
			}
			else
			{
				material.DisableKeyword("APPLY_GAMMA");
			}
#endif
		}

		public static int ConvertTimeSecondsToFrame(float seconds, float frameRate)
		{
			return Mathf.FloorToInt(frameRate * seconds);
		}

		public static float ConvertFrameToTimeSeconds(int frame, float frameRate)
		{
			float frameDurationSeconds = 1f / frameRate;
			return ((float)frame * frameDurationSeconds) + (frameDurationSeconds * 0.5f);		// Add half a frame we that the time lands in the middle of the frame range and not at the edges
		}

		public static void DrawTexture(Rect screenRect, Texture texture, ScaleMode scaleMode, AlphaPacking alphaPacking, Material material)
		{
			if (Event.current.type == EventType.Repaint)
			{
				float textureWidth = texture.width;
				float textureHeight = texture.height;
				switch (alphaPacking)
				{
					case AlphaPacking.LeftRight:
						textureWidth *= 0.5f;
						break;
					case AlphaPacking.TopBottom:
						textureHeight *= 0.5f;
						break;
				}

				float aspectRatio = (float)textureWidth / (float)textureHeight;
				Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
				switch (scaleMode)
				{
					case ScaleMode.ScaleAndCrop:
						{
							float screenRatio = screenRect.width / screenRect.height;
							if (screenRatio > aspectRatio)
							{
								float adjust = aspectRatio / screenRatio;
								sourceRect = new Rect(0f, (1f - adjust) * 0.5f, 1f, adjust);
							}
							else
							{
								float adjust = screenRatio / aspectRatio;
								sourceRect = new Rect(0.5f - adjust * 0.5f, 0f, adjust, 1f);
							}
						}
						break;
					case ScaleMode.ScaleToFit:
						{
							float screenRatio = screenRect.width / screenRect.height;
							if (screenRatio > aspectRatio)
							{
								float adjust = aspectRatio / screenRatio;
								screenRect = new Rect(screenRect.xMin + screenRect.width * (1f - adjust) * 0.5f, screenRect.yMin, adjust * screenRect.width, screenRect.height);
							}
							else
							{
								float adjust = screenRatio / aspectRatio;
								screenRect = new Rect(screenRect.xMin, screenRect.yMin + screenRect.height * (1f - adjust) * 0.5f, screenRect.width, adjust * screenRect.height);
							}
						}
						break;
					case ScaleMode.StretchToFill:
						break;
				}

				Graphics.DrawTexture(screenRect, texture, sourceRect, 0, 0, 0, 0, GUI.color, material);
			}
		}

		// Converts a non-readable texture to a readable Texture2D.
		// "targetTexture" can be null or you can pass in an existing texture.
		// Remember to Destroy() the returned texture after finished with it
		public static Texture2D GetReadableTexture(Texture inputTexture, bool requiresVerticalFlip, Orientation ori, Texture2D targetTexture)
		{
			Texture2D resultTexture = targetTexture;

			RenderTexture prevRT = RenderTexture.active;

			int textureWidth = inputTexture.width;
			int textureHeight = inputTexture.height;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
			if (ori == Orientation.Portrait || ori == Orientation.PortraitFlipped)
			{
				textureWidth = inputTexture.height;
				textureHeight = inputTexture.width;
			}
#endif

			// Blit the texture to a temporary RenderTexture
			// This handles any format conversion that is required and allows us to use ReadPixels to copy texture from RT to readable texture
			RenderTexture tempRT = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);

			if (ori == Orientation.Landscape)
			{
				if (!requiresVerticalFlip)
				{
					Graphics.Blit(inputTexture, tempRT);
				}
				else
				{
					// The above Blit can't flip unless using a material, so we use Graphics.DrawTexture instead
					GL.PushMatrix();
					RenderTexture.active = tempRT;
					GL.LoadPixelMatrix(0f, tempRT.width, 0f, tempRT.height);
					Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
					// NOTE: not sure why we need to set y to -1, without this there is a 1px gap at the bottom
					Rect destRect = new Rect(0f, -1f, tempRT.width, tempRT.height);

					Graphics.DrawTexture(destRect, inputTexture, sourceRect, 0, 0, 0, 0);
					GL.PopMatrix();
					GL.InvalidateState();
				}
			}
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
			else
			{
				Matrix4x4 m = Matrix4x4.identity;
				switch (ori)
				{
					case Orientation.Portrait:
						m = Matrix4x4.TRS(new Vector3(0f, inputTexture.width, 0f), Quaternion.Euler(0f, 0f, -90f), Vector3.one);
						break;
					case Orientation.PortraitFlipped:
						m = Matrix4x4.TRS(new Vector3(inputTexture.height, 0f, 0f), Quaternion.Euler(0f, 0f, 90f), Vector3.one);
						break;
					case Orientation.LandscapeFlipped:
						m = Matrix4x4.TRS(new Vector3(inputTexture.width, inputTexture.height, 0f), Quaternion.identity, new Vector3(-1f, -1f, 1f));
						break;
				}
				
				// The above Blit can't flip unless using a material, so we use Graphics.DrawTexture instead
				GL.InvalidateState();
				GL.PushMatrix();
				GL.Clear(false, true, Color.red);
				RenderTexture.active = tempRT;
				GL.LoadPixelMatrix(0f, tempRT.width, 0f, tempRT.height);
				Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
				// NOTE: not sure why we need to set y to -1, without this there is a 1px gap at the bottom
				Rect destRect = new Rect(0f, -1f, inputTexture.width, inputTexture.height);
				GL.MultMatrix(m);

				Graphics.DrawTexture(destRect, inputTexture, sourceRect, 0, 0, 0, 0);
				GL.PopMatrix();
				GL.InvalidateState();
			}
#endif

			if (resultTexture == null)
			{
				resultTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
			}

			RenderTexture.active = tempRT;
			resultTexture.ReadPixels(new Rect(0f, 0f, textureWidth, textureHeight), 0, 0, false);
			resultTexture.Apply(false, false);
			RenderTexture.ReleaseTemporary(tempRT);

			RenderTexture.active = prevRT;

			return resultTexture;
		}

		/// <summary>
		/// Parse time in format: 00:00:48,924 and convert to milliseconds
		/// </summary>
		private static int ParseTimeToMs(string text)
		{
			int result = 0;

			string[] digits = text.Split(new char[] { ':', ',' });

			if (digits.Length == 4)
			{
				int hours = int.Parse(digits[0]);
				int minutes = int.Parse(digits[1]);
				int seconds = int.Parse(digits[2]);
				int milliseconds = int.Parse(digits[3]);

				result = milliseconds + (seconds + (minutes + (hours * 60)) * 60) * 1000;
			}

			return result;
		}

		public static List<Subtitle> LoadSubtitlesSRT(string data)
		{
			List<Subtitle> result = null;

			if (!string.IsNullOrEmpty(data))
			{
				data = data.Trim();
				string[] lines = data.Split(new string[] { "\n\r", "\r\n", "\n", "\r" }, System.StringSplitOptions.None);

				if (lines.Length >= 3)
				{
					result = new List<Subtitle>(256);

					int count = 0;
					int index = 0;
					Subtitle subtitle = null;
					for (int i = 0; i < lines.Length; i++)
					{
						if (index == 0)
						{
							subtitle = new Subtitle();
							subtitle.index = count;// int.Parse(lines[i]);
						}
						else if (index == 1)
						{
							string[] times = lines[i].Split(new string[] { " --> " }, System.StringSplitOptions.RemoveEmptyEntries);
							if (times.Length == 2)
							{
								subtitle.timeStartMs = ParseTimeToMs(times[0]);
								subtitle.timeEndMs = ParseTimeToMs(times[1]);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(lines[i]))
							{
								if (index == 2)
								{
									subtitle.text = lines[i];
								}
								else
								{
									subtitle.text += "\n" + lines[i];
								}
							}
						}

						if (string.IsNullOrEmpty(lines[i]) && index > 1)
						{
							result.Add(subtitle);
							index = 0;
							count++;
							subtitle = null;
						}
						else
						{
							index++;
						}
					}

					// Handle the last one
					if (subtitle != null)
					{
						result.Add(subtitle);
						subtitle = null;
					}
				}
				else
				{
					Debug.LogWarning("[AVProVideo] SRT format doesn't appear to be valid");
				}
			}

			return result;
		}
	}
}
