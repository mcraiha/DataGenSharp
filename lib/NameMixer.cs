using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public static class NameMixer
    {
        public static List<NameWeightPair> CombineNames(List<NameWeightPair> first, List<NameWeightPair> second)
        {
            int combinedSize = first.Count + second.Count;
            List<NameWeightPair> returnList = new List<NameWeightPair>(combinedSize);

            int loops = Math.Max(first.Count, second.Count);

            for (int i = 0; i < loops; i++)
            {
                if (i < first.Count)
                {
                    returnList.Add(first[i]);
                }

                if (i < second.Count)
                {
                    returnList.Add(second[i]);
                }
            }

            return returnList;
        }
    }
}