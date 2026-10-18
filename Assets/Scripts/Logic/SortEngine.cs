using System;
using System.Collections.Generic;

public static class SortEngine
{
    public static SortResult Run(SortType type, int[] array)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        // 元の配列のコピーを作成
        int[] initialArray = (int[])array.Clone();
        // ソート処理で使用する作業用配列
        int[] workArray = (int[])array.Clone();
        
        List<SortStep> steps = new List<SortStep>();
        bool gaveUp = false;

        switch (type)
        {
            case SortType.Bubble:
                RunBubbleSort(workArray, steps);
                break;

            case SortType.Selection:
                RunSelectionSort(workArray, steps);
                break;

            case SortType.Insertion:
                RunInsertionSort(workArray, steps);
                break;

            case SortType.Quick:
                RunQuickSort(workArray, steps);
                break;

            case SortType.Merge:
            case SortType.Heap:
            case SortType.Shell:
            case SortType.Counting:
            case SortType.Tim:
            case SortType.Bogo:
                throw new NotImplementedException($"{type} はまだ実装されていません。");

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return new SortResult
        {
            type = type,
            initialArray = initialArray,
            steps = steps,
            gaveUp = gaveUp
        };
    }

    /// <summary>
    /// バブルソートの実装
    /// </summary>
    private static void RunBubbleSort(int[] arr, List<SortStep> steps)
    {
        int n = arr.Length;

        // 内部関数：比較処理とステップ記録
        bool Compare(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Compare, i, j));
            return arr[i] > arr[j];
        }

        // 内部関数：交換処理とステップ記録
        void Swap(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Swap, i, j));
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        // バブルソートのメインループ
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - 1 - i; j++)
            {
                // 隣り合う要素を比較し、左の方が大きければ交換
                if (Compare(j, j + 1))
                {
                    Swap(j, j + 1);
                    swapped = true;
                }
            }

            // 1周で一度も交換が発生しなければソート完了
            if (!swapped)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 選択ソートの実装
    /// </summary>
    private static void RunSelectionSort(int[] arr, List<SortStep> steps)
    {
        int n = arr.Length;

        // 内部関数：比較処理とステップ記録
        bool Compare(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Compare, i, j));
            return arr[i] > arr[j];
        }

        // 内部関数：交換処理とステップ記録
        void Swap(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Swap, i, j));
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        // 選択ソートのメインループ
        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < n; j++)
            {
                // 現在の最小値候補(minIndex)よりも小さい値(j)が見つかったか比較
                if (Compare(minIndex, j))
                {
                    minIndex = j;
                }
            }

            // 最小値の位置が変わっていれば確定位置(i)と交換
            if (minIndex != i)
            {
                Swap(i, minIndex);
            }
        }
    }

    /// <summary>
    /// 挿入ソートの実装
    /// </summary>
    private static void RunInsertionSort(int[] arr, List<SortStep> steps)
    {
        int n = arr.Length;

        // 内部関数：比較処理とステップ記録
        bool Compare(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Compare, i, j));
            return arr[i] > arr[j];
        }

        // 内部関数：交換処理とステップ記録
        void Swap(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Swap, i, j));
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        // 挿入ソートのメインループ
        for (int i = 1; i < n; i++)
        {
            int j = i;

            // 左隣の要素と比較し、左の方が大きければ隣同士を交換しながら左へ移動
            while (j > 0 && Compare(j - 1, j))
            {
                Swap(j - 1, j);
                j--;
            }
        }
    }

    /// <summary>
    /// クイックソートの実装
    /// </summary>
    private static void RunQuickSort(int[] arr, List<SortStep> steps)
    {
        // 内部関数：比較処理とステップ記録 (arr[i] > arr[j] なら true)
        bool Compare(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Compare, i, j));
            return arr[i] > arr[j];
        }

        // 内部関数：交換処理とステップ記録
        void Swap(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Swap, i, j));
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        // パーティション分割関数（右端 high をピボットとする）
        int Partition(int low, int high)
        {
            int pivotIndex = high; // 右端の要素をピボットとする
            int i = low - 1;       // ピボット以下の要素を置く境界インデックス

            for (int j = low; j < high; j++)
            {
                // ピボット(pivotIndex) > arr[j] かどうかを判定 (Compare(pivotIndex, j) が true)
                if (Compare(pivotIndex, j))
                {
                    i++;
                    if (i != j)
                    {
                        Swap(i, j);
                    }
                }
            }

            // ピボットを適切な位置へ移動
            if (i + 1 != high)
            {
                Swap(i + 1, high);
            }

            return i + 1;
        }

        // 再帰処理関数
        void QuickSortRecursive(int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(low, high);

                // ピボットの左右の部分配列を再帰的にソート
                QuickSortRecursive(low, pi - 1);
                QuickSortRecursive(pi + 1, high);
            }
        }

        // ソート実行（配列全体）
        if (arr.Length > 1)
        {
            QuickSortRecursive(0, arr.Length - 1);
        }
    }
}