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
    using DataStructures.RandomSelector.Math;

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
        public T SelectRandomItem() {

            float randomValue = (float) random.NextDouble();
            
            return items[ CDA.SelectIndexBinarySearch(randomValue) ];
        }

        // Binary Search on CDA
        public T SelectRandomItem(float randomValue) {
        
            return items[ CDA.SelectIndexBinarySearch(randomValue) ];
        }
    }
}