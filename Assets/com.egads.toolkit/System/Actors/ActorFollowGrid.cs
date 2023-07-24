using UnityEngine;
using egads.system.pathFinding;

namespace egads.system.actors
{
	public class ActorFollowGrid : MonoBehaviour
	{
        #region Unity Methods

        void Start()
		{
			Actor2D actor = GetComponent<Actor2D>();
			actor.target.SetPathField(FindObjectOfType<LevelGrid>());
		}

        #endregion
    }
}