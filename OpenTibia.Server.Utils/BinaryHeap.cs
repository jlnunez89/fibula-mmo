using System;
using System.Collections.Generic;

namespace OpenTibia.Utilities
{
    public class BinaryHeap<T>
    {
        protected T[] Data;

        protected Comparison<T> Comparison;

        public BinaryHeap()
        {
            Constructor(4, null);
        }

        public BinaryHeap(Comparison<T> comparison)
        {
            Constructor(4, comparison);
        }

        public BinaryHeap(int capacity)
        {
            Constructor(capacity, null);
        }

        public BinaryHeap(int capacity, Comparison<T> comparison)
        {
            Constructor(capacity, comparison);
        }

        private void Constructor(int capacity, Comparison<T> comparison)
        {
            Data = new T[capacity];
            Comparison = comparison;
            if (Comparison == null)
                Comparison = Comparer<T>.Default.Compare;
        }

        public int Size { get; private set; }

        /// <summary>
        /// Add an item to the heap
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            if (Size == Data.Length)
                Resize();
            Data[Size] = item;
            HeapifyUp(Size);
            Size++;
        }

        /// <summary>
        /// Get the item of the root
        /// </summary>
        /// <returns></returns>
        public T Peak()
        {
            return Data[0];
        }

        /// <summary>
        /// Extract the item of the root
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            var item = Data[0];
            Size--;
            Data[0] = Data[Size];
            HeapifyDown(0);
            return item;
        }

        private void Resize()
        {
            var resizedData = new T[Data.Length * 2];
            Array.Copy(Data, 0, resizedData, 0, Data.Length);
            Data = resizedData;
        }

        private void HeapifyUp(int childIdx)
        {
            if (childIdx > 0)
            {
                var parentIdx = (childIdx - 1) / 2;
                if (Comparison.Invoke(Data[childIdx], Data[parentIdx]) > 0)
                {
                    // swap parent and child
                    var t = Data[parentIdx];
                    Data[parentIdx] = Data[childIdx];
                    Data[childIdx] = t;
                    HeapifyUp(parentIdx);
                }
            }
        }

        private void HeapifyDown(int parentIdx)
        {
            var leftChildIdx = 2 * parentIdx + 1;
            var rightChildIdx = leftChildIdx + 1;
            var largestChildIdx = parentIdx;
            if (leftChildIdx < Size && Comparison.Invoke(Data[leftChildIdx], Data[largestChildIdx]) > 0)
            {
                largestChildIdx = leftChildIdx;
            }
            if (rightChildIdx < Size && Comparison.Invoke(Data[rightChildIdx], Data[largestChildIdx]) > 0)
            {
                largestChildIdx = rightChildIdx;
            }
            if (largestChildIdx != parentIdx)
            {
                var t = Data[parentIdx];
                Data[parentIdx] = Data[largestChildIdx];
                Data[largestChildIdx] = t;
                HeapifyDown(largestChildIdx);
            }
        }
    }
}
