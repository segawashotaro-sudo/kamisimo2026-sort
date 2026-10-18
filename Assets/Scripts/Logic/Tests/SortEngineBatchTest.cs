using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortEngineBatchTest : MonoBehaviour
{
    private void Start()
    {
        const int testCount = 100;
        List<int[]> testArrays = new List<int[]>();

        // 1. 長さ SequenceMinLength～SequenceMaxLength、値 ValueMin～ValueMax のランダム配列を100個生成
        for (int i = 0; i < testCount; i++)
        {
            int length = UnityEngine.Random.Range(GameConfig.SequenceMinLength, GameConfig.SequenceMaxLength + 1);
            int[] arr = new int[length];
            for (int j = 0; j < length; j++)
            {
                arr[j] = UnityEngine.Random.Range(GameConfig.ValueMin, GameConfig.ValueMax + 1);
            }
            testArrays.Add(arr);
        }

        // 2. SortType の全種類について検証
        SortType[] allTypes = (SortType[])Enum.GetValues(typeof(SortType));

        foreach (SortType type in allTypes)
        {
            List<int> stepCounts = new List<int>();
            int gaveUpCount = 0;

            foreach (int[] sample in testArrays)
            {
                SortResult result = SortEngine.Run(type, sample);
                int[] finalArray = Replay(result);

                // gaveUp が true でないのに昇順になっていなければ Error ログを出力
                if (!result.gaveUp && !IsSorted(finalArray))
                {
                    Debug.LogError($"[ソート失敗] 種類: {type}, 元の配列: [{string.Join(",", sample)}], ソート結果: [{string.Join(",", finalArray)}]");
                }

                if (result.gaveUp)
                {
                    gaveUpCount++;
                }

                stepCounts.Add(result.StepCount);
            }

            // 3. 種類ごとに平均・最小・最大ステップ数と gaveUp 回数を Debug.Log で出力
            double averageSteps = stepCounts.Average();
            int minSteps = stepCounts.Min();
            int maxSteps = stepCounts.Max();

            Debug.Log($"{type}: 平均ステップ数={averageSteps:F2}, 最小={minSteps}, 最大={maxSteps}, 諦めた回数={gaveUpCount}/{testCount}");
        }
    }

    /// <summary>
    /// 記録された手順を最初の配列に当てはめ直す
    /// </summary>
    private static int[] Replay(SortResult r)
    {
        int[] a = (int[])r.initialArray.Clone();
        foreach (var s in r.steps)
        {
            if (s.kind == StepKind.Swap)
            {
                (a[s.a], a[s.b]) = (a[s.b], a[s.a]);
            }
            else if (s.kind == StepKind.Set)
            {
                a[s.a] = s.b;
            }
            else if (s.kind == StepKind.Shuffle)
            {
                a = (int[])s.newOrder.Clone();
            }
        }
        return a;
    }

    /// <summary>
    /// 配列が昇順に並んでいるか検証する
    /// </summary>
    private static bool IsSorted(int[] a)
    {
        for (int i = 1; i < a.Length; i++)
        {
            if (a[i - 1] > a[i])
            {
                return false;
            }
        }
        return true;
    }
}