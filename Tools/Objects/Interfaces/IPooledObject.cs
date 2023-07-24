using System;

namespace egads.tools.objects
{
    public interface IPooledObject
    {
        #region Events

        event Action<IPooledObject> getsDisabled;

        #endregion

        #region Properties

        bool isUsedByObjectPool { get; set; }

        #endregion

        #region Methods

        // Used by the object pool to reset the object to its original state
        void ToggleOn();

        // Called by object pool after notification of 'getsDisabled'
        void ToggleOff();

        #endregion
    }
}