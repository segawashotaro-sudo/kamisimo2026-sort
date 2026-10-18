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
                RunMergeSort(workArray, steps);
                break;

            case SortType.Heap:
                RunHeapSort(workArray, steps);
                break;

            case SortType.Shell:
                RunShellSort(workArray, steps);
                break;

            case SortType.Counting:
                RunCountingSort(workArray, steps);
                break;

            case SortType.Tim:
                RunTimSort(workArray, steps);
                break;

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

    /// <summary>
    /// マージソートの実装
    /// </summary>
    private static void RunMergeSort(int[] arr, List<SortStep> steps)
    {
        // 内部関数：比較処理とステップ記録 (arr[i] > arr[j] なら true)
        bool Compare(int i, int j)
        {
            steps.Add(new SortStep(StepKind.Compare, i, j));
            return arr[i] > arr[j];
        }

        // 内部関数：値書き込み処理とステップ記録
        void Set(int index, int value)
        {
            steps.Add(new SortStep(StepKind.Set, index, value));
            arr[index] = value;
        }

        // 2つのソート済み部分配列 [left, mid] と [mid + 1, right] をマージする関数
        void Merge(int left, int mid, int right)
        {
            int[] temp = new int[right - left + 1];
            int i = left;
            int j = mid + 1;
            int k = 0;

            while (i <= mid && j <= right)
            {
                // arr[i] > arr[j] であれば右側の要素を先に追加、そうでなければ左側を先に追加
                if (Compare(i, j))
                {
                    temp[k++] = arr[j++];
                }
                else
                {
                    temp[k++] = arr[i++];
                }
            }

            // 左側の残りの要素をコピー
            while (i <= mid)
            {
                temp[k++] = arr[i++];
            }

            // 右側の残りの要素をコピー
            while (j <= right)
            {
                temp[k++] = arr[j++];
            }

            // 一時配列から元の配列へ Set 関数経由で書き戻す
            for (int idx = 0; idx < temp.Length; idx++)
            {
                Set(left + idx, temp[idx]);
            }
        }

        // 再帰処理関数
        void MergeSortRecursive(int left, int right)
        {
            if (left < right)
            {
                int mid = left + (right - left) / 2;

                MergeSortRecursive(left, mid);
                MergeSortRecursive(mid + 1, right);

                Merge(left, mid, right);
            }
        }

        // ソート実行（配列全体）
        if (arr.Length > 1)
        {
            MergeSortRecursive(0, arr.Length - 1);
        }
    }

    /// <summary>
    /// ヒープソートの実装
    /// </summary>
    private static void RunHeapSort(int[] arr, List<SortStep> steps)
    {
        int n = arr.Length;

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

        // 指定のノードを根とする部分木を最大ヒープに再構築する関数
        void Heapify(int heapSize, int root)
        {
            int largest = root;
            int left = 2 * root + 1;
            int right = 2 * root + 2;

            // 左の子が根より大きい場合 (arr[left] > arr[largest])
            if (left < heapSize && Compare(left, largest))
            {
                largest = left;
            }

            // 右の子が現在の最大値より大きい場合 (arr[right] > arr[largest])
            if (right < heapSize && Compare(right, largest))
            {
                largest = right;
            }

            // 最大値が根でない場合、交換して再帰的にヒープ化
            if (largest != root)
            {
                Swap(root, largest);
                Heapify(heapSize, largest);
            }
        }

        // 1. 最大ヒープの構築（末尾の親ノードから順に適用）
        for (int i = n / 2 - 1; i >= 0; i--)
        {
            Heapify(n, i);
        }

        // 2. ヒープから1つずつ要素を取り出して末尾へ移動
        for (int i = n - 1; i > 0; i--)
        {
            // 現在の根（最大値）を未確定領域の末尾へ移動
            Swap(0, i);

            // 残りのヒープ領域を再構築
            Heapify(i, 0);
        }
    }

    /// <summary>
    /// シェルソートの実装
    /// </summary>
    private static void RunShellSort(int[] arr, List<SortStep> steps)
    {
        int n = arr.Length;

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

        // 間隔 gap を n/2, n/4, ..., 1 と小さくしていく
        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            // 間隔 gap で挿入ソートを行う
            for (int i = gap; i < n; i++)
            {
                int j = i;

                // 距離 gap 離れた要素と比較し、左の方が大きければ交換して戻っていく
                while (j >= gap && Compare(j - gap, j))
                {
                    Swap(j - gap, j);
                    j -= gap;
                }
            }
        }
    }

    /// <summary>
    /// カウントソートの実装
    /// </summary>
    private static void RunCountingSort(int[] arr, List<SortStep> steps)
    {
        if (arr.Length == 0)
        {
            return;
        }

        // 内部関数：値書き込み処理とステップ記録
        void Set(int index, int value)
        {
            steps.Add(new SortStep(StepKind.Set, index, value));
            arr[index] = value;
        }

        // 1. 最小値と最大値を探索
        int min = arr[0];
        int max = arr[0];
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] < min)
            {
                min = arr[i];
            }
            if (arr[i] > max)
            {
                max = arr[i];
            }
        }

        // 2. 度数分布表（カウント配列）の作成
        int range = max - min + 1;
        int[] count = new int[range];
        for (int i = 0; i < arr.Length; i++)
        {
            count[arr[i] - min]++;
        }

        // 3. カウントを元に配列に書き戻す (Set を使用)
        int currentIndex = 0;
        for (int i = 0; i < range; i++)
        {
            while (count[i] > 0)
            {
                Set(currentIndex, i + min);
                currentIndex++;
                count[i]--;
            }
        }
    }

    /// <summary>
    /// Timソート（簡易版）の実装
    /// </summary>
    private static void RunTimSort(int[] arr, List<SortStep> steps)
    {
        int n = arr.Length;
        const int RUN = 4; // 判定ブロックサイズ

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

        // 内部関数：値書き込み処理とステップ記録
        void Set(int index, int value)
        {
            steps.Add(new SortStep(StepKind.Set, index, value));
            arr[index] = value;
        }

        // 部分配列を挿入ソートする関数
        void InsertionSortRange(int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                int j = i;
                while (j > left && Compare(j - 1, j))
                {
                    Swap(j - 1, j);
                    j--;
                }
            }
        }

        // 2つのソート済み部分配列 [left, mid] と [mid + 1, right] をマージする関数
        void Merge(int left, int mid, int right)
        {
            int[] temp = new int[right - left + 1];
            int i = left;
            int j = mid + 1;
            int k = 0;

            while (i <= mid && j <= right)
            {
                if (Compare(i, j))
                {
                    temp[k++] = arr[j++];
                }
                else
                {
                    temp[k++] = arr[i++];
                }
            }

            while (i <= mid)
            {
                temp[k++] = arr[i++];
            }

            while (j <= right)
            {
                temp[k++] = arr[j++];
            }

            for (int idx = 0; idx < temp.Length; idx++)
            {
                Set(left + idx, temp[idx]);
            }
        }

        // 1. 長さ RUN（4）ごとのブロックに分けて挿入ソートを適用
        for (int i = 0; i < n; i += RUN)
        {
            InsertionSortRange(i, Math.Min(i + RUN - 1, n - 1));
        }

        // 2. ブロックサイズを倍々にしながらマージしていく
        for (int size = RUN; size < n; size = 2 * size)
        {
            for (int left = 0; left < n; left += 2 * size)
            {
                int mid = left + size - 1;
                int right = Math.Min(left + 2 * size - 1, n - 1);

                if (mid < right)
                {
                    Merge(left, mid, right);
                }
            }
        }
    }
}