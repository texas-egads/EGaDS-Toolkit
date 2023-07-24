using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
	public class ActionBase : IActionQueueElement
	{
		#region Public Methods

		public virtual void OnStart()
		{
			// empty
		}

		public virtual void Update()
		{
			// empty
		}

		public virtual void OnExit()
		{
			// empty
		}

		public virtual bool hasEnded
		{
			get { return true; }
		}

        #endregion
    }
}