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

    public class RandomSelectorBuilder<T> : IRandomSelectorBuilder<T> {
    
        System.Random random;
        List<T> itemBuffer;
        List<float> weightBuffer;

        public RandomSelectorBuilder() {

            random       = new System.Random();
            itemBuffer   = new List<T>();
            weightBuffer = new List<float>();
        }

        // Add item with weight
        public void Add(T item, float weight) {

            itemBuffer.Add(item);
            weightBuffer.Add(weight);
        }
        
        /// <summary>
        /// Builds RandomSelector & clears internal buffers
        /// </summary>
        /// <param name="seed">Seed for random selector. If you leave it -1, the internal random will generate one.</param>
        /// <returns>Returns IRandomSelector, underlying object depends on size of items inside object.</returns>
        public IRandomSelector<T> Build(int seed = -1) {

            T[] items = itemBuffer.ToArray();
            float[] CDA = weightBuffer.ToArray();
            
            itemBuffer.Clear();     // potential GC problem, read if Unity clears lists or sets count to 0
            weightBuffer.Clear();

            RandomMath.BuildCumulativeDistribution(CDA);
            
            if(seed == -1)
                seed = random.Next();

            // 16 is break point
            // if CDA array is smaller than 16, then pick linear search random selector, else pick binary search selector
            // number 16 was calculated empirically (10 million random picks on both linear and binary to see where their performance is similar - crossing point)
            if(CDA.Length < RandomMath.ArrayBreakpoint) 
           
                return new StaticRandomSelectorLinear<T>(items, CDA, seed);
            else 
                // bigger array sizes need binary search for much faster lookup
                return new StaticRandomSelectorBinary<T>(items, CDA, seed);           
        }

        static RandomSelectorBuilder<T> _staticBuilder = new RandomSelectorBuilder<T>();
        
        // non-instance based, single threaded only
        public static IRandomSelector<T> Build(T[] itemsArray, float[] weights) {
            
            for(int i = 0; i < itemsArray.Length; i++)
                _staticBuilder.Add(itemsArray[i], weights[i]);

            return _staticBuilder.Build();
        }
        
        // non-instance based, single threaded only
        public static IRandomSelector<T> Build(List<T> itemsArray, List<float> weights) {

            for (int i = 0; i < itemsArray.Count; i++)
                _staticBuilder.Add(itemsArray[i], weights[i]);

            return _staticBuilder.Build();
        }
    }  
}