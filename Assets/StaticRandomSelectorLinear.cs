
namespace DataStructures.RandomSelector {
    using DataStructures.RandomSelector.Math;

    /// <summary>
    /// Uses Linear Search for picking random items
    /// Good for small sized number of items
    /// </summary>
    /// <typeparam name="T">Type of items you wish this selector returns</typeparam>
    public class StaticRandomSelectorLinear<T> : IRandomSelector<T> {

        System.Random random;

        // internal buffers
        T[] items;
        float[] CDA;

        /// <summary>
        /// Constructor, used by StaticRandomSelectorBuilder
        /// Needs array of items and CDA (Cummulative Distribution Array). 
        /// </summary>
        /// <param name="items">Items of type T</param>
        /// <param name="CDA">Cummulative Distribution Array</param>
        /// <param name="seed">Seed for internal random generator</param>
        public StaticRandomSelectorLinear(T[] items, float[] CDA, int seed) {

            this.items = items;
            this.CDA = CDA;
            this.random = new System.Random(seed);           
        }

        /// <summary>
        /// Selects random item based on their weights.
        /// Uses linear search for random selection.
        /// </summary>
        /// <returns>Returns item</returns>
        public T SelectRandomItem(float randomValue) {
        
            return items[CDA.SelectIndexBinarySearch(randomValue)];
        }

        /// <summary>
        /// Selects random item based on their weights.
        /// Uses linear search for random selection.
        /// </summary>
        /// <param name="randomValue">Random value from your uniform generator</param>
        /// <returns>Returns item</returns>
        public T SelectRandomItem() {
        
            float randomValue = (float) random.NextDouble();
            
            return items[CDA.SelectIndexBinarySearch(randomValue)];
        }
    }
}