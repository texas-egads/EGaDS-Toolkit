using System;

namespace egads.system.actors
{
    public interface ISelectable
    {
        #region Properties

        bool canGetSelected { get; }
        event Action isDisabled;

        #endregion

        #region Methods

        void Select();
        void Deselect();

        #endregion
    }
}