using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBHKUtils {
    public class IndexList<T> {
        private List<T> list = new List<T>();

        // Which index should be used to insert next entry
        private List<int> availableIndexes = new List<int>() { 0 };
        private List<int> usedIndexes = new List<int>() { };

        // Return used indexes in list
        // Some of the values here would not be used
        public List<T> Value { get => GetSome(); }

        // Values may be null so / available index so this is neccessary
        // The +1 is used because the availableIndexes list should always have 1 element
        // What does => do anyway?
        public int Count => usedIndexes.Count;

        public int Add(T _element) {
            int index = availableIndexes[0]; // Get index to insert at
            list.Insert(index, _element); // Insert _element at index

            availableIndexes.RemoveAt(0); // Remove available index
            usedIndexes.Add(index); // Add used index

            // If there are no available indexses left, add a new one at the end of the list
            if (availableIndexes.Count <= 0) {
                availableIndexes.Add(list.Count);
            }

            return index;
        }

        public void Remove(int _index) {
            availableIndexes.Add(_index);
            usedIndexes.Remove(_index);
        }

        public T GetElement(int _index) {
            return list[_index];
        }

        private List<T> GetSome() {
            List<T> output = new List<T>();

            for (int i = 0; i < usedIndexes.Count; i++) {
                output.Add(list[usedIndexes[i]]);
            }

            return output;
        }
    }
}
