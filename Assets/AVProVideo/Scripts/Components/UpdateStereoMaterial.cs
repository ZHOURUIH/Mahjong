#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0)
	#define UNITY_HELPATTRIB
#endif

using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2015-2017 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	/// <summary>
	/// This script is needed to send the camera position to the stereo shader so that
	/// it can determine which eye it is rendering.  In the future this script won't 
	/// be needed once we have support for single-pass stereo rendering.
	/// </summary>
	[AddComponentMenu("AVPro Video/Update Stereo Material", 400)]
#if UNITY_HELPATTRIB
	[HelpURL("http://renderheads.com/product/avpro-video/")]
#endif
	public class UpdateStereoMaterial : MonoBehaviour
	{
		[Header("Stereo camera")]
		public Camera _camera;

		[Header("Rendering elements")]
		public MeshRenderer _renderer;
		public UnityEngine.UI.Graphic _uGuiComponent;
		public Material _material;

		private int _cameraPositionId;
		private int _viewMatrixId;

		void Awake()
		{
			_cameraPositionId = Shader.PropertyToID("_cameraPosition");
			_viewMatrixId = Shader.PropertyToID("_ViewMatrix");
			if (_camera == null)
			{
				Debug.LogWarning("[AVProVideo] No camera set for UpdateStereoMaterial component. If you are rendering in stereo then it is recommended to set this.");
			}
		}

		private void SetupMaterial(Material m, Camera camera)
		{
			m.SetVector(_cameraPositionId, camera.transform.position);
			m.SetMatrix(_viewMatrixId, camera.worldToCameraMatrix.transpose);
		}

		// We do a LateUpdate() to allow for any changes in the camera position that may have happened in Update()
		void LateUpdate()
		{
			Camera camera = _camera;
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (_renderer == null && _material == null)
			{
				_renderer = this.gameObject.GetComponent<MeshRenderer>();
			}

			if (camera != null)
			{
				if (_renderer != null)
				{
					SetupMaterial(_renderer.material, camera);
				}
				if (_material != null)
				{
					SetupMaterial(_material, camera);
				}
				if (_uGuiComponent != null)
				{
					SetupMaterial(_uGuiComponent.material, camera);
				}
			}
		}
	}
}