// <copyright file="TangoGestureCamera.cs" company="Google">
//
// Copyright 2015 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using Tango;
using UnityEngine;

/// <summary>
/// Pointcloud orbit camera.
/// </summary>
public class TangoGestureCamera : MonoBehaviour
{   
	private YUVTexture m_textures;
	private TangoApplication m_tangoApplication;
	public Material m_screenMaterial;

    public GameObject m_targetFollowingObject;
	
    /// <summary>
    /// Enabled based on camera type.
    /// </summary>
    /// <param name="cameraType">Enable which camera.</param>
    public void EnableCamera()
    {
         transform.position = m_targetFollowingObject.transform.position;
         transform.rotation = m_targetFollowingObject.transform.rotation;
    }
    
    /// <summary>
    /// Set up cameras.
    /// </summary>
    private void Start() 
    {
        Application.targetFrameRate = 60;
        EnableCamera();

		m_tangoApplication = FindObjectOfType<TangoApplication>();

		m_textures = m_tangoApplication.GetVideoOverlayTextureYUV();
		
		// Pass YUV textures to shader for process.
		m_screenMaterial.SetTexture("_YTex", m_textures.m_videoOverlayTextureY);
		m_screenMaterial.SetTexture("_UTex", m_textures.m_videoOverlayTextureCb);
		m_screenMaterial.SetTexture("_VTex", m_textures.m_videoOverlayTextureCr);

		m_tangoApplication.Register(this);
    }


	void Update()
	{
		double timestamp = VideoOverlayProvider.RenderLatestFrame(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR);

		GL.InvalidateState();
	}
    /// <summary>
    /// Updates, take touching event.
    /// </summary>
    private void LateUpdate()
    {
		EnableCamera();
    }

}
