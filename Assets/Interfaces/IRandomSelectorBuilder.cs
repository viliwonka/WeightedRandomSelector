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

    public interface IRandomSelectorBuilder<T> {

        IRandomSelector<T> Build(int seed=-1);
    }    
}