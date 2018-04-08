using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

/// <summary>
/// By Vili Volcini
/// Generic structure for high performance random selection of items on very big arrays
/// Is precompiled/precomputed, and uses binary search to find item based on random
/// O(log2(N)) per random pick for bigger arrays
/// O(n) per random pick for smaller arrays
/// O(n) construction
/// </summary>
namespace DataStructures.RandomSelector.Test {
    using DataStructures.RandomSelector.Math;

    public class RandomSelectorTests : MonoBehaviour {
    
        // Run series of tests
        void Start() {

            var result = TestEqualityOfLinearVsBinarySearch();

            Debug.Log("Do both searches (linear and binary) produce identical results? " + (result?"Yes":"No"));           

            int optimalBreakpointArray = FindOptimalBreakpointArray();
            
            Debug.Log("Optimal breakpoint for arrays is at size of " + optimalBreakpointArray);           
            
            int optimalBreakpointList = FindOptimalBreakpointList();
            
            Debug.Log("Optimal breakpoint for lists is at size of " + optimalBreakpointList);

            TestStaticSelector();
            TestDynamicSelector();
        }
        
        /// <summary>
        /// Test and compare linear and binary searches, they should return identical results
        /// </summary>
        /// <returns></returns>
        bool TestEqualityOfLinearVsBinarySearch() {
        
            var random = new System.Random();
            
            for (int i = 0; i < 1000000; i++) {

                float u = i / 999999f;
                float r = (float) random.NextDouble();

                float[] randomWeights = RandomMath.RandomWeightsArray(random, 33);
                
                RandomMath.BuildCumulativeDistribution(randomWeights);

                if (randomWeights.SelectIndexLinearSearch(1f) != randomWeights.SelectIndexBinarySearch(1f))
                    return false;

                if (randomWeights.SelectIndexLinearSearch(u) != randomWeights.SelectIndexBinarySearch(u)) {
                
                    Debug.Log("Not matching u");
                    Debug.Log(u);
                    Debug.Log(randomWeights.SelectIndexLinearSearch(u));
                    Debug.Log(randomWeights.SelectIndexBinarySearch(u));
                    
                    return false;
                }

                if (randomWeights.SelectIndexLinearSearch(r) != randomWeights.SelectIndexBinarySearch(r)) {
                
                    Debug.Log("Not matching r");
                    Debug.Log(r);
                    Debug.Log(randomWeights.SelectIndexLinearSearch(r));
                    Debug.Log(randomWeights.SelectIndexBinarySearch(r));

                    return false;
                }
            }

            return true;
        }
        
        // time both searches (linear and binary (log)), and find optimal breakpoint - where to use which for maximal performance
        int FindOptimalBreakpointArray() {
        
            int optimalBreakpoint = 2;

            var random = new System.Random();
            
            Stopwatch stopwatchLinear = new Stopwatch();
            Stopwatch stopwatchBinary = new Stopwatch();

            float lin = 0f;
            float log = 1f;
            
            // continue increasing "optimalBreakpoint" until linear becomes slower than log
            // result is around 15-16, varies a bit due to random nature of test
            while (lin <= log) {
            
                int numOfDiffArrays = 100;
                int numOfTestPerArr = 10000;

                // u = uniform grid, r = uniform random 
                float u, r;
                ///Linear Search
                stopwatchLinear.Stop();
                stopwatchLinear.Reset();

                float[] items = RandomMath.IdentityArray(optimalBreakpoint);
                float selectedItem; //here just to simulate selecting from array
                float[] arr = new float[optimalBreakpoint];

                for (int k = 0; k < numOfDiffArrays; k++) {

                    RandomMath.RandomWeightsArray(ref arr, random);
                    RandomMath.BuildCumulativeDistribution(arr);

                    stopwatchLinear.Start();
                    for (int i = 0; i < numOfTestPerArr; i++) {

                        u = i / (numOfTestPerArr - 1f);
                        selectedItem = items[arr.SelectIndexLinearSearch(u)];

                        r = (float) random.NextDouble();
                        selectedItem = items[arr.SelectIndexLinearSearch(r)];
                    }

                    stopwatchLinear.Stop();
                }

                lin = stopwatchLinear.ElapsedMilliseconds;

                /// Binary Search
                stopwatchBinary.Stop();
                stopwatchBinary.Reset();
                
                for (int k = 0; k < numOfDiffArrays; k++) {

                    RandomMath.RandomWeightsArray(ref arr, random);
                    RandomMath.BuildCumulativeDistribution(arr);

                    stopwatchBinary.Start();
                    for (int i = 0; i < numOfTestPerArr; i++) {

                        u = i / (numOfTestPerArr - 1f);
                        selectedItem = items[arr.SelectIndexBinarySearch(u)];

                        r = (float) random.NextDouble();
                        selectedItem = items[arr.SelectIndexBinarySearch(r)];
                    }
                    stopwatchBinary.Stop();
                }

                log = stopwatchBinary.ElapsedMilliseconds;

                optimalBreakpoint++;
            }
            
            return optimalBreakpoint;
        }

        //same as before, but for lists instead of arrays
        int FindOptimalBreakpointList() {

            int optimalBreakpoint = 2;

            var random = new System.Random();

            Stopwatch stopwatchLinear = new Stopwatch();
            Stopwatch stopwatchBinary = new Stopwatch();

            float lin = 0f;
            float log = 1f;

            // continue increasing "optimalBreakpoint" until linear becomes slower than log
            // result is around 15-16, varies a bit due to random nature of test
            while (lin <= log) {

                int numOfDiffArrays = 100;
                int numOfTestPerArr = 10000;

                // u = uniform grid, r = uniform random 
                float u, r;
                ///Linear Search
                stopwatchLinear.Stop();
                stopwatchLinear.Reset();

                List<float> items = RandomMath.IdentityList(optimalBreakpoint);

                float selectedItem; //simulate selecting from array

                List<float> list = new List<float>(optimalBreakpoint);
                
                for(int i = 0; i < optimalBreakpoint; i++)
                    list.Add(0f);
                
                for (int k = 0; k < numOfDiffArrays; k++) {

                    RandomMath.RandomWeightsList(ref list, random);
                    RandomMath.BuildCumulativeDistribution(list);

                    stopwatchLinear.Start();
                    for (int i = 0; i < numOfTestPerArr; i++) {

                        u = i / (numOfTestPerArr - 1f);
                        selectedItem = items[list.SelectIndexLinearSearch(u)];

                        r = (float) random.NextDouble();
                        selectedItem = items[list.SelectIndexLinearSearch(r)];
                    }

                    stopwatchLinear.Stop();
                }

                lin = stopwatchLinear.ElapsedMilliseconds;

                /// Binary Search
                stopwatchBinary.Stop();
                stopwatchBinary.Reset();
                
                for (int k = 0; k < numOfDiffArrays; k++) {

                    RandomMath.RandomWeightsList(ref list, random);
                    RandomMath.BuildCumulativeDistribution(list);

                    stopwatchBinary.Start();
                    for (int i = 0; i < numOfTestPerArr; i++) {

                        u = i / (numOfTestPerArr - 1f);
                        selectedItem = items[list.SelectIndexBinarySearch(u)];

                        r = (float) random.NextDouble();
                        selectedItem = items[list.SelectIndexBinarySearch(r)];
                    }
                    stopwatchBinary.Stop();

                }

                log = stopwatchBinary.ElapsedMilliseconds;

                optimalBreakpoint++;
            }

            return optimalBreakpoint;
        }

        void TestStaticSelector() {

            System.Random r = new System.Random();

            RandomSelectorBuilder<float> builder = new RandomSelectorBuilder<float>();

            // add items
            // pair (item, unnormalized probability)
            for(int i = 0; i < 32; i++)
                builder.Add(i, Mathf.Sqrt(i+1));

            //build with seed 42
            IRandomSelector<float> selector = builder.Build(42);

            string print = "";

            for(int i = 0; i < 100; i++) {
                print += i.ToString() + ". " + selector.SelectRandomItem() + "\n";
            }

            Debug.Log(print);

            /// LONG version, to test binary search
            // add items
            // pair (item, unnormalized probability)
            for (int i = 0; i < 1024; i++)
                builder.Add(i, Mathf.Sqrt(i + 1));

            //build with seed 42
            IRandomSelector<float> longSelector = builder.Build(42);

            // just run 10000 tests, should be enough
            for (int i = 0; i < 10000; i++)
                longSelector.SelectRandomItem();

            // wont print long version, would spam console too much
        }

        void TestDynamicSelector() {

            //seed = 42, expected number of item = 32
            DynamicRandomSelector<float> selector = new DynamicRandomSelector<float>(42, 32);

            // add items
            for (int i = 0; i < 32; i++)
                selector.Add(i, Mathf.Sqrt(i + 1));

            // Build internals
            // pair (item, unnormalized probability)
            selector.Build();

            string print = "";

            for (int i = 0; i < 100; i++) {
                print += i.ToString() + " " + selector.SelectRandomItem() + "\n";
            }

            Debug.Log(print);

            /// LONG version, to test binary search
            
            //we can just keep adding new members
            for(int i = 0; i < 1024; i++) {
                selector.Add(i, Mathf.Sqrt(i));
            }

            //do not forget to (re)build
            selector.Build();

            //just test it this way
            for(int i = 0; i < 10000; i++) {
                selector.SelectRandomItem();
            }
            // wont print long version, would spam console too much
        }
    }
}