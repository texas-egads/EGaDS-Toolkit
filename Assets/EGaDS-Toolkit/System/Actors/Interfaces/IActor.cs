using UnityEngine;

namespace egads.system.actors
{
    #region Delegates

    public delegate void StateChanged(IActor actor, ActorState state);

    #endregion
    public interface IActor
    {
        #region Properties

        bool isAlive { get; }
        Vector3 position { get; }
        Vector2 position2D { get; }
        event StateChanged stateChanged;

        #endregion
    }
}
