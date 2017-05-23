#define NGUI
using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

#if NGUI
namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Display NGUI")]
	public class ApplyToTextureWidgetNGUI : MonoBehaviour 
	{
		public UITexture _uiTexture;
		public MediaPlayer _mediaPlayer;
		public Texture2D _defaultTexture;
		[SerializeField] bool _makePixelPerfect = false;
	
		void Update()
		{
			if (_mediaPlayer != null)
			{
				if (_mediaPlayer.TextureProducer != null)
				{
					Texture texture = _mediaPlayer.TextureProducer.GetTexture();
					if (texture != null)
					{
						if (_mediaPlayer.TextureProducer.RequiresVerticalFlip())
						{
							_uiTexture.flip = UITexture.Flip.Vertically;
						}

						_uiTexture.mainTexture = texture;
					}
				}
			}
			else
			{	
				_uiTexture.mainTexture = _defaultTexture;
			}

			if (_makePixelPerfect)
			{
				// TODO: set video texture filtering mode to POINT
				_uiTexture.MakePixelPerfect();
			}
		}
	
		public void OnDisable()
		{
		}

		void OnDestroy()
		{
			_uiTexture.mainTexture = _defaultTexture;
		}
	}
}
#endif