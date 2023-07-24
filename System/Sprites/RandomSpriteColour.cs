using UnityEngine;
using Random = UnityEngine.Random;

namespace egads.system.sprites
{
	public class RandomSpriteColour : MonoBehaviour
	{
		#region Public Properties

		public Color fromColor;
		public Color toColor;

		#endregion

		#region Unity Methods

		public void Awake()
		{
			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
			spriteRenderer.color = Color.Lerp(fromColor, toColor, Random.Range(0, 1f));
		}

        #endregion
    }
}