using System;
using System.Collections.Generic;
using UnityEngine;

public class PartsTester : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("========== [1. SequenceGenerator テスト] ==========");
        foreach (SequencePattern pattern in Enum.GetValues(typeof(SequencePattern)))
        {
            int[] seq = SequenceGenerator.Generate(pattern);
            Debug.Log($"[{pattern}] 長さ={seq.Length}, 数列=[{string.Join(",", seq)}]");
        }

        Debug.Log("\n========== [2. CardDeck.Draw() 100回テスト] ==========");
        Dictionary<SortType, int> drawCounts = new Dictionary<SortType, int>();
        foreach (SortType type in Enum.GetValues(typeof(SortType)))
        {
            drawCounts[type] = 0;
        }

        for (int i = 0; i < 100; i++)
        {
            SortType drawnCard = CardDeck.Draw();
            drawCounts[drawnCard]++;
        }

        foreach (var pair in drawCounts)
        {
            Debug.Log($"{pair.Key}: {pair.Value} 回");
        }

        Debug.Log("\n========== [3. PlayerState & AddCard/HandLimit テスト] ==========");
        PlayerState p1 = new PlayerState("プレイヤー1", isCpu: false);
        PlayerState p2 = new PlayerState("CPU1", isCpu: true);

        // 3枚ずつ配る
        for (int i = 0; i < GameConfig.HandLimit; i++)
        {
            p1.AddCard(CardDeck.Draw());
            p2.AddCard(CardDeck.Draw());
        }

        Debug.Log($"{p1.Name} の手札: [{string.Join(", ", p1.Hand)}]");
        Debug.Log($"{p2.Name} の手札: [{string.Join(", ", p2.Hand)}]");

        // 4枚目を追加して false になることを確認
        bool p1OverAdd = p1.AddCard(CardDeck.Draw());
        bool p2OverAdd = p2.AddCard(CardDeck.Draw());
        Debug.Log($"手札上限(3枚)時の追加結果 -> {p1.Name}: {p1OverAdd} (期待値: False), {p2.Name}: {p2OverAdd} (期待値: False)");

        Debug.Log("\n========== [4. CanBet() テスト] ==========");
        Debug.Log($"初期持ち点: {p1.Points}pt (MinBet={GameConfig.MinBet})");
        Debug.Log($"CanBet(3)   : {p1.CanBet(3)}   (期待値: False / MinBet未満のため)");
        Debug.Log($"CanBet(50)  : {p1.CanBet(50)}  (期待値: True / 条件を満たすため)");
        Debug.Log($"CanBet(200) : {p1.CanBet(200)} (期待値: False / 所持ポイント超過のため)");

        // 持ち点が MinBet 未満になった場合のテスト（オールイン確認）
        p1.Points = 3;
        Debug.Log($"持ち点変更後: {p1.Points}pt (MinBet未満の状態)");
        Debug.Log($"CanBet(3) [全額オールイン] : {p1.CanBet(3)} (期待値: True)");
        Debug.Log($"CanBet(2) [中途半端な額]   : {p1.CanBet(2)} (期待値: False)");
    }
}