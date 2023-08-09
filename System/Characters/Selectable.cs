using UnityEngine;
using egads.tools.extensions;

namespace egads.system.characters
{
    /// <summary>
    /// Component that allows GameObjects to be selectable and provides settings for searching for the selectable interface in the current object or its parent/children.
    /// </summary>
    public class Selectable : MonoBehaviour
    {
        #region Selectable Settings

        /// <summary>
        /// Enumeration representing different search options for finding the selectable interface.
        /// </summary>
        public enum SearchForSelectable
        {
            /// <summary>
            /// Search for the selectable interface in the current GameObject.
            /// </summary>
            InCurrent,

            /// <summary>
            /// Search for the selectable interface in the parent GameObject and its children.
            /// </summary>
            InParent,

            /// <summary>
            /// Search for the selectable interface in the children GameObjects.
            /// </summary>
            InChildren,
        }

        /// <summary>
        /// Reference to the selectable interface of the GameObject.
        /// </summary>
        public ISelectable selectable = null;

        /// <summary>
        /// The search option for finding the selectable interface.
        /// </summary>
        [SerializeField]
        private SearchForSelectable searchType = SearchForSelectable.InCurrent;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// Search for the selectable interface based on the chosen search type.
        /// </summary>
        private void Awake()
        {
            switch (searchType)
            {
                case SearchForSelectable.InCurrent:
                    // Search for the selectable interface in the current GameObject.
                    selectable = transform.GetInterface<ISelectable>();
                    break;

                case SearchForSelectable.InParent:
                    // Search for the selectable interface in the parent GameObject and its children.
                    selectable = transform.GetInterfaceInParentAndChildren<ISelectable>();
                    break;

                case SearchForSelectable.InChildren:
                    // Search for the selectable interface in the children GameObjects.
                    selectable = transform.GetInterfaceInChildren<ISelectable>();
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
