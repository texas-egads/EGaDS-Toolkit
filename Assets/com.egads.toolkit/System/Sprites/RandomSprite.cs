using UnityEngine;
using System.Collections.Generic;
using egads.tools.extensions;

namespace egads.system.sprites
{
	public class RandomSprite : MonoBehaviour
	{
        #region Public Properties

        public List<Sprite> images;

        #endregion

        #region Unity Methods

        void Awake()
		{
			GetComponent<SpriteRenderer>().sprite = images.PickRandom();
		}

        #endregion
    }
}