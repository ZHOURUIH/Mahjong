using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	/// <summary>
	/// In Unity 5.3.x and below there is no support for single pass VR stereo renering,
	/// so this script is needed to send the camera position to the stereo shader so that
	/// it can determine which eye it is rendering.  This script isn't needed for Unity 5.4
	/// and above.
	/// </summary>
	public class UpdateStereoMaterial : MonoBehaviour
	{
		public Camera _camera;
		public MeshRenderer _renderer;
		private int _cameraPositionId;

		void Awake()
		{
			_cameraPositionId = Shader.PropertyToID("_cameraPosition");
		}

		void Update()
		{
			Camera camera = _camera;
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (_renderer == null)
			{
				_renderer = this.gameObject.GetComponent<MeshRenderer>();
			}

			if (camera != null && _renderer != null)
			{
				_renderer.material.SetVector(_cameraPositionId, camera.transform.position);
			}
		}
	}
}