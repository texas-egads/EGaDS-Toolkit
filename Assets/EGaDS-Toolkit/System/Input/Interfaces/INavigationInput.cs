
namespace egads.system.input
{
    public interface INavigationInput
    {
        #region Input Properties

        bool acceptsSecondaryButtons { get; }

        #endregion

        #region Input Methods

        void InputUp();
        void InputDown();
        void InputLeft();
        void InputRight();
        void InputEnter();
        void InputBack();

        #endregion
    }
}