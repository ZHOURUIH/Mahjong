using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	/// <summary>
	/// Builds a cube mesh for displaying a 360 degree "Cubemap 3x2 facebook layout" texture in VR
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	//[ExecuteInEditMode]
	[AddComponentMenu("AVPro Video/Cubemap Cube (VR)")]
	public class CubemapCube : MonoBehaviour
	{
		private Mesh _mesh;
		private MeshRenderer _renderer;

		[SerializeField]
		private Material _material = null;

		[SerializeField]
		private MediaPlayer _mediaPlayer;

		// This value comes from the facebook transform ffmpeg filter and is used to prevent seams appearing along the edges due to bilinear filtering
		[SerializeField]
		private float expansion_coeff = 1.01f;
		
		public MediaPlayer Player
		{
			set { _mediaPlayer = value; }
			get { return _mediaPlayer; }
		}

		void Start()
		{
			if (_mesh == null)
			{
				_mesh = new Mesh();
				_mesh.MarkDynamic();
				MeshFilter filter = this.GetComponent<MeshFilter>();
				if (filter != null)
				{
					filter.mesh = _mesh;
				}
				_renderer = this.GetComponent<MeshRenderer>();
				if (_renderer != null)
				{
					_renderer.material = _material;
				}
				BuildMesh();
			}
		}

		void OnDestroy()
		{
			if (_mesh != null)
			{
				MeshFilter filter = this.GetComponent<MeshFilter>();
				if (filter != null)
				{
					filter.mesh = null;
				}

#if UNITY_EDITOR
				Mesh.DestroyImmediate(_mesh);
#else
				Mesh.Destroy(_mesh);
#endif
				_mesh = null;
			}

			if (_renderer != null)
			{
				_renderer.material = null;
				_renderer = null;
			}
		}
		
		void Update()
		{
			if (Application.isPlaying)
			{
				Texture texture = null;
				if (_mediaPlayer != null && _mediaPlayer.Control != null)
				{
					if (_mediaPlayer.TextureProducer != null)
					{
						texture = _mediaPlayer.TextureProducer.GetTexture();
					}
				}

				UpdateMaterial(texture);
				if (texture != null)
				{
					UpdateMeshUV(texture.width, texture.height, _mediaPlayer.TextureProducer.RequiresVerticalFlip());
				}
			}
		}
		
		private void UpdateMaterial(Texture texture)
		{
			_renderer.material.mainTexture = texture;
		}

		private void BuildMesh()
		{
			Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);
			Vector3[] v = new Vector3[]
			{
				// Left
				new Vector3(0f,-1f,0f) - offset,
				new Vector3(0f,0f,0f) - offset,
				new Vector3(0f,0f,-1f) - offset,
				new Vector3(0f,-1f,-1f) - offset,
				// Front
				new Vector3(0f,0f,0f) - offset,
				new Vector3(-1f,0f,0f) - offset,
				new Vector3(-1f,0f,-1f) - offset,
				new Vector3(0f,0f,-1f) - offset,
				// Right
				new Vector3(-1f,0f,0f) - offset,
				new Vector3(-1f,-1f,0f) - offset,
				new Vector3(-1f,-1f,-1f) - offset,
				new Vector3(-1f,0f,-1f) - offset,
				// Back
				new Vector3(-1f,-1f,0f) - offset,
				new Vector3(0f,-1f,0f) - offset,
				new Vector3(0f,-1f,-1f) - offset,
				new Vector3(-1f,-1f,-1f) - offset,
				// Bottom
				new Vector3(0f,-1f,-1f) - offset,
				new Vector3(0f,0f,-1f) - offset,
				new Vector3(-1f,0f,-1f) - offset,
				new Vector3(-1f,-1f,-1f) - offset,
				// Top
				new Vector3(-1f,-1f,0f) - offset,
				new Vector3(-1f,0f,0f) - offset,
				new Vector3(0f,0f,0f) - offset,
				new Vector3(0f,-1f,0f) - offset,
			};

			Matrix4x4 rot = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(-90f, Vector3.right), Vector3.one);
			for (int i = 0; i < v.Length; i++)
			{
				v[i] = rot.MultiplyPoint(v[i]);
			}

			_mesh.vertices = v;

			_mesh.triangles = new int[]
			{
				0,1,2,
				0,2,3,
				4,5,6,
				4,6,7,
				8,9,10,
				8,10,11,
				12,13,14,
				12,14,15,
				16,17,18,
				16,18,19,
				20,21,22,
				20,22,23,
			};

			_mesh.normals = new Vector3[]
			{
				// Left
				new Vector3(-1f,0f,0f),
				new Vector3(-1f,0f,0f),
				new Vector3(-1f,0f,0f),
				new Vector3(-1f,0f,0f),
				// Front
				new Vector3(0f,-1f,0f),
				new Vector3(0f,-1f,0f),
				new Vector3(0f,-1f,0f),
				new Vector3(0f,-1f,0f),
				// Right
				new Vector3(1f,0f,0f),
				new Vector3(1f,0f,0f),
				new Vector3(1f,0f,0f),
				new Vector3(1f,0f,0f),
				// Back
				new Vector3(0f,1f,0f),
				new Vector3(0f,1f,0f),
				new Vector3(0f,1f,0f),
				new Vector3(0f,1f,0f),
				// Bottom
				new Vector3(0f,0f,1f),
				new Vector3(0f,0f,1f),
				new Vector3(0f,0f,1f),
				new Vector3(0f,0f,1f),
				// Top
				new Vector3(0f,0f,-1f),
				new Vector3(0f,0f,-1f),
				new Vector3(0f,0f,-1f),
				new Vector3(0f,0f,-1f)
			};

			UpdateMeshUV(512, 512, false);
		}

		private void UpdateMeshUV(int textureWidth, int textureHeight, bool flipY)
		{
			float texWidth = textureWidth;
			float texHeight = textureHeight;

			float blockWidth = texWidth / 3f;
			float pixelOffset = Mathf.Floor(((expansion_coeff * blockWidth) - blockWidth) / 2f);

			float wO = pixelOffset / texWidth;
			float hO = pixelOffset / texHeight;

			const float third = 1f / 3f;
			const float half = 1f / 2f;

			Vector2[] uv = new Vector2[]
			{
				//left
				new Vector2(third+wO,1-hO),
				new Vector2((third*2)-wO, 1-hO),
				new Vector2((third*2)-wO, half+hO),
				new Vector2(third+wO, half+hO),

				//front
				new Vector2(third+wO, half-hO),
				new Vector2((third*2)-wO, half-hO),
				new Vector2((third*2)-wO, 0f+hO),
				new Vector2(third+wO, 0f+hO),

				//right
				new Vector2(0+wO, 1f-hO),
				new Vector2(third-wO, 1f-hO),
				new Vector2(third-wO, half+hO),
				new Vector2(0+wO, half+hO),

				//back
				new Vector2((third*2)+wO, half-hO),
				new Vector2(1-wO, half-hO),
				new Vector2(1-wO, 0+hO),
				new Vector2((third*2)+wO, 0+hO),

				//bottom
				new Vector2(0+wO, 0+hO),
				new Vector2(0+wO, half-hO),
				new Vector2(third-wO, half-hO),
				new Vector2(third-wO, 0+hO),

				//top
				new Vector2(1-wO, 1-hO),
				new Vector2(1-wO, half+hO),
				new Vector2((third*2)+wO, half+hO),
				new Vector2((third*2)+wO, 1-hO)
			};
			
			if (flipY)
			{
				for (int i = 0; i < uv.Length; i++)
				{
					uv[i].y = 1f - uv[i].y;
				}
			}

			_mesh.uv = uv;
			_mesh.UploadMeshData(false);
		}
	}
}
