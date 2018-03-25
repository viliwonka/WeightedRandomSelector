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
namespace DataStructures.RandomSelector.Math {

    public static class RandomMath {

        public static readonly int ArrayBreakpoint = 16;
        
        public static readonly int ListBreakpoint = 16; //? to find out
        
        public static void BuildCumulativeDistribution(List<float> CDA) {

            int Length = CDA.Count;

            // Use double for more precise calculation
            double Sum = 0;

            // Sum of weights
            for (int i = 0; i < Length; i++)
                Sum += CDA[i];

            // k is normalization constant
            // calculate inverse of sum and convert to float
            // this is optimisation (multiplying is faster than division)      
            double k = (1f / Sum);

            Sum = 0;

            // Make Cummulative Distribution Array
            for (int i = 0; i < Length; i++) {

                Sum += CDA[i];
                CDA[i] = (float) (Sum * k); //k, the normalization constant is applied here
            }

            CDA[Length - 1] = 1f; //last item of CDA is always 1, I do this because numerical inaccurarcies add up and last item probably wont be 1

        }
        
        public static void BuildCumulativeDistribution(float[] CDA) {

            int Length = CDA.Length;

            // Use double for more precise calculation
            double Sum = 0;
            
            // Sum of weights
            for (int i = 0; i < Length; i++)
                Sum += CDA[i];

            // k is normalization constant
            // calculate inverse of sum and convert to float
            // this is optimisation (multiplying is faster than division)      
            double k = (1f / Sum);

            Sum = 0;

            // Make Cummulative Distribution Array
            for (int i = 0; i < Length; i++) {

                Sum += CDA[i];
                CDA[i] = (float) (Sum * k); //k, the normalization constant is applied here
            }

            CDA[Length - 1] = 1f; //last item of CDA is always 1, I do this because numerical inaccurarcies add up and last item probably wont be 1
        }
        
        public static int SelectIndexLinearSearch(this float[] CDA, float randomValue) {

            int i = 0;

            while (i < CDA.Length && CDA[i] < randomValue)
                i++;

            return i;
        }
        
        // Code taken out of C# Array Binary Search & modified
        public static int SelectIndexBinarySearch(this float[] CDA, float randomValue) {

            int lo = 0;
            int hi = CDA.Length - 1;
            int index;

            while (lo <= hi) {

                // calculate median
                index = lo + ((hi - lo) >> 1);

                if (CDA[index] == randomValue) {
                    return index;
                }
                if (CDA[index] < randomValue) {
                    lo = index + 1;
                }
                else {
                    hi = index - 1;
                }
            }

            index = lo;

            return index;
        }

        public static int SelectIndexLinearSearch(this List<float> CDA, float randomValue) {

            int i = 0;
            int Length = CDA.Count;
            
            while (i < Length && CDA[i] < randomValue)
                i++;

            return i;
        }
        
        // Code taken out of C# Array Binary Search & modified
        public static int SelectIndexBinarySearch(this List<float> CDA, float randomValue) {
        
            int lo = 0;
            int hi = CDA.Count - 1;
            int index;

            while (lo <= hi) {

                // calculate median
                index = lo + ((hi - lo) >> 1);

                if (CDA[index] == randomValue) {
                    return index;
                }
                if (CDA[index] < randomValue) {
                    lo = index + 1;
                }
                else {
                    hi = index - 1;
                }
            }

            index = lo;

            return index;
        }
        
        public static float[] IdentityArray(int length) {

            float[] array = new float[length];

            for (int i = 0; i < array.Length; i++)
                array[i] = i;
            
            return array;
        }

        public static float[] RandomWeightsArray(int length) {

            var r = new System.Random();

            float[] array = new float[length];

            for (int i = 0; i < array.Length; i++) {
                array[i] = (float) r.NextDouble();

                if (array[i] == 0)
                    i--;
            }
            return array;
        }
        
    }
}