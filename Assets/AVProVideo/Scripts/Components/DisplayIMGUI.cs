using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Display IMGUI")]
	[ExecuteInEditMode]
	public class DisplayIMGUI : MonoBehaviour
	{
		public MediaPlayer	_mediaPlayer;

		public bool			_displayInEditor = true;
		public ScaleMode	_scaleMode	= ScaleMode.ScaleToFit;
		public Color		_color		= Color.white;
		public bool			_alphaBlend	= false;

		public bool			_fullScreen	= true;
		public int 			_depth		= 0;	
		public float		_x			= 0.0f;
		public float		_y			= 0.0f;
		public float		_width		= 1.0f;
		public float		_height		= 1.0f;
		
		public void OnGUI()
		{
			if (_mediaPlayer == null || !_displayInEditor)
			{
				return;
			}

			bool requiresVerticalFlip = false;
			Texture texture = Texture2D.whiteTexture;
			
			if (_mediaPlayer.Info != null && !_mediaPlayer.Info.HasVideo())
			{
				texture = null;
			}

			if (_mediaPlayer.TextureProducer != null)
			{
				if (_mediaPlayer.TextureProducer.GetTexture() != null)
				{
					texture = _mediaPlayer.TextureProducer.GetTexture();
					requiresVerticalFlip = _mediaPlayer.TextureProducer.RequiresVerticalFlip();
				}
			}

			if (texture != null)
			{
				if (!_alphaBlend || _color.a > 0)
				{
					GUI.depth = _depth;
					GUI.color = _color;

					Rect rect = GetRect();

					if (requiresVerticalFlip)
					{
						GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, rect.y + (rect.height / 2)));
					}

					GUI.DrawTexture(rect, texture, _scaleMode, _alphaBlend);
				}
			}
		}

		public Rect GetRect()
		{
			Rect rect;
			if (_fullScreen)
				rect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
			else
				rect = new Rect(_x * (Screen.width-1), _y * (Screen.height-1), _width * Screen.width, _height * Screen.height);

			return rect;
		}
	}
}
