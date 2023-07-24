using UnityEngine;
using System.Collections.Generic;

namespace egads.tools.extensions
{
	public class ExtRandom
	{
        #region Methods

        public static void RandomizeSeed()
        {
            int seed = System.Math.Abs(((int)(System.DateTime.Now.Ticks % 2147483648L) - (int)(Time.realtimeSinceStartup + 2000f)) / ((int)System.DateTime.Now.Day - (int)System.DateTime.Now.DayOfWeek * System.DateTime.Now.DayOfYear));
            seed = System.Math.Abs((int)((Random.value * (float)System.DateTime.Now.Ticks * (float)Random.Range(0, 2)) + (Random.value * Time.realtimeSinceStartup * Random.Range(1f, 3f))) + 1);

            Random.InitState(seed);
        }

        public static bool SplitChance() => Random.Range(0, 2) == 0 ? true : false;

		public static bool Chance(int nProbabilityFactor, int nProbabilitySpace) => Random.Range(0, nProbabilitySpace) < nProbabilityFactor ? true : false;

        #endregion
    }

    public class ExtRandom<T>
	{
        #region Methods

        public static void RandomizeSeed()
        {
            int seed = System.Math.Abs(((int)(System.DateTime.Now.Ticks % 2147483648L) - (int)(Time.realtimeSinceStartup + 2000f)) / ((int)System.DateTime.Now.Day - (int)System.DateTime.Now.DayOfWeek * System.DateTime.Now.DayOfYear));
            seed = System.Math.Abs((int)((Random.value * (float)System.DateTime.Now.Ticks * (float)Random.Range(0, 2)) + (Random.value * Time.realtimeSinceStartup * Random.Range(1f, 3f))) + 1);
            
            Random.InitState(seed);
        }

		public static bool SplitChance() => Random.Range(0, 2) == 0 ? true : false;
		public static bool Chance(int nProbabilityFactor, int nProbabilitySpace) => Random.Range(0, nProbabilitySpace) < nProbabilityFactor ? true : false;
		public static T Choice(T[] array) => array[Random.Range(0, array.Length)];
		public static T Choice(List<T> list) => list[Random.Range(0, list.Count)];

		public static T WeightedChoice(T[] array, int[] nWeights)
		{
			int nTotalWeight = 0;
			for (int i = 0; i < array.Length; i++) { nTotalWeight += nWeights[i]; }

			int nChoiceIndex = Random.Range(0, nTotalWeight);
			for (int i = 0; i < array.Length; i++)
			{
				if (nChoiceIndex < nWeights[i])
				{
					nChoiceIndex = i;
					break;
				}
				nChoiceIndex -= nWeights[i];
			}

			return array[nChoiceIndex];
		}

		public static T WeightedChoice(List<T> list, int[] nWeights)
		{
			int nTotalWeight = 0;
			for (int i = 0; i < list.Count; i++) { nTotalWeight += nWeights[i]; }

			int nChoiceIndex = Random.Range(0, nTotalWeight);
			for (int i = 0; i < list.Count; i++)
			{
				if (nChoiceIndex < nWeights[i])
				{
					nChoiceIndex = i;
					break;
				}
				nChoiceIndex -= nWeights[i];
			}

			return list[nChoiceIndex];
		}

		public static T WeightedChoice(List<T> list, List<float> weights)
		{
			float totalWeight = 0;
			for (int i = 0; i < list.Count; i++) { totalWeight += weights[i]; }

			float random = Random.Range(0, totalWeight);
			float current = 0;

			for (int i = 0; i < list.Count; i++)
			{
				if (random <= weights[i] + current) { return list[i]; }
				current += weights[i];
			}

			return list[0];
		}

		public static T[] Shuffle(T[] array)
		{
			T[] shuffledArray = new T[array.Length];
			List<int> elementIndices = new List<int>(0);
			for (int i = 0; i < array.Length; i++) { elementIndices.Add(i); }

			int nArrayIndex;
			for (int i = 0; i < array.Length; i++)
			{
				nArrayIndex = elementIndices[Random.Range(0, elementIndices.Count)];
				shuffledArray[i] = array[nArrayIndex];
				elementIndices.Remove(nArrayIndex);
			}

			return shuffledArray;
		}

        #endregion
    }
}