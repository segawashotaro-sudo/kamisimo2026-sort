using System;
using System.Collections.Generic;
using UnityEngine;

// 数列のパターンの種類
public enum SequencePattern
{
    Random,        // 完全にバラバラ
    NearlySorted,  // ほぼ昇順（1〜2箇所のみ入れ替え）
    Reversed,      // 降順
    FewUnique      // 2〜3種類の値のみ
}

public static class SequenceGenerator
{
    /// <summary>
    /// ランダムなパターンの数列を生成します。
    /// </summary>
    public static int[] Generate()
    {
        // 4種類のパターンからランダムに選択
        Array patterns = Enum.GetValues(typeof(SequencePattern));
        SequencePattern randomPattern = (SequencePattern)patterns.GetValue(UnityEngine.Random.Range(0, patterns.Length));

        return Generate(randomPattern);
    }

    /// <summary>
    /// 指定されたパターンの数列を生成します。
    /// </summary>
    public static int[] Generate(SequencePattern pattern)
    {
        // ランダムな配列長を取得
        int length = UnityEngine.Random.Range(GameConfig.SequenceMinLength, GameConfig.SequenceMaxLength + 1);

        switch (pattern)
        {
            case SequencePattern.Random:
                return GenerateRandom(length);

            case SequencePattern.NearlySorted:
                return GenerateNearlySorted(length);

            case SequencePattern.Reversed:
                return GenerateReversed(length);

            case SequencePattern.FewUnique:
                return GenerateFewUnique(length);

            default:
                throw new ArgumentOutOfRangeException(nameof(pattern), pattern, null);
        }
    }

    // --- 各パターンの生成処理 ---

    // 1. Random：完全にバラバラ
    private static int[] GenerateRandom(int length)
    {
        int[] arr = new int[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = UnityEngine.Random.Range(GameConfig.ValueMin, GameConfig.ValueMax + 1);
        }
        return arr;
    }

    // 2. NearlySorted：昇順に並べたあと、隣同士を1〜2か所だけ入れ替える
    private static int[] GenerateNearlySorted(int length)
    {
        int[] arr = GenerateRandom(length);
        Array.Sort(arr); // 昇順にソート

        // 要素数が2未満の場合は入れ替え不可のためそのまま返す
        if (length < 2)
        {
            return arr;
        }

        // 交換回数を1〜2回で決定
        int swapCount = UnityEngine.Random.Range(1, 3);

        for (int i = 0; i < swapCount; i++)
        {
            // 隣り合うインデックス（0 ～ length - 2）をランダムに選ぶ
            int idx = UnityEngine.Random.Range(0, length - 1);
            
            // 隣同士（idx と idx + 1）を交換
            int temp = arr[idx];
            arr[idx] = arr[idx + 1];
            arr[idx + 1] = temp;
        }

        return arr;
    }

    // 3. Reversed：降順
    private static int[] GenerateReversed(int length)
    {
        int[] arr = GenerateRandom(length);
        Array.Sort(arr);
        Array.Reverse(arr); // 降順に反転
        return arr;
    }

    // 4. FewUnique：2〜3種類の値だけで作る
    private static int[] GenerateFewUnique(int length)
    {
        // ユニーク値の種類数（2 または 3）
        int uniqueCount = UnityEngine.Random.Range(2, 4);
        List<int> availableValues = new List<int>();

        // 重複しないランダムな値を uniqueCount 個選択
        while (availableValues.Count < uniqueCount)
        {
            int val = UnityEngine.Random.Range(GameConfig.ValueMin, GameConfig.ValueMax + 1);
            if (!availableValues.Contains(val))
            {
                availableValues.Add(val);
            }
        }

        int[] arr = new int[length];
        for (int i = 0; i < length; i++)
        {
            // 選択した2〜3種類の値の中からランダムに割り当てる
            int randomIndex = UnityEngine.Random.Range(0, availableValues.Count);
            arr[i] = availableValues[randomIndex];
        }

        return arr;
    }
}