using System.Collections.Generic;
using egads.tools.json;

namespace egads.system.gameManagement
{
	public class GameStateData
	{
        #region Public Properties

        public HashSet<int> wasUsed = new HashSet<int>();

        #endregion

        #region Loading and Saving

        public JSONObject GetJSON()
		{
			JSONObject data = new JSONObject();

			JSONArray wasUsedArray = new JSONArray();
			foreach (var item in wasUsed) { wasUsedArray.Add(item); }
			data.Add("wasUsed", wasUsedArray);

			return data;
		}

		public void FromJSON(JSONObject data)
		{
			// Clear old values
			Reset();

			JSONArray wasUsedArray = data.GetArray("wasUsed");
			for (int i = 0; i < wasUsedArray.Length; i++) { wasUsed.Add((int)wasUsedArray[i].Number); }
		}

		public void Reset()
		{
			wasUsed.Clear();
		}

		#endregion
	}
}