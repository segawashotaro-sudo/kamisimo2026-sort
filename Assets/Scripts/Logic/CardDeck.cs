using System;
using System.Collections.Generic;
using UnityEngine;

public static class CardDeck
{
    // 各 SortType の出現重みを保持する辞書
    private static readonly Dictionary<SortType, int> cardWeights;

    // 静的コンストラクタ（クラス初回参照時に重みを初期化）
    static CardDeck()
    {
        cardWeights = new Dictionary<SortType, int>();

        // 全種類の初期重みを 10 に設定
        foreach (SortType type in Enum.GetValues(typeof(SortType)))
        {
            cardWeights[type] = 10;
        }

        // Bogo のみ出にくくするため重みを 3 に変更
        cardWeights[SortType.Bogo] = 3;
    }

    /// <summary>
    /// 重みに基づいてランダムに SortType を1枚引きます。
    /// </summary>
    public static SortType Draw()
    {
        // 1. 重みの合計値を計算
        int totalWeight = 0;
        foreach (var weight in cardWeights.Values)
        {
            totalWeight += weight;
        }

        if (totalWeight <= 0)
        {
            throw new InvalidOperationException("すべてのカードの重みが0以下のため、ドローできません。");
        }

        // 2. 0 ～ totalWeight - 1 の範囲でランダムな値を決定
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        // 3. 重みに応じたカードを選択
        int currentSum = 0;
        foreach (var pair in cardWeights)
        {
            currentSum += pair.Value;
            if (randomValue < currentSum)
            {
                return pair.Key;
            }
        }

        // 基本的にここには到達しませんが、フォールバックとして最初の要素を返します
        return SortType.Bubble;
    }

    /// <summary>
    /// 特定のカードの重みを変更したい場合に呼び出せます（任意）
    /// </summary>
    public static void SetWeight(SortType type, int weight)
    {
        if (cardWeights.ContainsKey(type))
        {
            cardWeights[type] = Mathf.Max(0, weight);
        }
    }

    /// <summary>
    /// 現在の重み設定を取得します（任意）
    /// </summary>
    public static int GetWeight(SortType type)
    {
        return cardWeights.TryGetValue(type, out int weight) ? weight : 0;
    }
}