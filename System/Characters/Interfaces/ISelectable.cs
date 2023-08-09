using System;

namespace egads.system.characters
{
    /// <summary>
    /// Interface representing a selectable object in the game.
    /// </summary>
    public interface ISelectable
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the object can be selected.
        /// </summary>
        bool canGetSelected { get; }

        /// <summary>
        /// Event triggered when the object becomes disabled or unselectable.
        /// </summary>
        event Action isDisabled;

        #endregion

        #region Methods

        /// <summary>
        /// Selects the object.
        /// </summary>
        void Select();

        /// <summary>
        /// Deselects the object.
        /// </summary>
        void Deselect();

        #endregion
    }
}
