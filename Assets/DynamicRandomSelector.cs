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

    public class DynamicRandomSelector<T> : IRandomSelector<T> {
    
        System.Random random;
        
        List<T> itemsList;
        List<float> weightsList; 
        List<float> CDL; //cummulative distribution list
        
        public DynamicRandomSelector(int seed) {

            this.random = new System.Random(seed);

            itemsList = new List<T>();
            weightsList = new List<float>();
            CDL         = new List<float>();
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
        

        //clears everything out of selector
        public void Clear() {
            itemsList.Clear();
            weightsList.Clear();
            CDL.Clear();
        }

        // must be called after modifying the selector (adding or removing items)
        // recalculates CDL
        void Rebuild() {

        }

        public T SelectRandomItem(float randomValue) {
            
            int i = 0;

            while (i < CDL.Count && CDL[i] < randomValue)
                i++;
            
            return itemsList[i];
        }
        
        public T SelectRandomItem() {

            float randomValue = (float) random.NextDouble();

            int i = 0;

            while (i < CDL.Count && CDL[i] < randomValue)
                i++;

            return itemsList[i];
        }
    }
}