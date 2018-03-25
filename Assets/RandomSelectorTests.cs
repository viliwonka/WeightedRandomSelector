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
namespace DataStructures.RandomSelector {

    public class RandomSelectorTests : MonoBehaviour {
    
        void Start() {

            var result = TestLinearVsBinarySearch();

            Debug.Log("Do both functions match?:" + result);
            
            int optimalBreakpoint = FindOptimalBreakpoint();

            Debug.Log("Optimal breakpoint is at array size of " + optimalBreakpoint);
        }

        // test both searches, they should return identical results
        bool TestLinearVsBinarySearch() {

            var randomSelector = RandomSelectorBuilder<float>.Build(

                new float[] { 1f  , 2f,   3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f }, 
                new float[] { 0.5f, 1f, 0.5f, 1f, 1f, 2f, 3f, 1f, 0.1f,1f }
            );

            var random = new System.Random();

            for (int i = 0; i < 1000000; i++) {

                float u = i / 999999f;
                float r = (float) random.NextDouble();

                if (randomSelector.SelectRandomItemLinearSearch(u).CompareTo(randomSelector.SelectRandomItemBinarySearch(u)) != 0) {
                
                    Debug.Log("Not matching u");
                    Debug.Log(u);
                    Debug.Log(randomSelector.SelectRandomItemLinearSearch(u));
                    Debug.Log(randomSelector.SelectRandomItemBinarySearch(u));
                    
                    return false;
                }

                if (randomSelector.SelectRandomItemLinearSearch(r).CompareTo(randomSelector.SelectRandomItemBinarySearch(r)) != 0) {

                    Debug.Log("Not matching r");
                    Debug.Log(r);
                    Debug.Log(randomSelector.SelectRandomItemLinearSearch(r));
                    Debug.Log(randomSelector.SelectRandomItemBinarySearch(r));

                    return false;
                }
            }

            return true;
        }
        

        float[] IdentityArray(int length) {

            float[] array = new float[length];

            for (int i = 0; i < array.Length; i++)
                array[i] = i;
            
            return array;
        }

        float[] RandomArray(int length) {

            var r = new System.Random();

            float[] array = new float[length];

            for(int i = 0; i < array.Length; i++)
                array[i] = (float) r.NextDouble();

            return array;
        }
        
        // time both searches (linear and binary (log)), and find optimal breakpoint - where to use which for maximal performance
        int FindOptimalBreakpoint() {

            int optimalBreakpoint = 2;

            var random = new System.Random();
            
            Stopwatch stopwatchLinear = new Stopwatch();
            Stopwatch stopwatchBinary = new Stopwatch();

            float lin = 0f;
            float log = 1f;

            // continue increasing "optimalBreakpoint" until linear becomes slower than log
            // result is around 15-16, varies a bit due to random nature of test
            while (lin <= log) {

                var linearRandomSelector = RandomSelectorBuilder<float>.Build(IdentityArray(optimalBreakpoint), RandomArray(optimalBreakpoint));
                var binaryRandomSelector = RandomSelectorBuilder<float>.Build(IdentityArray(optimalBreakpoint), RandomArray(optimalBreakpoint));

                int numOfTests = 10000000;

                float u, r;
                ///Linear Search
                stopwatchLinear.Reset();
                stopwatchLinear.Start();

                for(int i = 0; i < numOfTests; i++) {

                    u = i / (numOfTests - 1f);
                    linearRandomSelector.SelectRandomItem(u);
                    
                    r = (float)random.NextDouble();
                    linearRandomSelector.SelectRandomItem(r);
                }

                stopwatchLinear.Stop();

                lin = stopwatchLinear.ElapsedMilliseconds;

                ///Binary Search
                stopwatchBinary.Reset();
                stopwatchBinary.Start();

                for (int i = 0; i < numOfTests; i++) {

                    u = i / (numOfTests - 1f);
                    linearRandomSelector.SelectRandomItemBinarySearch(u);

                    r = (float) random.NextDouble();
                    linearRandomSelector.SelectRandomItemBinarySearch(r);
                }

                stopwatchBinary.Stop();
                
                log = stopwatchBinary.ElapsedMilliseconds;

                optimalBreakpoint++;
            }


            return optimalBreakpoint;
        }

    }
}