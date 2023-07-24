using UnityEngine;

namespace egads.system.input
{
	public interface IGamepadInput 
	{
        #region Movement, First Analog Stick Properties

        bool hasMovement { get; }
		Vector2 movement { get; }
		bool directionChanged { get; }
		float horizontal { get; }
		float vertical { get; }

		bool leftWasPressed { get; }
		bool rightWasPressed { get; }
		bool upWasPressed { get; }
		bool downWasPressed { get; }

        #endregion

        #region Look, Second Analog Stick Properties

        Vector2 look { get; }

        #endregion

        #region Button Properties

        bool anyButton { get; }
		bool menuButton { get; }

		bool action1WasPressed { get; }
		bool action2WasPressed { get; }
		bool action3WasPressed { get; }
		bool action4WasPressed { get; }

		bool dpadLeftWasPressed { get; }
		bool dpadRightWasPressed { get; }
		bool dpadUpWasPressed { get; }
		bool dpadDownWasPressed { get; }

		bool leftBumperWasPressed { get; }
		bool leftBumper { get; }
		bool rightBumperWasPressed { get; }
		bool rightBumper { get; }

		bool leftTriggerWasPressed { get; }
		bool leftTrigger { get; }
		bool rightTriggerWasPressed { get; }
		bool rightTrigger { get; }

        #endregion
    }
}