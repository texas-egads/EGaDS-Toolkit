using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
	/// <summary>
	/// Describes an action that lasts a certain duration and can be added to a chronological queue
	/// </summary>
	public interface IActionQueueElement
	{
        #region Properties

        bool hasEnded { get; }

        #endregion

        #region Methods

        void OnStart();
		void Update();
		void OnExit();

        #endregion
    }
}