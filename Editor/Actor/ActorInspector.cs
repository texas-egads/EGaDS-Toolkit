using UnityEngine;
using UnityEditor;

namespace egads.system.actors
{
    [CustomEditor(typeof(Actor2D))]
    public class ActorInspector : Editor
    {
        #region Public Methods

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Hit with Damage", GUILayout.Height(40f)))
                {
                    DamageCharacter();
                }

                if (GUILayout.Button("Kill", GUILayout.Height(40f)))
                {
                    KillCharacter();
                }
            }
        }

        #endregion

        #region Private Methods

        private void KillCharacter()
        {
            Actor2D actor = target as Actor2D;
            actor.Kill();
        }

        private void DamageCharacter()
        {
            Actor2D actor = target as Actor2D;
            actor.ApplyDamage(2f);
        }

        #endregion
    }
}
