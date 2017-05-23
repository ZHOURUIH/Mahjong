#if UNTIY_IOS		// TODO: Add "|| UNITY_STANDALONE_OSX" to support OS X builds.  We've left it out as the iOS build module is needed for this script to run
	#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0 && !UNITY_5_3_1 && !UNITY_5_3_2
		#define AVPROVIDEO_UNITY_SUPPORTS_PLISTEDIT
	#endif
#endif
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if AVPROVIDEO_UNITY_SUPPORTS_PLISTEDIT
using UnityEditor.iOS.Xcode;	// Required for PlistDocument (requires Untiy 5.3.3 or above)
#endif

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Editor
{
	public class PostProcessBuild 
	{
		[PostProcessBuild]
 		public static void OnPostProcessBuild(BuildTarget target, string path)
 		{
			if (target == BuildTarget.StandaloneOSXIntel)
			{
				Debug.LogError("AVPro Video doesn't support target StandaloneOSXIntel, please use StandaloneOSXIntel64 or remove this PostProcessBuild script");

				EditorUtility.DisplayDialog("AVPro Video", "AVPro Video doesn't support target StandaloneOSXIntel, please use StandaloneOSXIntel64 or remove this PostProcessBuild script", "Ok");
			}

#if AVPROVIDEO_UNITY_SUPPORTS_PLISTEDIT
			// On Apple platforms we need to adjust the plist to allow loading from HTTP for video streaming
 			if ((target == BuildTarget.StandaloneOSXIntel)		||
 				(target == BuildTarget.StandaloneOSXIntel64)	||
 				(target == BuildTarget.StandaloneOSXUniversal))
 			{
 				string plistPath = path + "/Contents/Info.plist";
 				PlistDocument plist = new PlistDocument();
 				plist.ReadFromString(File.ReadAllText(plistPath));
 
 				// Get root
 				PlistElementDict rootDict = plist.root;
 
 				PlistElementDict nsAppTransportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
 				nsAppTransportSecurity.SetBoolean("NSAllowsArbitraryLoads", true);
 
 				File.WriteAllText(plistPath, plist.WriteToString());
			}
#endif
		}
	}
}