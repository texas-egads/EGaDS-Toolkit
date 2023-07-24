using UnityEngine;
using egads.tools.extensions;

namespace egads.tools.utils
{
    public static class GuiUtilities
    {
        #region Methods

        public static void DrawFullScreen(Texture image)
        {
            Rect fullScreenRect = new Rect(-1, -1, Screen.width + 2, Screen.height + 2);
            GUI.DrawTexture(fullScreenRect, image);
        }

		public static void DrawSprite(Sprite sprite, Rect rect, float texCoordsScale)
		{
			if (sprite != null)
			{
				Texture tex = sprite.texture;
				Rect texRect = sprite.textureRect;
				Rect texCoordsRect = new Rect(texRect.x / tex.width, texRect.y / tex.height, texRect.width / tex.width, texRect.height / tex.height);
				texCoordsRect = texCoordsRect.ScaleSizeBy(texCoordsScale);

				GUI.DrawTextureWithTexCoords(rect, tex, texCoordsRect);
			}
		}

		// Draw Sprite during OnGUI method using GUI.DrawTextureWithTexCoords
		// Aspect ratio is preserved
		public static void DrawClippedSprite(Sprite sprite, Rect rect, float verticalPortion)
		{
			if (sprite != null)
			{
				if (verticalPortion > 1.0f) { verticalPortion = 1.0f; }
				if (verticalPortion < 0) { verticalPortion = 0; }

				Texture tex = sprite.texture;
				Rect origin = sprite.textureRect;

				float drawRatio = rect.width / rect.height;

				float originWidth = origin.width / tex.width;
                float originHeight = origin.height / tex.height;
				float originRatio = origin.width / origin.height;
				float originX = origin.x / tex.width;
				float originY = origin.y / tex.height;

				float height = originHeight * verticalPortion;
				float width = originWidth * verticalPortion;

				if (drawRatio > originRatio) { width = height / originRatio; }

				float offsetX = (originWidth - width) * 0.5f;
				float offsety = (originHeight - height) * 0.5f;

				Rect texCoordsRect = new Rect(originX + offsetX, originY + offsety, width, height);
				GUI.DrawTextureWithTexCoords(rect, tex, texCoordsRect);
			}
		}

        #endregion
    }
}
