using UnityEngine;
using UnityEditor;

namespace egads.system.actors
{
    [CustomEditor(typeof(Character2D))]
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
            Character2D actor = target as Character2D;
            actor.Kill();
        }

        private void DamageCharacter()
        {
            Character2D actor = target as Character2D;
            actor.ApplyDamage(2f);
        }

        #endregion
    }
}
