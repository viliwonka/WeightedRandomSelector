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

    public class RandomSelectorBuilder<T> where T : IComparable<T> {

        System.Random random;
        List<T> itemBuffer;
        List<float> weightBuffer;

        public RandomSelectorBuilder() {

            random = new System.Random();
            itemBuffer = new List<T>();
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
        /// <returns></returns>
        IRandomSelector<T> Build() {

            T[] items = itemBuffer.ToArray();
            float[] CDA = weightBuffer.ToArray();

            itemBuffer.Clear();     // potential GC problem, read if Unity clears lists or sets count to 0
            weightBuffer.Clear();

            // Use double for more precise calculation
            double Sum = 0;

            // Sum of probabilities
            for (int i = 0; i < CDA.Length; i++)
                Sum += CDA[i];

            // calculate inverse of sum and convert to float
            // optimisation (multiplying is faster than division)
            float k = (float) (1f / Sum);

            for (int i = 0; i < CDA.Length; i++) {
                // normalisation
                CDA[i] *= k;
            }

            Sum = 0;

            // Make Cummulative Distribution Array
            for (int i = 0; i < CDA.Length; i++) {

                Sum += CDA[i];
                CDA[i] = (float) Sum;
            }

            CDA[CDA.Length - 1] = 1f; //last iitem of CDA is always 1, I do this because numerical inaccurarcies add up and last item probably wont be 1

            int seed = random.Next();

            //if CDA array is smaller than 16, then pick linear search random selector
            //number 16 was calculated empirically (10 million random picks on both linear and binary to see where their performance is similar - crossing point)
            if(CDA.Length <= 16) {
            
                return new StaticRandomSelectorLinear<T>(items, CDA, seed);
            } else {
                //bigger array sizes need binary search for faster lookup
                return new StaticRandomSelectorBinary<T>(items, CDA, seed);
            }
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