using System.Collections;
using System.Collections.Generic;
using System;


namespace DataStructures.RandomSelector {

    /// <summary>
    /// Interface for Random selector
    /// </summary>
    /// <typeparam name="T">Type of items that gets randomly returned</typeparam>
    public interface IRandomSelector<T> {

        T SelectRandomItem();
        T SelectRandomItem(float randomValue);
    }
}