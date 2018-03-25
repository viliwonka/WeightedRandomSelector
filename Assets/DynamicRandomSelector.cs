using System.Collections;
using System.Collections.Generic;
using System;

//TODO: change the comment
/// <summary>
/// By Vili Volcini
/// Generic structure for high performance random selection of items on small or very big arrays
/// Is precompiled/precomputed, and uses linear or binary search (depends on size of array) to find item based on random value
/// O(log2(N)) per random pick for bigger arrays
/// O(n) per random pick for smaller arrays
/// O(n) construction
/// </summary>
namespace DataStructures.RandomSelector {
    using DataStructures.RandomSelector.Math;

    public class DynamicRandomSelector<T> : IRandomSelector<T>, IRandomSelectorBuilder<T> {
    
        System.Random random;
        
        // internal buffers
        List<T> itemsList;
        List<float> weightsList; 
        List<float> CDL; // cummulative distribution list

        // internal function that gets dynamically swapped on each Build
        private Func<float, T> _selectRandomItem;
        
        public DynamicRandomSelector(int seed = -1, int defaultBufferSize = 32) {
        
            if(seed == -1)
                random = new System.Random();
            else
                random = new System.Random(seed);

            itemsList = new List<T>(defaultBufferSize);
            weightsList = new List<float>(defaultBufferSize);
            CDL         = new List<float>(defaultBufferSize);
        }
        
        public DynamicRandomSelector(T[] items, float[] weights, int seed) : this(seed) {
        
            for(int i = 0; i < items.Length; i++) {

                itemsList.Add(items[i]);
                weightsList.Add(weights[i]);
            }
        }
      
        public void Add(T item, float weight) {
            
        }

        public void Remove(T item) {
        
        }
        
        // clears everything, should make no garbage (unless internal lists hold objects that aren't referenced anywhere)
        public void Clear() {

            itemsList.Clear();
            weightsList.Clear();
            CDL.Clear();
        }

        // must be called after modifying the selector (adding or removing items)
        // recalculates CDL
        // might make some garbage (list resize), this is why you should pick defaultBufferSize of your flavor to reduce this garbage, but after using this object for longer time, produced garbage will reduce to zero
        // returns itself
        public IRandomSelector<T> Build(int seed=-1) {
        
            // transfer weights
            CDL.Clear();
            CDL.AddRange(weightsList);

            RandomMath.BuildCumulativeDistribution(CDL);
            
            if (seed == -1) {
                // default behavior
                // seed wasn't specified, keep same seed - avoids garbage collection from making new random
            } 
            else if(seed == -2) {
                // special functionality of DynamicRandomSelector
                seed = random.Next();
                random = new Random(seed);
            } else {
                random = new Random(seed);
            }

            // 16 is break point
            // if CDA array is smaller than 16, then pick linear search random selector, else pick binary search selector
            // number 16 was calculated empirically (10 million random picks on both linear and binary to see where their performance is similar - crossing point)
            if (CDL.Count < 16) {
                _selectRandomItem = SelectLinearSearch;
            }
            else {
                _selectRandomItem = SelectBinarySearch;
            }
            
            return this;
        }
        
        private T SelectLinearSearch(float randomValue) {

            int i = 0;

            while (i < itemsList.Count && CDL[i] < randomValue)
                i++;

            return itemsList[i];
        }

        private T SelectBinarySearch(float randomValue) {

            int lo = 0;
            int hi = CDL.Count - 1;
            int index;

            while (lo <= hi) {

                // calculate median
                index = lo + ((hi - lo) >> 1);

                if (CDL[index] == randomValue) {
                    return itemsList[index];
                }
                if (CDL[index] < randomValue) {
                    lo = index + 1;
                }
                else {
                    hi = index - 1;
                }
            }

            index = lo;
            
            return itemsList[index];
        }

        public T SelectRandomItem(float randomValue) {

            return _selectRandomItem(randomValue);
        }
        
        public T SelectRandomItem() {
        
            float randomValue = (float) random.NextDouble();

            return _selectRandomItem(randomValue);
        }
    }
}