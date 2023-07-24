using UnityEngine;
using egads.tools.extensions;

namespace egads.system.actors
{
    public class Selectable : MonoBehaviour
    {
        #region Selectable Settings

        public enum SearchForSelectable
        {
            InCurrent,
            InParent,
            InChildren,
        }

        public ISelectable selectable = null;

        [SerializeField]
        private SearchForSelectable searchType = SearchForSelectable.InCurrent;

        #endregion

        #region Unity Methods

        public void Awake()
        {
            switch (searchType)
            {
                case SearchForSelectable.InCurrent:
                    selectable = transform.GetInterface<ISelectable>();
                    break;

                case SearchForSelectable.InParent:
                    selectable = transform.GetInterfaceInParentAndChildren<ISelectable>();
                    break;

                case SearchForSelectable.InChildren:
                    selectable = transform.GetInterfaceInChildren<ISelectable>();
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}