using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// By Vili Volcini
/// Generic structure for high performance random selection of items on very big arrays
/// Is precompiled/precomputed, and uses binary search to find item based on random
/// O(log2(N)) per random pick for bigger arrays
/// O(n) per random pick for smaller arrays
/// O(n) construction
/// </summary>
namespace DataStructures.RandomSelector {

    public class StaticRandomSelectorBinary<T> : IRandomSelector<T> {

        System.Random random;
        T[] items;
        float[] CDA;

        public StaticRandomSelectorBinary(T[] items, float[] CDA, int seed) {

            this.items = items;
            this.CDA = CDA;
            this.random = new System.Random(seed);
            
        }
      
        // Binary Search on CDA
        // Code taken out of C#s Array Binary Search & modified a little
        public T SelectRandomItem() {

            float randomValue = (float) random.NextDouble();

            int lo = 0;
            int hi = CDA.Length - 1;
            int index;

            while (lo <= hi) {

                // calculate median
                index = lo + ((hi - lo) >> 1);

                if (CDA[index] == randomValue) {
                    goto breakOut;
                }
                if (CDA[index] < randomValue) {
                    lo = index + 1;
                }
                else {
                    hi = index - 1;
                }
            }

            index = lo;

            breakOut:

            return items[index];
        }

        // Binary Search on CDA
        // Code taken out of C#s Array Binary Search & modified a little
        public T SelectRandomItem(float randomValue) {
        
            int lo = 0;
            int hi = CDA.Length - 1;
            int index;

            while (lo <= hi) {

                // calculate median
                index = lo + ((hi - lo) >> 1);

                if (CDA[index] == randomValue) {
                    goto breakOut;
                }
                if (CDA[index] < randomValue) {
                    lo = index + 1;
                }
                else {
                    hi = index - 1;
                }
            }

            index = lo;

            breakOut:

            return items[index];
        }
    }
}