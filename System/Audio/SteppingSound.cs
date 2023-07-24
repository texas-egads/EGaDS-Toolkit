using UnityEngine;
using egads.system.actors;

namespace egads.system.audio
{
	public class SteppingSound : MonoBehaviour
	{
        #region Public Properties

        public AudioSource source;

        #endregion

        #region Private Properties

        private Actor2D _actor;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			if (source == null) { source = GetComponent<AudioSource>(); }

			_actor = GetComponent<Actor2D>();

			source.loop = true;
			source.Stop();
		}

		private void Update()
		{
			if (_actor.isMoving)
			{
				if (!source.isPlaying) { source.Play(); }
			}
			else
			{
				if (source.isPlaying) { source.Stop(); }
			}
		}

        #endregion
    }
}