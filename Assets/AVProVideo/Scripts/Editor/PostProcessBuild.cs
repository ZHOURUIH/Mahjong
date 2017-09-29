#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

//-----------------------------------------------------------------------------
// Copyright 2015-2017 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Editor
{
	public class PostProcessBuild
	{
		[PostProcessBuild]
 		public static void OnPostProcessBuild(BuildTarget target, string path)
 		{
			if (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXUniversal)
			{
				string message = "AVPro Video doesn't support target StandaloneOSXIntel (32-bit), please use StandaloneOSXIntel64 (64-bit) or remove this PostProcessBuild script";
				Debug.LogError(message);
				EditorUtility.DisplayDialog("AVPro Video", message, "Ok");
			}
		}
	}
}
#endif