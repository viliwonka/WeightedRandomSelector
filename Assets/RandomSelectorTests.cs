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
    
        void Start() {

            var result = TestEqualityOfLinearVsBinarySearch();

            Debug.Log("Do both searches (linear and binary) produce identical results? " + (result?"Yes":"No"));
            
            int optimalBreakpointArray = FindOptimalBreakpointArray();

            Debug.Log("Optimal breakpoint for arrays is at size of " + optimalBreakpointArray);
            
            int optimalBreakpointList = FindOptimalBreakpointList();

            Debug.Log("Optimal breakpoint for lists is at size of " + optimalBreakpointList);
        }
        

        // test both searches, they should return identical indexes for all random values
        bool TestEqualityOfLinearVsBinarySearch() {
        
            var randomSelector = RandomSelectorBuilder<float>.Build(

                new float[] { 1f  , 2f,   3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f }, 
                new float[] { 0.5f, 1f, 0.5f, 1f, 1f, 2f, 3f, 1f, 0.1f,1f }
            );

            var random = new System.Random();

            for (int i = 0; i < 1000000; i++) {

                float u = i / 999999f;
                float r = (float) random.NextDouble();

                float[] randomWeights = RandomMath.RandomWeightsArray(random, 33);
                
                RandomMath.BuildCumulativeDistribution(randomWeights);
                
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

    }
}