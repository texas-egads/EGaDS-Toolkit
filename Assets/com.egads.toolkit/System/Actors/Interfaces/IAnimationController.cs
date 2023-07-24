using UnityEngine;

namespace egads.system.actors
{
	public interface IAnimationController
	{
        #region Methods

        void FadeOut(float time = 1f);
        void FadeOutAfterDeath();
		void Reset();
		void SetMaterialColor(Color color);

        #endregion
    }
}