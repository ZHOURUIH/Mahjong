using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Apply To Mesh")]
	public class ApplyToMesh : MonoBehaviour 
	{
		public Vector2 _offset = Vector2.zero;
		public Vector2 _scale = Vector2.one;
		public MeshRenderer _mesh;
		public MediaPlayer _media;
		public Texture2D _defaultTexture;

		void Update()
		{
			bool applied = false;
			if (_media != null && _media.TextureProducer != null)
			{
				Texture texture = _media.TextureProducer.GetTexture();
				if (texture != null)
				{
					ApplyMapping(texture, _media.TextureProducer.RequiresVerticalFlip());
					applied = true;
				}
			}

			if (!applied)
			{
				ApplyMapping(_defaultTexture, false);
			}
		}
		
		private void ApplyMapping(Texture texture, bool requiresYFlip)
		{
			if (_mesh != null && _mesh.materials != null)
			{
				for (int i = 0; i < _mesh.materials.Length; i++)
				{
					Material mat = _mesh.materials[i];
					if( mat != null )
					{
						mat.mainTexture = texture;

						if (texture != null )
						{
							if (requiresYFlip)
							{
								mat.mainTextureScale = new Vector2(_scale.x, -_scale.y);
								mat.mainTextureOffset = Vector2.up + _offset;
							}
							else
							{
								mat.mainTextureScale = _scale;
								mat.mainTextureOffset = _offset;
							}
						}

						// Apply changes for stereo videos
						if (mat.shader.name == "Unlit/InsideSphere" && _media != null)
						{
							Helper.SetupStereoMaterial(mat, _media.m_StereoPacking, _media.m_DisplayDebugStereoColorTint);
						}
					}
				}
			}
		}

		void OnEnable()
		{
			if (_mesh == null)
			{
				_mesh = this.GetComponent<MeshRenderer>();
				if (_mesh == null)
				{
					Debug.LogWarning("[AVProVideo] No mesh renderer set or found in gameobject");
				}
			}
			
			if (_mesh != null)
			{
				Update();
			}
		}
		
		void OnDisable()
		{
			ApplyMapping(_defaultTexture, false);
		}
	}
}