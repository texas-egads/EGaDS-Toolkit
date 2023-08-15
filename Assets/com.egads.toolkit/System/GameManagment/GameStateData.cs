using System.Collections.Generic;
using egads.tools.json;

namespace egads.system.gameManagement
{
    /// <summary>
    /// Represents the data associated with the game state.
    /// </summary>
    public class GameStateData
    {
        #region Public Properties

        /// <summary>
        /// Stores a collection of unique integer values that were used.
        /// </summary>
        public HashSet<int> wasUsed = new HashSet<int>();

        #endregion

        #region Loading and Saving

        /// <summary>
        /// Gets the JSON representation of the GameStateData.
        /// </summary>
        /// <returns>JSON object representing the data.</returns>
        public JSONObject GetJSON()
        {
            JSONObject data = new JSONObject();

            JSONArray wasUsedArray = new JSONArray();
            foreach (var item in wasUsed) { wasUsedArray.Add(item); }
            data.Add("wasUsed", wasUsedArray);

            return data;
        }

        /// <summary>
        /// Restores the GameStateData from a JSON object.
        /// </summary>
        /// <param name="data">JSON object containing the data.</param>
        public void FromJSON(JSONObject data)
        {
            // Clear old values
            Reset();

            JSONArray wasUsedArray = data.GetArray("wasUsed");
            for (int i = 0; i < wasUsedArray.Length; i++) { wasUsed.Add((int)wasUsedArray[i].Number); }
        }

        /// <summary>
        /// Resets the GameStateData by clearing stored values.
        /// </summary>
        public void Reset()
        {
            wasUsed.Clear();
        }

        #endregion
    }
}
