
namespace egads.system.actors
{
	public interface IActorTimedAction
	{
        #region Properties

        float range { get; }
		float cooldown { get; }

        #endregion

        #region Methods

        void Execute();

        #endregion
    }
}