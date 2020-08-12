using System.Collections.Generic;
namespace KyleConibear
{
    public static class FibonacciSequence
    {
        private static List<int> sequence = new List<int>();
        private static int sequenceLastIndex
        {
            get
            {
                return sequence.Count - 1;
            }
        }

        public static int GetValueAtIndex(int index)
        {
            // If the count is less than 2 the sequence has not been initialized
            if (sequence.Count < 2)
            {
                sequence.Add(0);
                sequence.Add(1);
            }

            // The wanted index is larger than the cached amount
            if (index > sequenceLastIndex)
            {
                // Add to the cached sequence up to the desired index
                for (int i = sequenceLastIndex; i < index; i++)
                {
                    AddValueToSequence();
                }
            }
            return sequence[index];
        }

        private static void AddValueToSequence()
        {
            int aVal = sequence[sequenceLastIndex - 1];
            int bVal = sequence[sequenceLastIndex];

            sequence.Add(aVal + bVal);
        }
    }
}