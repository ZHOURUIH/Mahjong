//#define AVPROVIDEO_BETA_SUPPORT_TIMESCALE		// BETA FEATURE: comment this in if you want to support frame stepping based on changes in Time.timeScale or Time.captureFramerate
//#define AVPROVIDEO_FORCE_NULL_MEDIAPLAYER		// DEV FEATURE: comment this out to make all mediaplayers use the null mediaplayer
//#define AVPROVIDEO_DEBUG_DISPLAY_EVENTS		// DEV FEATURE: show event logs in the gui display
//#define AVPROVIDEO_DEBUG_FRAMESYNC			// INTERNAL TESTING
//#define AVPROVIDEO_DISABLE_DEBUG_GUI			// INTERNAL TESTING
//#define AVPROVIDEO_DISABLE_LOGGING			// DEV FEATURE: disables Debug.Log from AVPro Video
#if UNITY_ANDROID && !UNITY_EDITOR
	#define REAL_ANDROID
#endif
#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0)
	#define UNITY_HELPATTRIB
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.Storage.Streams;
#endif

//-----------------------------------------------------------------------------
// Copyright 2015-2017 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Media Player", -100)]
#if UNITY_HELPATTRIB
	[HelpURL("http://renderheads.com/product/avpro-video/")]
#endif
	public class MediaPlayer : MonoBehaviour
	{
		// These fields are just used to setup the default properties for a new video that is about to be loaded
		// Once a video has been loaded you should use the interfaces exposed in the properties to
		// change playback properties (eg volume, looping, mute)
		public FileLocation m_VideoLocation = FileLocation.RelativeToStreamingAssetsFolder;

		public string m_VideoPath;

		public bool m_AutoOpen = true;
		public bool m_AutoStart = true;
		public bool m_Loop = false;

		[Range(0.0f, 1.0f)]
		public float m_Volume = 1.0f;

		[SerializeField]
		[Range(-1.0f, 1.0f)]
		private float m_Balance = 0.0f;

		public bool m_Muted = false;

		[SerializeField]
		[Range(-4.0f, 4.0f)]
		public float m_PlaybackRate = 1.0f;

		[System.Serializable]
		public class Setup
		{
			public bool displayDebugGUI;
			public bool persistent;
		}

		// Component Properties
		[SerializeField]
		private bool m_DebugGui = false;

		[SerializeField]
		private bool m_DebugGuiControls = true;

		[SerializeField]
		private bool m_Persistent = false;

		public bool DisplayDebugGUI
		{
			get { return m_DebugGui; }
			set { m_DebugGui = value; }
		}

		public bool DisplayDebugGUIControls
		{
			get { return m_DebugGuiControls; }
			set { m_DebugGuiControls = value; }
		}

		public bool Persistent
		{
			get { return m_Persistent; }
			set { m_Persistent = value; }
		}

		public StereoPacking m_StereoPacking = StereoPacking.None;

		public AlphaPacking m_AlphaPacking = AlphaPacking.None;

		public bool m_DisplayDebugStereoColorTint = false;

		public FilterMode m_FilterMode = FilterMode.Bilinear;

		public TextureWrapMode m_WrapMode = TextureWrapMode.Clamp;

		[Range(0, 16)]
		public int m_AnisoLevel = 0;

		[SerializeField]
		private bool m_LoadSubtitles;

		[SerializeField]
		private FileLocation m_SubtitleLocation = FileLocation.RelativeToStreamingAssetsFolder;
		private FileLocation m_queueSubtitleLocation;

		[SerializeField]
		private string m_SubtitlePath;
		private string m_queueSubtitlePath;
		private Coroutine m_loadSubtitlesRoutine;

		[SerializeField]
		private MediaPlayerEvent m_events;

		private IMediaControl m_Control;
		private IMediaProducer m_Texture;
		private IMediaInfo m_Info;
		private IMediaPlayer m_Player;
		private IMediaSubtitles m_Subtitles;
		private System.IDisposable m_Dispose;

		// State
		private bool m_VideoOpened = false;
		private bool m_AutoStartTriggered = false;
		private bool m_WasPlayingOnPause = false;
		private Coroutine _renderingCoroutine = null;

		// Debug GUI
		private const int s_GuiDepth = -1000;
		private const float s_GuiScale = 1.5f;
		private const int s_GuiStartWidth = 10;
		private const int s_GuiWidth = 180;
#if AVPROVIDEO_DISABLE_DEBUG_GUI && !UNITY_EDITOR
#else
		private int m_GuiPositionX = s_GuiStartWidth;
#endif

		// Global init
		private static bool s_GlobalStartup = false;

		// Event state
		private bool m_EventFired_ReadyToPlay = false;
		private bool m_EventFired_Started = false;
		private bool m_EventFired_FirstFrameReady = false;
		private bool m_EventFired_FinishedPlaying = false;
		private bool m_EventFired_MetaDataReady = false;
		private int m_previousSubtitleIndex = -1;
		private bool m_isPlaybackStalled = false;


		public enum FileLocation
		{
			AbsolutePathOrURL,
			RelativeToProjectFolder,
			RelativeToStreamingAssetsFolder,
			RelativeToDataFolder,
			RelativeToPeristentDataFolder,
			// TODO: Resource?
		}

		[System.Serializable]
		public class PlatformOptions
		{
			public bool overridePath = false;
			public FileLocation pathLocation = FileLocation.RelativeToStreamingAssetsFolder;
			public string path;

			public virtual bool IsModified()
			{
				return overridePath;     // The other variables don't matter if overridePath is false
			}
		}

		[System.Serializable]
		public class OptionsWindows : PlatformOptions
		{
			public Windows.VideoApi videoApi = Windows.VideoApi.MediaFoundation;
			public bool useHardwareDecoding = true;
			public bool useUnityAudio = false;
			public bool useTextureMips = false;
			public string forceAudioOutputDeviceName = string.Empty;

			public override bool IsModified()
			{
				return (base.IsModified() || !useHardwareDecoding || useTextureMips || useUnityAudio || videoApi != Windows.VideoApi.MediaFoundation);
			}
		}

		[System.Serializable]
		public class OptionsMacOSX : PlatformOptions
		{
			[Multiline]
			public string httpHeaderJson = null;

			public override bool IsModified()
			{
				return (base.IsModified() || !string.IsNullOrEmpty(httpHeaderJson));
			}
		}
		[System.Serializable]
		public class OptionsIOS : PlatformOptions
		{
			public bool useYpCbCr420Textures = false;

			[Multiline]
			public string httpHeaderJson = null;

			public override bool IsModified()
			{
				return (base.IsModified() || !string.IsNullOrEmpty(httpHeaderJson) || useYpCbCr420Textures);
			}
		}
		[System.Serializable]
		public class OptionsTVOS : PlatformOptions
		{
			public bool useYpCbCr420Textures = false;

			[Multiline]
			public string httpHeaderJson = null;

			public override bool IsModified()
			{
				return (base.IsModified() || !string.IsNullOrEmpty(httpHeaderJson) || useYpCbCr420Textures);
			}
		}

		[System.Serializable]
		public class OptionsAndroid : PlatformOptions
		{
			public bool useFastOesPath = false;
			public bool showPosterFrame = false;

			[Multiline]
			public string httpHeaderJson = null;

			[SerializeField]
			public int fileOffset = 0;

			public override bool IsModified()
			{
				return (base.IsModified() || fileOffset != 0 || useFastOesPath || showPosterFrame || !string.IsNullOrEmpty(httpHeaderJson));
			}
		}
		[System.Serializable]
		public class OptionsWindowsPhone : PlatformOptions
		{
			public bool useHardwareDecoding = true;
			public bool useUnityAudio = false;
			public bool useTextureMips = false;

			public override bool IsModified()
			{
				return (base.IsModified() || !useHardwareDecoding || useTextureMips || useUnityAudio);
			}
		}
		[System.Serializable]
		public class OptionsWindowsUWP : PlatformOptions
		{
			public bool useHardwareDecoding = true;
			public bool useUnityAudio = false;
			public bool useTextureMips = false;

			public override bool IsModified()
			{
				return (base.IsModified() || !useHardwareDecoding || useTextureMips || useUnityAudio);
			}
		}
		[System.Serializable]
		public class OptionsWebGL : PlatformOptions
		{
		}

		// TODO: move these to a Setup object
		[SerializeField]
		private OptionsWindows _optionsWindows = new OptionsWindows();
		[SerializeField]
		private OptionsMacOSX _optionsMacOSX = new OptionsMacOSX();
		[SerializeField]
		private OptionsIOS _optionsIOS = new OptionsIOS();
		[SerializeField]
		private OptionsTVOS _optionsTVOS = new OptionsTVOS();
		[SerializeField]
		private OptionsAndroid _optionsAndroid = new OptionsAndroid();
		[SerializeField]
		private OptionsWindowsPhone _optionsWindowsPhone = new OptionsWindowsPhone();
		[SerializeField]
		private OptionsWindowsUWP _optionsWindowsUWP = new OptionsWindowsUWP();
		[SerializeField]
		private OptionsWebGL _optionsWebGL = new OptionsWebGL();

		/// <summary>
		/// Properties
		/// </summary>

		public IMediaInfo Info
		{
			get { return m_Info; }
		}
		public IMediaControl Control
		{
			get { return m_Control; }
		}

		public IMediaPlayer Player
		{
			get { return m_Player; }
		}

		public virtual IMediaProducer TextureProducer
		{
			get { return m_Texture; }
		}

		public virtual IMediaSubtitles Subtitles
		{
			get { return m_Subtitles; }
		}

		public MediaPlayerEvent Events
		{
			get
			{
				if (m_events == null)
				{
					m_events = new MediaPlayerEvent();
				}
				return m_events;
			}
		}

		public bool VideoOpened
		{
			get { return m_VideoOpened; }
		}

		public OptionsWindows PlatformOptionsWindows { get { return _optionsWindows; } }
		public OptionsMacOSX PlatformOptionsMacOSX { get { return _optionsMacOSX; } }
		public OptionsIOS PlatformOptionsIOS { get { return _optionsIOS; } }
		public OptionsTVOS PlatformOptionsTVOS { get { return _optionsTVOS; } }
		public OptionsAndroid PlatformOptionsAndroid { get { return _optionsAndroid; } }
		public OptionsWindowsPhone PlatformOptionsWindowsPhone { get { return _optionsWindowsPhone; } }
		public OptionsWindowsUWP PlatformOptionsWindowsUWP { get { return _optionsWindowsUWP; } }
		public OptionsWebGL PlatformOptionsWebGL { get { return _optionsWebGL; } }

		/// <summary>
		/// Methods
		/// </summary>

		void Awake()
		{
			if (m_Persistent)
			{
				// TODO: set "this.transform.root.gameObject" to also DontDestroyOnLoad?
				DontDestroyOnLoad(this.gameObject);
			}
		}

		protected void Initialise()
		{
			BaseMediaPlayer mediaPlayer = CreatePlatformMediaPlayer();
			if (mediaPlayer != null)
			{
				// Set-up interface
				m_Control = mediaPlayer;
				m_Texture = mediaPlayer;
				m_Info = mediaPlayer;
				m_Player = mediaPlayer;
				m_Subtitles = mediaPlayer;
				m_Dispose = mediaPlayer;

				if (!s_GlobalStartup)
				{
#if UNITY_5 || UNITY_5_4_OR_NEWER
					Helper.LogInfo(string.Format("Initialising AVPro Video (script v{0} plugin v{1}) on {2}/{3} (MT {4}) on {5}", Helper.ScriptVersion, mediaPlayer.GetVersion(), SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsMultiThreaded, Application.platform));
#else
					Helper.LogInfo(string.Format("Initialising AVPro Video (script v{0} plugin v{1}) on {2}/{3} on {4}", Helper.ScriptVersion, mediaPlayer.GetVersion(), SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion, Application.platform));
#endif

#if AVPROVIDEO_BETA_SUPPORT_TIMESCALE
					Debug.LogWarning("[AVProVideo] TimeScale support used.  This could affect performance when changing Time.timeScale or Time.captureFramerate.  This feature is useful for supporting video capture system that adjust time scale during capturing.");
#endif

#if (UNITY_HAS_GOOGLEVR || UNITY_DAYDREAM) && (UNITY_ANDROID)
					// NOte: WE've removed this minor optimisation until Daydream support is more offical..
					// It seems to work with the official release, but in 5.6beta UNITY_HAS_GOOGLEVR is always defined
					// even for GearVR, which causes a problem as it doesn't use the same stereo eye determination method

					// TODO: add iOS support for this once Unity supports it
					//Helper.LogInfo("Enabling Google Daydream support");
					//Shader.EnableKeyword("GOOGLEVR");
#endif

					s_GlobalStartup = true;
				}
			}
		}

		void Start()
		{
			if (m_Control == null)
			{
				Initialise();
			}

			if (m_Control != null)
			{
				if (m_AutoOpen)
				{
					OpenVideoFromFile();

					if (m_LoadSubtitles && m_Subtitles != null && !string.IsNullOrEmpty(m_SubtitlePath))
					{
						EnableSubtitles(m_SubtitleLocation, m_SubtitlePath);
					}
				}

				StartRenderCoroutine();
			}
		}

		public bool OpenVideoFromFile(FileLocation location, string path, bool autoPlay = true)
		{
			m_VideoLocation = location;
			m_VideoPath = path;
			m_AutoStart = autoPlay;

			if (m_Control == null)
			{
				Initialise();
			}

			return OpenVideoFromFile(); 
		}

#if NETFX_CORE
		public bool OpenVideoFromStream(IRandomAccessStream ras, string path, bool autoPlay = true)
		{
			m_VideoLocation = FileLocation.AbsolutePathOrURL;
			m_VideoPath = path;
			m_AutoStart = autoPlay;

			if (m_Control == null)
			{
				Initialise();
			}

			return OpenVideoFromStream(ras);
		}
#endif

		public bool SubtitlesEnabled
		{
			get { return m_LoadSubtitles; }
		}

		public string SubtitlePath
		{
			get { return m_SubtitlePath; }
		}

		public FileLocation SubtitleLocation
		{
			get { return m_SubtitleLocation; }
		}

		public bool EnableSubtitles(FileLocation fileLocation, string filePath)
		{
			bool result = false;
			if (m_Subtitles != null)
			{
				if (!string.IsNullOrEmpty(filePath))
				{
					string fullPath = GetPlatformFilePath(GetPlatform(), ref filePath, ref fileLocation);

					bool checkForFileExist = true;
					if (fullPath.Contains("://"))
					{
						checkForFileExist = false;
					}
#if (UNITY_ANDROID || (UNITY_5_2 && UNITY_WSA))
					checkForFileExist = false;
#endif

					if (checkForFileExist && !System.IO.File.Exists(fullPath))
					{
						Debug.LogError("[AVProVideo] Subtitle file not found: " + fullPath, this);
					}
					else
					{
						Helper.LogInfo("Opening subtitles " + fullPath, this);

						m_previousSubtitleIndex = -1;

						try
						{
							if (fullPath.Contains("://"))
							{
								// Use coroutine and WWW class for loading
								if (m_loadSubtitlesRoutine != null)
								{
									StopCoroutine(m_loadSubtitlesRoutine);
									m_loadSubtitlesRoutine = null;
								}
								m_loadSubtitlesRoutine = StartCoroutine(LoadSubtitlesCoroutine(fullPath, fileLocation, filePath));
							}
							else
							{
								// Load directly from file
#if !UNITY_WEBPLAYER
								string subtitleData = System.IO.File.ReadAllText(fullPath);
								if (m_Subtitles.LoadSubtitlesSRT(subtitleData))
								{
									m_SubtitleLocation = fileLocation;
									m_SubtitlePath = filePath;
									m_LoadSubtitles = false;
									result = true;
								}
								else
#endif
								{
									Debug.LogError("[AVProVideo] Failed to load subtitles" + fullPath, this);
								}
							}

						}
						catch (System.Exception e)
						{
							Debug.LogError("[AVProVideo] Failed to load subtitles " + fullPath, this);
							Debug.LogException(e, this);
						}
					}
				}
				else
				{
					Debug.LogError("[AVProVideo] No subtitle file path specified", this);
				}
			}
			else
			{
				m_queueSubtitleLocation = fileLocation;
				m_queueSubtitlePath = filePath;
			}

			return result;
		}

		private IEnumerator LoadSubtitlesCoroutine(string url, FileLocation fileLocation, string filePath)
		{
			WWW www = new WWW(url);

			yield return www;

			string subtitleData = string.Empty;
			if (string.IsNullOrEmpty(www.error))
			{
				subtitleData = www.text;
			}
			else
			{
				Debug.LogError("[AVProVideo] Error loading subtitles '" + www.error + "' from " + url);
			}

			if (m_Subtitles.LoadSubtitlesSRT(subtitleData))
			{
				m_SubtitleLocation = fileLocation;
				m_SubtitlePath = filePath;
				m_LoadSubtitles = false;
			}
			else
			{
				Debug.LogError("[AVProVideo] Failed to load subtitles" + url, this);
			}

			m_loadSubtitlesRoutine = null;
		}

		public void DisableSubtitles()
		{
			if (m_loadSubtitlesRoutine != null)
			{
				StopCoroutine(m_loadSubtitlesRoutine);
				m_loadSubtitlesRoutine = null;
			}

			if (m_Subtitles != null)
			{
				m_previousSubtitleIndex = -1;
				m_LoadSubtitles = false;
				m_Subtitles.LoadSubtitlesSRT(string.Empty);
			}
			else
			{
				m_queueSubtitlePath = string.Empty;
			}
		}

		private bool OpenVideoFromFile()
		{
			bool result = false;
			// Open the video file
			if ( m_Control != null )
			{
				CloseVideo();

				m_VideoOpened = true;
				m_AutoStartTriggered = !m_AutoStart;
				m_EventFired_MetaDataReady = false;
				m_EventFired_ReadyToPlay = false;
				m_EventFired_Started = false;
				m_EventFired_FirstFrameReady = false;
				m_previousSubtitleIndex = -1;

				// Potentially override the file location
				long fileOffset = GetPlatformFileOffset();
				string fullPath = GetPlatformFilePath(GetPlatform(), ref m_VideoPath, ref m_VideoLocation);

				if (!string.IsNullOrEmpty(m_VideoPath))
				{
					string httpHeaderJson = null;

					bool checkForFileExist = true;
					if (fullPath.Contains("://"))
					{
						checkForFileExist = false;
						httpHeaderJson = GetPlatformHttpHeaderJson();
						// TODO: validate the above JSON
					}
#if (UNITY_ANDROID || (UNITY_5_2 && UNITY_WSA))
					checkForFileExist = false;
#endif

					if (checkForFileExist && !System.IO.File.Exists(fullPath))
					{
						Debug.LogError("[AVProVideo] File not found: " + fullPath, this);
					}
					else
					{
						Helper.LogInfo("Opening " + fullPath + " (offset " + fileOffset + ")", this);

						if (!m_Control.OpenVideoFromFile(fullPath, fileOffset, httpHeaderJson))
						{
							Debug.LogError("[AVProVideo] Failed to open " + fullPath, this);
						}
						else
						{
							SetPlaybackOptions();
							result = true;
							StartRenderCoroutine();
						}
					}
				}
				else
				{
					Debug.LogError("[AVProVideo] No file path specified", this);
				}
			}
			return result;
		}

#if NETFX_CORE
		private bool OpenVideoFromStream(IRandomAccessStream ras)
		{
			bool result = false;
			// Open the video file
			if (m_Control != null)
			{
				CloseVideo();

				m_VideoOpened = true;
				m_AutoStartTriggered = !m_AutoStart;
				m_EventFired_MetaDataReady = false;
				m_EventFired_ReadyToPlay = false;
				m_EventFired_Started = false;
				m_EventFired_FirstFrameReady = false;
				m_previousSubtitleIndex = -1;

				// Potentially override the file location
				long fileOffset = GetPlatformFileOffset();

				if (!m_Control.OpenVideoFromFile(ras, m_VideoPath, fileOffset, null))
				{
					Debug.LogError("[AVProVideo] Failed to open " + m_VideoPath, this);
				}
				else
				{
					SetPlaybackOptions();
					result = true;
					StartRenderCoroutine();
				}
			}
			return result;
		}
#endif

		private void SetPlaybackOptions()
		{
			// Set playback options
			if (m_Control != null)
			{
				m_Control.SetLooping(m_Loop);
				m_Control.SetVolume(m_Volume);
				m_Control.SetBalance(m_Balance);
				m_Control.SetPlaybackRate(m_PlaybackRate);
				m_Control.MuteAudio(m_Muted);
				m_Control.SetTextureProperties(m_FilterMode, m_WrapMode, m_AnisoLevel);
			}
		}

		public void CloseVideo()
		{
			// Close the video file
			if( m_Control != null )
			{
				if (m_events != null)
				{
					m_events.Invoke(this, MediaPlayerEvent.EventType.Closing, ErrorCode.None);
				}

				m_AutoStartTriggered = false;
				m_VideoOpened = false;
				m_EventFired_ReadyToPlay = false;
				m_EventFired_Started = false;
				m_EventFired_MetaDataReady = false;
				m_EventFired_FirstFrameReady = false;

				if (m_loadSubtitlesRoutine != null)
				{
					StopCoroutine(m_loadSubtitlesRoutine);
					m_loadSubtitlesRoutine = null;
				}
				m_previousSubtitleIndex = -1;

				m_isPlaybackStalled = false;

				m_Control.CloseVideo();
			}
			StopRenderCoroutine();
		}

		public void Play()
		{
			if (m_Control != null && m_Control.CanPlay())
			{
				m_Control.Play();

				// Mark this event as done because it's irrelevant once playback starts
				m_EventFired_ReadyToPlay = true;
			}
			else
			{
				// Can't play, perhaps it's still loading?  Queuing play using m_AutoStart to play after loading
				m_AutoStart = true;
			}
		}

		public void Pause()
		{
			if (m_Control != null && m_Control.IsPlaying())
			{
				m_Control.Pause();
			}
#if AVPROVIDEO_BETA_SUPPORT_TIMESCALE
			_timeScaleIsControlling = false;
#endif
		}

		public void Stop()
		{
			if (m_Control != null)
			{
				m_Control.Stop();
			}
#if AVPROVIDEO_BETA_SUPPORT_TIMESCALE
			_timeScaleIsControlling = false;
#endif
		}

		public void Rewind(bool pause)
		{
			if (m_Control != null)
			{
				if (pause)
				{
					Pause();
				}
				m_Control.Rewind();
			}
		}

		void Update()
		{
			// Auto start the playback
			if (m_Control != null)
			{
				if (m_VideoOpened && m_AutoStart && !m_AutoStartTriggered && m_Control.CanPlay())
				{
					m_AutoStartTriggered = true;
					Play();
				}

				if (_renderingCoroutine == null && m_Control.CanPlay())
				{
					StartRenderCoroutine();
				}

				if (m_Subtitles != null && !string.IsNullOrEmpty(m_queueSubtitlePath))
				{
					EnableSubtitles(m_queueSubtitleLocation, m_queueSubtitlePath);
					m_queueSubtitlePath = string.Empty;
				}

#if AVPROVIDEO_BETA_SUPPORT_TIMESCALE
				UpdateTimeScale();
#endif

				// Update
				m_Player.Update();

				// Render (done in co-routine)
				//m_Player.Render();

				UpdateErrors();
				UpdateEvents();
			}			
		}

		void OnEnable()
		{
			if (m_Control != null && m_WasPlayingOnPause)
			{
				m_AutoStart = true;
				m_AutoStartTriggered = false;
				m_WasPlayingOnPause = false;
			}

			if(m_Player != null)
			{
				m_Player.OnEnable();
			}

			StartRenderCoroutine();
		}

		void OnDisable()
		{
			if (m_Control != null)
			{
				if (m_Control.IsPlaying())
				{
					m_WasPlayingOnPause = true;
					Pause();
				}
			}

			StopRenderCoroutine();
		}

		void OnDestroy()
		{
			CloseVideo();

			if (m_Dispose != null)
			{
				m_Dispose.Dispose();
				m_Dispose = null;
			}
			m_Control = null;
			m_Texture = null;
			m_Info = null;
			m_Player = null;

			// TODO: possible bug if MediaPlayers are created and destroyed manually (instantiated), OnApplicationQuit won't be called!
		}

		void OnApplicationQuit()
		{
			if (s_GlobalStartup)
			{
				Helper.LogInfo("Shutdown");

				// Clean up any open media players
				MediaPlayer[] players = Resources.FindObjectsOfTypeAll<MediaPlayer>();
				if (players != null && players.Length > 0)
				{
					for (int i = 0; i < players.Length; i++)
					{
						players[i].CloseVideo();
					}
				}

#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
				WindowsMediaPlayer.DeinitPlatform();
#endif
#else
#if (UNITY_STANDALONE_WIN)
				WindowsMediaPlayer.DeinitPlatform();
#endif
#endif
				s_GlobalStartup = false;
			}
		}

#region Rendering Coroutine

		private void StartRenderCoroutine()
		{
			if (_renderingCoroutine == null)
			{
				// Use the method instead of the method name string to prevent garbage
				_renderingCoroutine = StartCoroutine(FinalRenderCapture());
			}
		}

		private void StopRenderCoroutine()
		{
			if (_renderingCoroutine != null)
			{
				StopCoroutine(_renderingCoroutine);
				_renderingCoroutine = null;
			}
		}

		private IEnumerator FinalRenderCapture()
		{
			// Preallocate the YieldInstruction to prevent garbage
			YieldInstruction wait = new WaitForEndOfFrame();
			while (Application.isPlaying)
			{
				// NOTE: in editor, if the game view isn't visible then WaitForEndOfFrame will never complete
				yield return wait;

				if (this.enabled)
				{
					if (m_Player != null)
					{
						m_Player.Render();
					}
				}
			}
		}
#endregion

#region Platform and Path
		public static Platform GetPlatform()
		{
			Platform result = Platform.Unknown;

			// Setup for running in the editor (Either OSX, Windows or Linux)
#if UNITY_EDITOR
#if (UNITY_EDITOR_OSX && UNITY_EDITOR_64)
			result = Platform.MacOSX;
#elif UNITY_EDITOR_WIN
			result = Platform.Windows;
#endif
#else
			// Setup for running builds
#if (UNITY_STANDALONE_WIN)
			result = Platform.Windows;
#elif (UNITY_STANDALONE_OSX)
			result = Platform.MacOSX;
#elif (UNITY_IPHONE || UNITY_IOS)
			result = Platform.iOS;
#elif (UNITY_TVOS)
			result = Platform.tvOS;
#elif (UNITY_ANDROID)
			result = Platform.Android;
#elif (UNITY_WP8 || UNITY_WP81 || UNITY_WINRT_8_1)
			result = Platform.WindowsPhone;
#elif (UNITY_WSA_10_0)
			result = Platform.WindowsUWP;
#elif (UNITY_WEBGL)
			result = Platform.WebGL;
#endif
			
#endif
			return result;
		}

		public PlatformOptions GetCurrentPlatformOptions()
		{
			PlatformOptions result = null;

#if UNITY_EDITOR
#if (UNITY_EDITOR_OSX && UNITY_EDITOR_64)
			result = _optionsMacOSX;
#elif UNITY_EDITOR_WIN
			result = _optionsWindows;
#endif
#else
	// Setup for running builds
			
#if (UNITY_STANDALONE_WIN)
			result = _optionsWindows;
#elif (UNITY_STANDALONE_OSX)
			result = _optionsMacOSX;
#elif (UNITY_IPHONE || UNITY_IOS)
			result = _optionsIOS;
#elif (UNITY_TVOS)
			result = _optionsTVOS;
#elif (UNITY_ANDROID)
			result = _optionsAndroid;
#elif (UNITY_WP8 || UNITY_WP81 || UNITY_WINRT_8_1)
			result = _optionsWindowsPhone;
#elif (UNITY_WSA_10_0)
			result = _optionsWindowsUWP;
#elif (UNITY_WEBGL)
			result = _optionsWebGL;
#endif
			
#endif
			return result;
		}

#if UNITY_EDITOR
		public PlatformOptions GetPlatformOptions(Platform platform)
		{
			PlatformOptions result = null;

			switch (platform)
			{
				case Platform.Windows:
					result = _optionsWindows;
					break;
				case Platform.MacOSX:
					result = _optionsMacOSX;
					break;
				case Platform.Android:
					result = _optionsAndroid;
					break;
				case Platform.iOS:
					result = _optionsIOS;
					break;
				case Platform.tvOS:
					result = _optionsTVOS;
					break;
				case Platform.WindowsPhone:
					result = _optionsWindowsPhone;
					break;
				case Platform.WindowsUWP:
					result = _optionsWindowsUWP;
					break;
				case Platform.WebGL:
					result = _optionsWebGL;
					break;
			}

			return result;
		}

		public static string GetPlatformOptionsVariable(Platform platform)
		{
			string result = string.Empty;

			switch (platform)
			{
				case Platform.Windows:
					result = "_optionsWindows";
					break;
				case Platform.MacOSX:
					result = "_optionsMacOSX";
					break;
				case Platform.iOS:
					result = "_optionsIOS";
					break;
				case Platform.tvOS:
					result = "_optionsTVOS";
					break;
				case Platform.Android:
					result = "_optionsAndroid";
					break;
				case Platform.WindowsPhone:
					result = "_optionsWindowsPhone";
					break;
				case Platform.WindowsUWP:
					result = "_optionsWindowsUWP";
					break;
				case Platform.WebGL:
					result = "_optionsWebGL";
					break;
			}

			return result;
		}
#endif

		public static string GetPath(FileLocation location)
		{
			string result = string.Empty;
			switch (location)
			{
				case FileLocation.AbsolutePathOrURL:
					break;
				case FileLocation.RelativeToDataFolder:
					result = Application.dataPath;
					break;
				case FileLocation.RelativeToPeristentDataFolder:
					result = Application.persistentDataPath;
					break;
				case FileLocation.RelativeToProjectFolder:
#if !UNITY_WINRT_8_1
                    string path = "..";
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
                        path += "/..";
#endif
					result = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, path));
					result = result.Replace('\\', '/');
#endif
					break;
				case FileLocation.RelativeToStreamingAssetsFolder:
					result = Application.streamingAssetsPath;
					break;
			}
			return result;
		}

		public static string GetFilePath(string path, FileLocation location)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(path))
			{
				switch (location)
				{
					case FileLocation.AbsolutePathOrURL:
						result = path;
						break;
					case FileLocation.RelativeToDataFolder:
					case FileLocation.RelativeToPeristentDataFolder:
					case FileLocation.RelativeToProjectFolder:
					case FileLocation.RelativeToStreamingAssetsFolder:
						result = System.IO.Path.Combine(GetPath(location), path);
						break;
				}
			}
			return result;
		}

		private long GetPlatformFileOffset()
		{
			long result = 0;
#if UNITY_EDITOR_OSX
#elif UNITY_EDITOR_WIN
#elif UNITY_EDITOR_LINUX
#elif UNITY_ANDROID
			result = _optionsAndroid.fileOffset;
#endif
			return result;
		}

		private string GetPlatformHttpHeaderJson()
		{
			string result = null;

#if UNITY_EDITOR_OSX
			result = _optionsMacOSX.httpHeaderJson;
#elif UNITY_EDITOR_WIN
#elif UNITY_EDITOR_LINUX
#elif UNITY_STANDALONE_OSX
			result = _optionsMacOSX.httpHeaderJson;
#elif UNITY_STANDALONE_WIN
#elif UNITY_WSA_10_0
#elif UNITY_WINRT_8_1
#elif UNITY_IOS || UNITY_IPHONE
			result = _optionsIOS.httpHeaderJson;
#elif UNITY_TVOS
			result = _optionsTVOS.httpHeaderJson;
#elif UNITY_ANDROID
			result = _optionsAndroid.httpHeaderJson;
#elif UNITY_WEBGL
#endif

			if (!string.IsNullOrEmpty(result))
			{
				result = result.Trim();
			}

			return result;
		}

		private string GetPlatformFilePath(Platform platform, ref string filePath, ref FileLocation fileLocation)
		{
			string result = string.Empty;

			// Replace file path and location if overriden by platform options
			if (platform != Platform.Unknown)
			{
				PlatformOptions options = GetCurrentPlatformOptions();
				if (options != null)
				{
					if (options.overridePath)
					{
						filePath = options.path;
						fileLocation = options.pathLocation;
					}
				}
			}

			result = GetFilePath(filePath, fileLocation);

			return result;
		}
#endregion

		public virtual BaseMediaPlayer CreatePlatformMediaPlayer()
		{
			BaseMediaPlayer mediaPlayer = null;

#if !AVPROVIDEO_FORCE_NULL_MEDIAPLAYER

			// Setup for running in the editor (Either OSX, Windows or Linux)
#if UNITY_EDITOR
#if (UNITY_EDITOR_OSX)
#if UNITY_EDITOR_64
			mediaPlayer = new OSXMediaPlayer();
#else
			Debug.LogWarning("[AVProVideo] 32-bit OS X Unity editor not supported.  64-bit required.");
#endif
#elif UNITY_EDITOR_WIN
			WindowsMediaPlayer.InitialisePlatform();
			
			mediaPlayer = new WindowsMediaPlayer(_optionsWindows.videoApi, _optionsWindows.useHardwareDecoding, _optionsWindows.useTextureMips, _optionsWindows.forceAudioOutputDeviceName, _optionsWindows.useUnityAudio);
#endif
#else
			// Setup for running builds
#if (UNITY_STANDALONE_WIN || UNITY_WSA_10_0 || UNITY_WINRT_8_1)
			WindowsMediaPlayer.InitialisePlatform();
#if UNITY_STANDALONE_WIN
			mediaPlayer = new WindowsMediaPlayer(_optionsWindows.videoApi, _optionsWindows.useHardwareDecoding, _optionsWindows.useTextureMips, _optionsWindows.forceAudioOutputDeviceName, _optionsWindows.useUnityAudio);
#elif UNITY_WSA_10_0
			mediaPlayer = new WindowsMediaPlayer(Windows.VideoApi.MediaFoundation, _optionsWindowsUWP.useHardwareDecoding, _optionsWindowsUWP.useTextureMips, string.Empty, _optionsWindowsUWP.useUnityAudio);
#elif UNITY_WINRT_8_1
			mediaPlayer = new WindowsMediaPlayer(Windows.VideoApi.MediaFoundation, _optionsWindowsPhone.useHardwareDecoding, _optionsWindowsPhone.useTextureMips, string.Empty, _optionsWindowsPhone.useUnityAudio);
#endif

#elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_IOS || UNITY_TVOS)
#if UNITY_TVOS
			mediaPlayer = new OSXMediaPlayer(_optionsTVOS.useYpCbCr420Textures);
#elif (UNITY_IOS || UNITY_IPHONE)
			mediaPlayer = new OSXMediaPlayer(_optionsIOS.useYpCbCr420Textures);
#else
			mediaPlayer = new OSXMediaPlayer();
#endif

#elif (UNITY_ANDROID)
			// Initialise platform (also unpacks videos from StreamingAsset folder (inside a jar), to the persistent data path)
			AndroidMediaPlayer.InitialisePlatform();
			mediaPlayer = new AndroidMediaPlayer(_optionsAndroid.useFastOesPath, _optionsAndroid.showPosterFrame);
#elif (UNITY_WEBGL)
            WebGLMediaPlayer.InitialisePlatform();
            mediaPlayer = new WebGLMediaPlayer();
#endif
#endif

#endif

			// Fallback
			if (mediaPlayer == null)
			{
				Debug.LogError(string.Format("[AVProVideo] Not supported on this platform {0} {1} {2} {3}.  Using null media player!", Application.platform, SystemInfo.deviceModel, SystemInfo.processorType, SystemInfo.operatingSystem));
				
				mediaPlayer = new NullMediaPlayer();
			}

			return mediaPlayer;
		}

#region Support for Time Scale
#if AVPROVIDEO_BETA_SUPPORT_TIMESCALE
		// Adjust this value to get faster performance but may drop frames.  
		// Wait longer to ensure there is enough time for frames to process
		private const float TimeScaleTimeoutMs = 20f;
		private bool _timeScaleIsControlling;
		private float _timeScaleVideoTime;

		private void UpdateTimeScale()
		{
			if (Time.timeScale != 1f || Time.captureFramerate != 0)
			{
				if (Control.IsPlaying())
				{
					Control.Pause();
					_timeScaleIsControlling = true;
					_timeScaleVideoTime = Control.GetCurrentTimeMs();
				}

				if (_timeScaleIsControlling)
				{
					int frameCount = TextureProducer.GetTextureFrameCount();

					// Progress time
					_timeScaleVideoTime += (Time.deltaTime * 1000f);

					// Handle looping
					if (m_Loop && _timeScaleVideoTime >= Info.GetDurationMs())
					{
						_timeScaleVideoTime = 0f;
					}

					// Seek
					Control.Seek(_timeScaleVideoTime);

					// Wait for frame to change
					ForceWaitForNewFrame(frameCount, TimeScaleTimeoutMs);
				}
			}
			else
			{
				// Restore playback when timeScale becomes 1
				if (_timeScaleIsControlling)
				{
					Control.Play();
					_timeScaleIsControlling = false;
				}
			}
		}
#endif
#endregion

		private bool ForceWaitForNewFrame(int lastFrameCount, float timeoutMs)
		{
			bool result = false;
			// Wait for the frame to change, or timeout to happen (for the case that there is no new frame for this time)
			System.DateTime startTime = System.DateTime.Now;
			int iterationCount = 0;
			while (Control != null && (System.DateTime.Now - startTime).TotalMilliseconds < (double)timeoutMs)
			{
				m_Player.Update();

				// TODO: check if Seeking has completed!  Then we don't have to wait

				// If frame has changed we can continue
				// NOTE: this will never happen because GL.IssuePlugin.Event is never called in this loop
				if (lastFrameCount != TextureProducer.GetTextureFrameCount())
				{
					result = true;
					break;
				}

				iterationCount++;

				// NOTE: we tried to add Sleep for 1ms but it was very slow, so switched to this time based method which burns more CPU but about double the speed
				// NOTE: had to add the Sleep back in as after too many iterations (over 1000000) of GL.IssuePluginEvent Unity seems to lock up
				// NOTE: seems that GL.IssuePluginEvent can't be called if we're stuck in a while loop and they just stack up
				//System.Threading.Thread.Sleep(0);
			}
	
			m_Player.Render();

			return result;
		}

		private void UpdateErrors()
		{
			ErrorCode errorCode = m_Control.GetLastError();
			if (ErrorCode.None != errorCode)
			{
				Debug.LogError("[AVProVideo] Error: " + Helper.GetErrorMessage(errorCode));

				if (m_events != null)
				{
					m_events.Invoke(this, MediaPlayerEvent.EventType.Error, errorCode);
				}
			}
		}

		private void UpdateEvents()
		{
			if (m_events != null && m_Control != null)
			{
				m_EventFired_FinishedPlaying = FireEventIfPossible(MediaPlayerEvent.EventType.FinishedPlaying, m_EventFired_FinishedPlaying);

				// Reset some event states that can reset during playback
				{
					// Keep track of whether the Playing state has changed
					if (m_EventFired_Started && m_Control != null && !m_Control.IsPlaying())
					{
						// Playing has stopped
						m_EventFired_Started = false;
					}

					// NOTE: We check m_Control isn't null in case the scene is unloaded in response to the FinishedPlaying event
					if (m_EventFired_FinishedPlaying && m_Control != null && m_Control.IsPlaying() && !m_Control.IsFinished())
					{
						bool reset = true;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
						reset = false;
						// Don't reset if within a frame of the end of the video, important for time > duration workaround
						float msPerFrame = 1000f / m_Info.GetVideoFrameRate();
						//Debug.Log(m_Info.GetDurationMs() - m_Control.GetCurrentTimeMs() + " " + msPerFrame);
						if(m_Info.GetDurationMs() - m_Control.GetCurrentTimeMs() > msPerFrame)
						{
							reset = true;
						}
#endif
						if (reset)
						{
							//Debug.Log("Reset");
							m_EventFired_FinishedPlaying = false;
						}
					}
				}

				m_EventFired_MetaDataReady = FireEventIfPossible(MediaPlayerEvent.EventType.MetaDataReady, m_EventFired_MetaDataReady);
				m_EventFired_ReadyToPlay = FireEventIfPossible(MediaPlayerEvent.EventType.ReadyToPlay, m_EventFired_ReadyToPlay);
				m_EventFired_Started = FireEventIfPossible(MediaPlayerEvent.EventType.Started, m_EventFired_Started);
				m_EventFired_FirstFrameReady = FireEventIfPossible(MediaPlayerEvent.EventType.FirstFrameReady, m_EventFired_FirstFrameReady);
				if (FireEventIfPossible(MediaPlayerEvent.EventType.SubtitleChange, false))
				{
					m_previousSubtitleIndex = m_Subtitles.GetSubtitleIndex();
				}

				// Is the stalled state changes, fire an event
				{
					bool newStalled = m_Info.IsPlaybackStalled();
					if(newStalled != m_isPlaybackStalled)
					{
						m_isPlaybackStalled = newStalled;

						var stallEvent = m_isPlaybackStalled ? MediaPlayerEvent.EventType.Stalled : MediaPlayerEvent.EventType.Unstalled;
						FireEventIfPossible(stallEvent, false);
					}
				}
			}
		}

		private bool FireEventIfPossible(MediaPlayerEvent.EventType eventType, bool hasFired)
		{
			if (CanFireEvent(eventType, hasFired))
			{
				hasFired = true;
				m_events.Invoke(this, eventType, ErrorCode.None);
			}
			return hasFired;
		}
		
		private bool CanFireEvent(MediaPlayerEvent.EventType et, bool hasFired)
		{
			bool result = false;
			if (m_events != null && m_Control != null && !hasFired)
			{
				switch (et)
				{
					case MediaPlayerEvent.EventType.FinishedPlaying:
						//Debug.Log(m_Control.GetCurrentTimeMs() + " " + m_Info.GetDurationMs());
						result = (!m_Control.IsLooping() && m_Control.CanPlay() && m_Control.IsFinished())
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
							|| (m_Control.GetCurrentTimeMs() > m_Info.GetDurationMs() && !m_Control.IsLooping())
#endif
							;
						break;
					case MediaPlayerEvent.EventType.MetaDataReady:
						result = (m_Control.HasMetaData());
						break;
					case MediaPlayerEvent.EventType.FirstFrameReady:
						result = (m_Texture != null && m_Control.CanPlay() && m_Texture.GetTextureFrameCount() > 0);
						break;
					case MediaPlayerEvent.EventType.ReadyToPlay:
						result = (!m_Control.IsPlaying() && m_Control.CanPlay());
						break;
					case MediaPlayerEvent.EventType.Started:
						result = (m_Control.IsPlaying());
						break;
					case MediaPlayerEvent.EventType.SubtitleChange:
						result = (m_previousSubtitleIndex != m_Subtitles.GetSubtitleIndex());
						break;
					case MediaPlayerEvent.EventType.Stalled:
						result = m_Info.IsPlaybackStalled();
						break;
					case MediaPlayerEvent.EventType.Unstalled:
						result = !m_Info.IsPlaybackStalled();
						break;
				}
			}
			return result;
		}

#region Application Focus and Pausing
#if !UNITY_EDITOR
		void OnApplicationFocus( bool focusStatus )
		{
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
//			Debug.Log("OnApplicationFocus: focusStatus: " + focusStatus);

			if( focusStatus )
			{
				if( m_Control != null && m_WasPlayingOnPause )
				{
					m_WasPlayingOnPause = false;
					m_Control.Play();

					Helper.LogInfo("OnApplicationFocus: playing video again");
				}
			}
#endif
		}

		void OnApplicationPause( bool pauseStatus )
		{
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
//			Debug.Log("OnApplicationPause: pauseStatus: " + pauseStatus);

			if( pauseStatus )
			{
				if( m_Control!= null && m_Control.IsPlaying() )
				{
					m_WasPlayingOnPause = true;
					m_Control.Pause();

					Helper.LogInfo("OnApplicationPause: pausing video");
				}
			}
			else
			{
				// Catch coming back from power off state when no lock screen
				OnApplicationFocus( true );
			}
#endif
		}
#endif
#endregion

#region Save Frame To PNG
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		[ContextMenu("Save Frame To PNG")]
		public void SaveFrameToPng()
		{
			Texture2D frame = ExtractFrame(null);
			if (frame != null)
			{
				byte[] imageBytes = frame.EncodeToPNG();
				if (imageBytes != null)
				{
#if !UNITY_WEBPLAYER
					string timecode = Mathf.FloorToInt(Control.GetCurrentTimeMs()).ToString("D8");
					System.IO.File.WriteAllBytes("frame-" + timecode + ".png", imageBytes);
#else
					Debug.LogError("Web Player platform doesn't support file writing.  Change platform in Build Settings.");
#endif
				}

				Destroy(frame);
			}
		}
#endif
#endregion

#region Extract Frame
		// "target" can be null or you can pass in an existing texture.
		public Texture2D ExtractFrame(Texture2D target, float timeSeconds = -1f, bool accurateSeek = true, int timeoutMs = 1000)
		{
			Texture2D result = target;

			// Extract frames returns the interal frame of the video player
			Texture frame = ExtractFrame(timeSeconds, accurateSeek, timeoutMs);
			if (frame != null)
			{
				result = Helper.GetReadableTexture(frame, TextureProducer.RequiresVerticalFlip(), Helper.GetOrientation(Info.GetTextureTransform()), target);
			}

			return result;
		}

		private Texture ExtractFrame(float timeSeconds = -1f, bool accurateSeek = true, int timeoutMs = 1000)
		{
			Texture result = null;

			if (m_Control != null)
			{
				if (timeSeconds >= 0f)
				{
					Pause();

					float seekTimeMs = timeSeconds * 1000f;

					// If the frame is already available just grab it
					if (TextureProducer.GetTexture() != null && m_Control.GetCurrentTimeMs() == seekTimeMs)
					{
						result = TextureProducer.GetTexture();
					}
					else
					{
						// Store frame count before seek
						int frameCount = TextureProducer.GetTextureFrameCount();

						// Seek to the frame
						if (accurateSeek)
						{
							m_Control.Seek(seekTimeMs);
						}
						else
						{
							m_Control.SeekFast(seekTimeMs);
						}

						// Wait for frame to change
						ForceWaitForNewFrame(frameCount, timeoutMs);
						result = TextureProducer.GetTexture();
					}
				}
				else
				{
					result = TextureProducer.GetTexture();
				}
			}
			return result;
		}
#endregion

#region Play/Pause Support for Unity Editor
		// This code handles the pause/play buttons in the editor
#if UNITY_EDITOR
		static MediaPlayer()
		{
			UnityEditor.EditorApplication.playmodeStateChanged -= OnUnityPlayModeChanged;
			UnityEditor.EditorApplication.playmodeStateChanged += OnUnityPlayModeChanged;
		}

		private static void OnUnityPlayModeChanged()
		{
			if (UnityEditor.EditorApplication.isPlaying)
			{
				if (UnityEditor.EditorApplication.isPaused)
				{
					MediaPlayer[] players = Resources.FindObjectsOfTypeAll<MediaPlayer>();
					foreach (MediaPlayer player in players)
					{
						player.EditorPause();
					}
				}
				else
				{
					MediaPlayer[] players = Resources.FindObjectsOfTypeAll<MediaPlayer>();
					foreach (MediaPlayer player in players)
					{
						player.EditorUnpause();
					}
				}
			}
		}

		private void EditorPause()
		{
			if (this.isActiveAndEnabled)
			{
				if (m_Control != null && m_Control.IsPlaying())
				{
					m_WasPlayingOnPause = true;
					Pause();
				}
				StopRenderCoroutine();
			}	
		}

		private void EditorUnpause()
		{
			if (this.isActiveAndEnabled)
			{
				if (m_Control != null && m_WasPlayingOnPause)
				{
					m_AutoStart = true;
					m_WasPlayingOnPause = false;
					m_AutoStartTriggered = false;
				}
				StartRenderCoroutine();
			}
		}
#endif
#endregion

#region IMGUI Debug Information Display
		public void SetGuiPositionFromVideoIndex(int index)
		{
#if AVPROVIDEO_DISABLE_DEBUG_GUI && !UNITY_EDITOR
#else
			m_GuiPositionX = Mathf.FloorToInt((s_GuiStartWidth * s_GuiScale) + (s_GuiWidth * index * s_GuiScale));
#endif
		}

		public void SetDebugGuiEnabled(bool bEnabled)
		{
			m_DebugGui = bEnabled;
		}

#if AVPROVIDEO_DEBUG_FRAMESYNC
		private int _lastFrameCount = 0;
		private int _sameFrameCount = 1;

		public int SameFrameCount
		{
			get { return _sameFrameCount; }
		}

		private void UpdateFrameSyncDebugging()
		{
			int frameCount = TextureProducer.GetTextureFrameCount();
			if (frameCount == _lastFrameCount)
			{
				_sameFrameCount++;
			}
			else
			{
				_sameFrameCount = 1;
			}
			_lastFrameCount = frameCount;
		}
#endif

#if AVPROVIDEO_DEBUG_DISPLAY_EVENTS
		private Queue<string> _eventLog = new Queue<string>(8);
		private float _eventTimer = 1f;

		private void AddEvent(MediaPlayerEvent.EventType et)
		{
			Helper.LogInfo("[MediaPlayer] Event: " + et.ToString(), this);
			_eventLog.Enqueue(et.ToString());
			if (_eventLog.Count > 5)
			{
				_eventLog.Dequeue();
				_eventTimer = 1f;
			}
		}

		private void UpdateEventLogs()
		{
			if (_eventLog != null && _eventLog.Count > 0)
			{
				_eventTimer -= Time.deltaTime;
				if (_eventTimer < 0f)
				{
					_eventLog.Dequeue();
					_eventTimer = 1f;
				}
			}
		}

		// Callback function to handle events
		private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
		{
			AddEvent(et);
		}
#endif

#if AVPROVIDEO_DISABLE_DEBUG_GUI && !UNITY_EDITOR
#else
		void OnGUI()
		{
			if (m_Info != null && m_DebugGui)
			{
				GUI.depth = s_GuiDepth;
				GUI.matrix = Matrix4x4.TRS(new Vector3(m_GuiPositionX, 10f, 0f), Quaternion.identity, new Vector3(s_GuiScale, s_GuiScale, 1.0f));

				GUILayout.BeginVertical("box", GUILayout.MaxWidth(s_GuiWidth));
				GUILayout.Label(System.IO.Path.GetFileName(m_VideoPath));
				GUILayout.Label("Dimensions: " + m_Info.GetVideoWidth() + "x" + m_Info.GetVideoHeight() + "@" + m_Info.GetVideoFrameRate().ToString("F2"));
				GUILayout.Label("Time: " + (m_Control.GetCurrentTimeMs() * 0.001f).ToString("F1") + "s / " + (m_Info.GetDurationMs() * 0.001f).ToString("F1") + "s");
				GUILayout.Label("Rate: " + m_Info.GetVideoDisplayRate().ToString("F2") + "Hz");

				if (TextureProducer != null && TextureProducer.GetTexture() != null)
				{
#if REAL_ANDROID
					// In OES mode we can't display the texture without using a special shader, so just don't display it
					if (!_optionsAndroid.useFastOesPath)
#endif
					{
						// Show texture without and with alpha blending
						GUILayout.BeginHorizontal();
						Rect r1 = GUILayoutUtility.GetRect(32f, 32f);
						GUILayout.Space(8f);
						Rect r2 = GUILayoutUtility.GetRect(32f, 32f);
						Matrix4x4 prevMatrix = GUI.matrix;
						if (TextureProducer.RequiresVerticalFlip())
						{
							GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, r1.y + (r1.height / 2f)));
						}
						GUI.DrawTexture(r1, TextureProducer.GetTexture(), ScaleMode.ScaleToFit, false);
						GUI.DrawTexture(r2, TextureProducer.GetTexture(), ScaleMode.ScaleToFit, true);
						GUI.matrix = prevMatrix;
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
					}
				}

				if (m_DebugGuiControls)
				{
					GUILayout.BeginHorizontal();
					if (m_Control.IsPaused())
					{
						if (GUILayout.Button("Play", GUILayout.Width(50)))
						{
							m_Control.Play();
						}
					}
					else
					{
						if (GUILayout.Button("Pause", GUILayout.Width(50)))
						{
							m_Control.Pause();
						}
					}

					float duration = m_Info.GetDurationMs();
					float time = m_Control.GetCurrentTimeMs();
					float newTime = GUILayout.HorizontalSlider(time, 0f, duration);
					if (newTime != time)
					{
						m_Control.Seek(newTime);
					}
					GUILayout.EndHorizontal();
				}

#if AVPROVIDEO_DEBUG_DISPLAY_EVENTS
				// Dirty code to hack in an event monitor
				if (Event.current.type == EventType.Repaint)
				{
					this.Events.RemoveListener(OnMediaPlayerEvent);
					this.Events.AddListener(OnMediaPlayerEvent);
					UpdateEventLogs();
				}

				if (_eventLog != null && _eventLog.Count > 0)
				{
					GUILayout.Label("Recent Events: ");
					GUILayout.BeginVertical("box");
					int eventIndex = 0;
					foreach (string eventString in _eventLog)
					{
						GUI.color = Color.white;
						if (eventIndex == 0)
						{
							GUI.color = new Color(1f, 1f, 1f, _eventTimer);
						}
						GUILayout.Label(eventString);
						eventIndex++;
					}
					GUILayout.EndVertical();
					GUI.color = Color.white;
				}
#endif
				GUILayout.EndVertical();
			}
		}
#endif

#endregion
	}
}