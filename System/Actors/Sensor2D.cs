using egads.tools.extensions;
using System.Collections.Generic;
using UnityEngine;

namespace egads.system.actors
{
    public class Sensor2D : MonoBehaviour
    {
        #region Sensor Events

        public delegate void SensorEventDelegate(SensorEvent type, IActor actor);
        public event SensorEventDelegate sensorEvent;

        #endregion

        #region Public Properties

        public string searchTag = "";
        public List<IActor> actors = new List<IActor>();
        public bool hasActorsDetected => actors.Count > 0;

        #endregion

        #region Private Properties

        protected Transform _transform;
        protected IActor _self;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _transform = transform;

            _self = _transform.GetInterface<IActor>();
            if (_self == null && _transform.parent != null) { _self = _transform.parent.GetInterface<IActor>(); }    
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {                
                IActor actor = other.transform.GetInterface<IActor>();
                if (actor != null && actor != _self && !actors.Contains(actor) && actor.isAlive)
                {
                    actors.Add(actor);

                    actor.stateChanged += Actor_StateChanged;

                    if (sensorEvent != null) { sensorEvent(SensorEvent.ActorDetected, actor); }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {
                IActor actor = other.transform.GetInterface<IActor>();
                Remove(actor);
            }
        }

        #endregion

        #region Private Methods

        private void Actor_StateChanged(IActor actor, ActorState state)
        {
            if (!actor.isAlive)
            {
                if (actors.Contains(actor))
                {
                    Remove(actor);
                }
                else
                {
                    actor.stateChanged -= Actor_StateChanged;
                }
            }
        }

        private void Remove(IActor actor)
        {
            if (actor != null && actors.Contains(actor))
            {
                actors.Remove(actor);

                actor.stateChanged -= Actor_StateChanged;

                if (sensorEvent != null)
                    sensorEvent(SensorEvent.ActorLeft, actor);
            }
        }

        private void UpdateList()
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] == null)
                {
                    actors.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion
    }
}
