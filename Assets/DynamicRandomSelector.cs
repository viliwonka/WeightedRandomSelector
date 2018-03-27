using System.Collections;
using System.Collections.Generic;
using System;

namespace DataStructures.RandomSelector {
    using DataStructures.RandomSelector.Math;

    /// <summary>
    /// DynamicRandomSelector allows you adding or removing items.
    /// Call "Build" after you finished modification.
    /// Switches between linear or binary search after each Build(), 
    /// depending on count of items, making it more performant for general use case.
    /// </summary>
    /// <typeparam name="T">Type of items you wish this selector returns</typeparam>
    public class DynamicRandomSelector<T> : IRandomSelector<T>, IRandomSelectorBuilder<T> {
    
        System.Random random;
        
        // internal buffers
        List<T> itemsList;
        List<float> weightsList; 
        List<float> CDL; // Cummulative Distribution List
        
        // internal function that gets dynamically swapped inside Build
        private Func<List<float>, float, int> selectFunction;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="seed">Leave it -1 if you want seed to be randomly picked</param>
        /// <param name="expectedNumberOfItems">Set this if you know how much items the collection will hold, to minimize Garbage Collection</param>
        public DynamicRandomSelector(int seed = -1, int expectedNumberOfItems = 32) {
        
            if(seed == -1)
                random = new System.Random();
            else
                random = new System.Random(seed);

            itemsList   = new List<T>(expectedNumberOfItems);
            weightsList = new List<float>(expectedNumberOfItems);
            CDL         = new List<float>(expectedNumberOfItems);
        }
        
        /// <summary>
        /// Constructor, where you can preload collection with items/weights array. 
        /// </summary>
        /// <param name="items">Items that will get returned on random selections</param>
        /// <param name="weights">Un-normalized weights/chances of items, should be same length as items array</param>
        /// <param name="seed">Leave it -1 if you want seed to be randomly picked</param>
        /// <param name="expectedNumberOfItems">Set this if you know how much items the collection will hold, to minimize Garbage Collection</param>
        public DynamicRandomSelector(T[] items, float[] weights, int seed = -1, int expectedNumberOfItems = 32) : this() {
        
            for(int i = 0; i < items.Length; i++)
                Add(items[i], weights[i]);
            
            Build();
        }

        /// <summary>
        /// Constructor, where you can preload collection with items/weights list. 
        /// </summary>
        /// <param name="items">Items that will get returned on random selections</param>
        /// <param name="weights">Un-normalized weights/chances of items, should be same length as items array</param>
        /// <param name="seed">Leave it -1 if you want seed to be randomly picked</param>
        /// <param name="expectedNumberOfItems">Set this if you know how much items the collection will hold, to minimize Garbage Collection</param>
        public DynamicRandomSelector(List<T> items, List<float> weights, int seed = -1, int expectedNumberOfItems = 32) : this() {

            for (int i = 0; i < items.Count; i++)
                Add(items[i], weights[i]);

            Build();
        }
        
        /// <summary>
        /// Clears internal buffers, should make no garbage (unless internal lists hold objects that aren't referenced anywhere else)
        /// </summary>
        public void Clear() {

            itemsList.Clear();
            weightsList.Clear();
            CDL.Clear();
        }

        /// <summary>
        /// Add new item with weight into collection. Items with zero weight will be ignored.
        /// Do not add duplicate items, because removing them will be buggy (you will need to call remove for duplicates too!).
        /// Be sure to call Build() after you are done adding items.
        /// </summary>
        /// <param name="item">Item that will be returned on random selection</param>
        /// <param name="weight">Non-zero non-normalized weight</param>
        public void Add(T item, float weight) {

            // ignore zero weight items
            if (weight == 0)
                return;
                
            itemsList.Add(item);
            weightsList.Add(weight);
        }
        
        /// <summary>
        /// Remove existing item with weight into collection.
        /// Be sure to call Build() after you are done removing items.
        /// </summary>
        /// <param name="item">Item that will be removed out of collection, if found</param>
        public void Remove(T item) {

            int index = itemsList.IndexOf(item); ;

            // nothing was found
            if (index == -1)
                return;

            itemsList.RemoveAt(index);
            weightsList.RemoveAt(index);
            // no need to remove from CDL, should be rebuilt instead
        }
        
        /// <summary>
        /// Re/Builds internal CDL (Cummulative Distribution List)
        /// Must be called after modifying (calling Add or Remove), or it will break. 
        /// Switches between linear or binary search, depending on which one will be faster.
        /// Might generate some garbage (list resize) on first few builds.
        /// </summary>
        /// <param name="seed">You can specify seed for internal random gen or leave it alone</param>
        /// <returns>Returns itself</returns>
        public IRandomSelector<T> Build(int seed = -1) {

            if (itemsList.Count == 0)
                throw new Exception("Cannot build with no items.");

            // clear list and then transfer weights
            CDL.Clear();           
            for (int i = 0; i < weightsList.Count; i++)
                CDL.Add(weightsList[i]);
                
            RandomMath.BuildCumulativeDistribution(CDL);
            
            // default behavior
            // if seed wasn't specified (it is seed==-1), keep same seed - avoids garbage collection from making new random
            if(seed != -1) {
            
                // input -2 if you want to randomize seed
                if(seed == -2) {
                    seed = random.Next();
                    random = new Random(seed);
                }
                else {
                    random = new Random(seed);
                }
            }

            // RandomMath.ListBreakpoint decides where to use Linear or Binary search, based on internal buffer size
            // if CDL list is smaller than breakpoint, then pick linear search random selector, else pick binary search selector
            if (CDL.Count < RandomMath.ListBreakpoint)
                selectFunction = RandomMath.SelectIndexLinearSearch;
            else
                selectFunction = RandomMath.SelectIndexBinarySearch;
            
            return this;
        }

        /// <summary>
        /// Selects random item based on its probability.
        /// Uses linear search or binary search, depending on internal list size.
        /// </summary>
        /// <param name="randomValue">Random value from your uniform generator</param>
        /// <returns>Returns item</returns>
        public T SelectRandomItem(float randomValue) {
            return itemsList[ selectFunction(CDL, randomValue) ];
        }

        /// <summary>
        /// Selects random item based on its probability.
        /// Uses linear search or binary search, depending on internal list size.
        /// </summary>
        /// <returns>Returns item</returns>
        public T SelectRandomItem() {
            float randomValue = (float) random.NextDouble();
            return itemsList[ selectFunction(CDL, randomValue) ];
        }
    }
}