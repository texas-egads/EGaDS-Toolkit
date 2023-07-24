﻿using UnityEngine;

namespace egads.tools.extensions
{

	public static class RendererExtensions
	{
        #region Methods

        /// <summary>
        /// Adds a visibility (view frustum) check to a renderer component
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}

        #endregion
    }

}