using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiThreadedMergeSort
{
    class MultiThreadedMergeSort
    {
        static void Main(string[] args)
        {
            Int64[] Test1 = {156, 32, 34, 243, 67, 12, 10, 89, 19, 10, 9, 113, 143, 290,15,876};
            Int64[] Test2 = { 156, 32, 34, 243, 67, 12, 10, 89, 19, 10, 9, 113, 143, 290, 15 };
            Console.WriteLine("Arr Length : "+Test1.Length);
            MultiThreadedMergeSort mg = new MultiThreadedMergeSort();
            Test1 = mg.mergeSort(Test1, 4);
            Test2 = mg.mergeSort(Test2, 4);
            Console.WriteLine();
            for (int i = 0; i < Test1.Length; i++)
            {
                Console.Write(Test1[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < Test2.Length; i++)
            {
                Console.Write(Test2[i] + " ");
            }
        }
        //array - the array of 64-bit integers to sort

        //minElements - the number of elements to be sorted per threads without splitting the array to two threads.

        // return value: the sorted array

        //lets use 2 threads
        public Int64[] mergeSort(Int64[] array, int minElements) {
            if (array.Length == minElements)
            {
                //Do not Divide the array into Threads
                //call for mergeSortForThreads - without dividing
                mergeSortforThreads(array, 0, array.Length-1);
            }
            else
            {
                //Divide the array into 2 threads
                int mid = (array.Length + 1) / 2;
                Int64[] firstArray = new Int64[mid];
                Int64[] secondArray = new Int64[array.Length - mid];
                Array.Copy(array, 0, firstArray, 0, mid);
                Array.Copy(array, mid, secondArray, 0, secondArray.Length);
                Thread t1 = new Thread(() => mergeSortforThreads(firstArray, 0, firstArray.Length-1));
                Thread t2 = new Thread(() => mergeSortforThreads(secondArray, 0, secondArray.Length-1));
                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
                firstArray.CopyTo(array, 0);
                secondArray.CopyTo(array, firstArray.Length);
                array = Combine(array, 0, firstArray.Length, array.Length - 1);
                
            }
            return array;
        }
        static public void merge(Int64[] numbers, int left, int mid, int right)
        {
            Int64[] temp = new Int64[25];
            int i, eol, num, pos;
            eol = (mid - 1);
            pos = left;
            num = (right - left + 1);

            while ((left <= eol) && (mid <= right))
            {
                if (numbers[left] <= numbers[mid])
                    temp[pos++] = numbers[left++];
                else
                    temp[pos++] = numbers[mid++];
            }
            while (left <= eol)
                temp[pos++] = numbers[left++];
            while (mid <= right)
                temp[pos++] = numbers[mid++];
            for (i = 0; i < num; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }
        static public void mergeSortforThreads(Int64[] arr, int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;
                mergeSortforThreads(arr, left, middle);
                mergeSortforThreads(arr, middle + 1, right);
                merge(arr, left, middle+1, right);
            }
        }
        static public Int64[] Combine(Int64[] numbers, int left, int mid, int right)
        {
            int middle = mid;
            Int64 temp;
            Int64[] Final = new Int64[numbers.Length];
            int i = 0;
            while (left <= middle && mid <= right)
            {
                if (left!=middle && numbers[left] < numbers[mid])
                {
                    Final[i] = numbers[left];
                    i++;
                    left++;
                }
                else
                {
                    Final[i] = numbers[mid];
                    i++;
                    mid++;
                }
            }
            return Final;
        }

    }
}
