using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace DataStructures.RandomSelector.Math {

    public static class RandomMath {

        /// <summary>
        /// Breaking point between using Linear vs. Binary search for arrays (StaticSelector). 
        /// Was calculated empirically.
        /// </summary>
        public static readonly int ArrayBreakpoint = 51;
        
        /// <summary>
        /// Breaking point between using Linear vs. Binary search for lists (DynamicSelector). 
        /// Was calculated empirically. 
        /// </summary>
        public static readonly int ListBreakpoint = 26;
        
        /// <summary>
        /// Builds cummulative distribution out of non-normalized weights inplace.
        /// </summary>
        /// <param name="CDL">List of Non-normalized weights</param>
        public static void BuildCumulativeDistribution(List<float> CDL) {

            int Length = CDL.Count;

            // Use double for more precise calculation
            double Sum = 0;

            // Sum of weights
            for (int i = 0; i < Length; i++)
                Sum += CDL[i];

            // k is normalization constant
            // calculate inverse of sum and convert to float
            // use multiplying, since it is faster than dividing      
            double k = (1f / Sum);

            Sum = 0;

            // Make Cummulative Distribution Array
            for (int i = 0; i < Length; i++) {

                Sum += CDL[i] * k; //k, the normalization constant is applied here
                CDL[i] = (float) Sum; 
            }

            CDL[Length - 1] = 1f; //last item of CDA is always 1, I do this because numerical inaccurarcies add up and last item probably wont be 1

        }

        /// <summary>
        /// Builds cummulative distribution out of non-normalized weights inplace.
        /// </summary>
        /// <param name="CDA">Array of Non-normalized weights</param>
        public static void BuildCumulativeDistribution(float[] CDA) {

            int Length = CDA.Length;

            // Use double for more precise calculation
            double Sum = 0;
            
            // Sum of weights
            for (int i = 0; i < Length; i++)
                Sum += CDA[i];

            // k is normalization constant
            // calculate inverse of sum and convert to float
            // use multiplying, since it is faster than dividing   
            double k = (1f / Sum);

            Sum = 0;

            // Make Cummulative Distribution Array
            for (int i = 0; i < Length; i++) {

                Sum += CDA[i] * k; //k, the normalization constant is applied here
                CDA[i] = (float) Sum; 
            }

            CDA[Length - 1] = 1f; //last item of CDA is always 1, I do this because numerical inaccurarcies add up and last item probably wont be 1
        }

        
        /// <summary>
        /// Linear search, good/faster for small arrays
        /// </summary>
        /// <param name="CDL">Cummulative Distribution Array</param>
        /// <param name="randomValue">Uniform random value</param>
        /// <returns>Returns index of an value inside CDA</returns>
        public static int SelectIndexLinearSearch(this float[] CDA, float randomValue) {

            int i = 0;

            // last element, CDA[CDA.Length-1] should always be 1
            while (CDA[i] < randomValue)
                i++;

            return i;
        }


        /// <summary>
        /// Binary search, good/faster for big array
        /// Code taken out of C# array.cs Binary Search & modified
        /// </summary>
        /// <param name="CDA">Cummulative Distribution Array</param>
        /// <param name="randomValue">Uniform random value</param>
        /// <returns>Returns index of an value inside CDA</returns>
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
                    // shrink left
                    lo = index + 1;
                }
                else {
                    // shrink right
                    hi = index - 1;
                }
            }

            index = lo;

            return index;
        }
        
        /// <summary>
        /// Linear search, good/faster for small lists
        /// </summary>
        /// <param name="CDL">Cummulative Distribution List</param>
        /// <param name="randomValue">Uniform random value</param>
        /// <returns>Returns index of an value inside CDA</returns>
        public static int SelectIndexLinearSearch(this List<float> CDL, float randomValue) {

            int i = 0;
            
            // last element, CDL[CDL.Length-1] should always be 1
            while (CDL[i] < randomValue)
                i++;

            return i;
        }

        /// <summary>
        /// Binary search, good/faster for big lists
        /// Code taken out of C# array.cs Binary Search & modified
        /// </summary>
        /// <param name="CDL">Cummulative Distribution List</param>
        /// <param name="randomValue">Uniform random value</param>
        /// <returns>Returns index of an value inside CDL</returns>
        public static int SelectIndexBinarySearch(this List<float> CDL, float randomValue) {
        
            int lo = 0;
            int hi = CDL.Count - 1;
            int index;

            while (lo <= hi) {

                // calculate median
                index = lo + ((hi - lo) >> 1);

                if (CDL[index] == randomValue) {
                    return index;
                }
                if (CDL[index] < randomValue) {
                    // shrink left
                    lo = index + 1;
                }
                else {
                    // shrink right
                    hi = index - 1;
                }
            }

            index = lo;

            return index;
        }
        
        /// <summary>
        /// Returns identity, array[i] = i
        /// </summary>
        /// <param name="length">Length of an array</param>
        /// <returns>Identity array</returns>
        public static float[] IdentityArray(int length) {

            float[] array = new float[length];

            for (int i = 0; i < array.Length; i++)
                array[i] = i;
            
            return array;
        }

        /// <summary>
        /// Gemerates uniform random values for all indexes in array.
        /// </summary>
        /// <param name="list">The array where all values will be randomized.</param>
        /// <param name="r">Random generator</param>
        public static void RandomWeightsArray(ref float[] array, System.Random r) {
            
            for (int i = 0; i < array.Length; i++) {
                array[i] = (float) r.NextDouble();

                if (array[i] == 0)
                    i--;
            }
        }

        /// <summary>
        /// Creates new array with uniform random variables. 
        /// </summary>
        /// <param name="r">Random generator</param>
        /// <param name="length">Length of new array</param>
        /// <returns>Array with random uniform random variables</returns>
        public static float[] RandomWeightsArray(System.Random r, int length) {
        
            float[] array = new float[length];

            for (int i = 0; i < length; i++) {
                array[i] = (float) r.NextDouble();

                if (array[i] == 0)
                    i--;
            }
            return array;
        }


        /// <summary>
        /// Returns identity, list[i] = i
        /// </summary>
        /// <param name="length">Length of an list</param>
        /// <returns>Identity list</returns>
        public static List<float> IdentityList(int length) {

            List<float> list = new List<float>(length);

            for (int i = 0; i < length; i++)
                list.Add(i);

            return list;
        }

        /// <summary>
        /// Gemerates uniform random values for all indexes in list.
        /// </summary>
        /// <param name="list">The list where all values will be randomized.</param>
        /// <param name="r">Random generator</param>
        public static void RandomWeightsList(ref List<float> list, System.Random r) {

            for (int i = 0; i < list.Count; i++) {
                list[i] = (float) r.NextDouble();

                if (list[i] == 0)
                    i--;
            }
        }
    }
}