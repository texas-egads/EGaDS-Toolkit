using UnityEngine;
using System.Collections.Generic;

namespace egads.system.actors
{
	public class Sensors : MonoBehaviour
	{
        #region Sensor Events

        public delegate void SensorEventDelegate(SensorEvent type, Actor2D otherActor);
		public event SensorEventDelegate sensorEvent;

        #endregion

        #region Public Properties

        public string Tag = "Player";
		public List<Actor2D> actors = new List<Actor2D>();
		public bool ActorsDetected => actors.Count > 0;

        #endregion

        #region Private Properties

        private Transform _transform;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			_transform = transform;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == Tag)
			{
				Actor2D actor = other.GetComponent<Actor2D>();
				if (actor != null && !actors.Contains(actor) && actor.isAlive)
				{
					actors.Add(actor);

					actor.stateChanged += Actor_StateChanged;

					if (sensorEvent != null) { sensorEvent(SensorEvent.ActorDetected, actor); }	
				}
			}
		}

		private void Actor_StateChanged(IActor actor, ActorState state)
		{
			if (!actor.isAlive)
			{
				if (actors.Contains(actor as Actor2D)) { RemoveActor(actor as Actor2D); }
				else { actor.stateChanged -= Actor_StateChanged; }
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.tag == Tag)
			{
				Actor2D actor = other.GetComponent<Actor2D>();
				RemoveActor(actor);
			}
		}

        #endregion

        #region Public Methods

        public Actor2D GetMostWoundedActor()
		{
			UpdateList();

			if (actors.Count == 0) { return null; }

			Actor2D targetActor = actors[0];
			float woundAmount = targetActor.health.missingAmount;

			for (int i = 1; i < actors.Count; i++)
			{
				float newHealthAmount = actors[i].health.missingAmount;
				if (newHealthAmount > woundAmount)
				{
					woundAmount = newHealthAmount;
					targetActor = actors[i];
				}
			}

			return targetActor;
		}

		public Actor2D GetNearestActor()
		{
			UpdateList();

			if (actors.Count == 0) { return null; }

			Vector2 position = _transform.position;
			Actor2D nearestActor = actors[0];
			float foundDistance = (position - nearestActor.position2D).sqrMagnitude;

			for (int i = 1; i < actors.Count; i++)
			{
				float newDistance = (position - actors[i].position2D).sqrMagnitude;
				if (newDistance < foundDistance)
				{
					foundDistance = newDistance;
					nearestActor = actors[i];
				}
			}

			return nearestActor;
		}

        #endregion

        #region Private Methods

        private void RemoveActor(Actor2D actor)
		{
			if (actor != null && actors.Contains(actor))
			{
				actors.Remove(actor);

				actor.stateChanged -= Actor_StateChanged;

				if (sensorEvent != null) { sensorEvent(SensorEvent.ActorLeft, actor); }
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