using System.Collections;
using System.Collections.Generic;
using System;

namespace DataStructures.RandomSelector {

    /// <summary>
    /// Interface for Random Selector Builders.
    /// </summary>
    /// <typeparam name="T">Type of items that gets randomly returned</typeparam>
    public interface IRandomSelectorBuilder<T> {

        IRandomSelector<T> Build(int seed=-1);
    }    
}