# Weighted Random Selector

### Description

This repository contains code for selecting items randomly based on weights.
It is very commonly used thing in video game development and it is suprisingly tricky to get right.

### It was designed to be:

* fast for smaller and bigger collections, 
* readable,
* easy to use, 
* numerically precise
* thread-safe selecting

#### Use cases

* Item dropping from resources, enemies
* Damage giving / taking
* Selecting random events to happen
* Procedural quest generation
* Picking random sound effect to play on event
* Casino games: Roulette, Slot machine (although I believe those casino games are weighted uniformly)
* Random state machines / Markov chain

#### How to use

You have two options:

##### DynamicRandomSelector
* Easier to use, since it contains both Construction and Random Picking parts
* Can be modified on the fly (added or removed items), since internal collection is List
* Around 2x slower, since internal collection is List

##### StaticRandomSelector
* Harder to use
* Separate component for Construction (StaticRandomSelectorBuilder) and RandomPicking (StaticRandomSelector)
* Faster, since internal collection is Array
* If you really need extra speed, remove all functions calls with hardcoding all functions

#### How it works?

##### Construction

0. You input items, and every item has it's own weight (un-normalized)
1. Array of weights is normalized, so that it becomes array of probabilities/masses (sum of 1)
2. This array of probabilities get converted to (discrete) Cumulative Distribution Array

##### Random picking

1. For each random pick, uniform random number is generated
2. Depending on size of collection, linear or binary search is used to select correct index
3. This index is used to return item from internal array/list. 

#### Minimum of linear search and binary search
![alt text](https://raw.githubusercontent.com/viliwonka/WeightedRandomSelector/master/Documentation/Complexity.png "Optimized search time")

