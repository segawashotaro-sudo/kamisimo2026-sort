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

        Debug.Log("\n========== [5. RoundResolver テスト] ==========");

        // --- ケース1: 同点 (同じソート・同じ配列でステップ数が一致する場合) ---
        Debug.Log("--- ケース1: 同点判定 ---");
        PlayerState r1_p1 = new PlayerState("P1", isCpu: false, initialPoints: 100);
        PlayerState r1_p2 = new PlayerState("P2", isCpu: true, initialPoints: 100);

        r1_p1.AddCard(SortType.Bubble);
        r1_p2.AddCard(SortType.Bubble);

        int[] seq1 = new int[] { 3, 1, 2 };

        List<PlayerChoice> choices1 = new List<PlayerChoice>
        {
            new PlayerChoice(r1_p1, SortType.Bubble, 10),
            new PlayerChoice(r1_p2, SortType.Bubble, 10)
        };

        Debug.Log($"[開始前] P1: {r1_p1.Points}pt, P2: {r1_p2.Points}pt");
        RoundResult result1 = RoundResolver.Resolve(seq1, choices1);

        Debug.Log($"[結果] ポット: {result1.TotalPot}pt, 勝者数: {result1.Winners.Count}人");
        Debug.Log($"[終了後] P1: {r1_p1.Points}pt (変動: {result1.PointChanges[r1_p1]}pt), IsOut: {r1_p1.IsOut}");
        Debug.Log($"[終了後] P2: {r1_p2.Points}pt (変動: {result1.PointChanges[r1_p2]}pt), IsOut: {r1_p2.IsOut}");

        // --- ケース2: 全員ボゴソートで諦め (gaveUp = true) ---
        Debug.Log("\n--- ケース2: 全員 gaveUp (返金) ---");
        PlayerState r2_p1 = new PlayerState("P1", isCpu: false, initialPoints: 100);
        PlayerState r2_p2 = new PlayerState("P2", isCpu: true, initialPoints: 100);

        r2_p1.AddCard(SortType.Bogo);
        r2_p2.AddCard(SortType.Bogo);

        // 要素数を多くして（逆順）、確実に BogoMaxSteps に到達させて gaveUp させる
        int[] seq2 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        List<PlayerChoice> choices2 = new List<PlayerChoice>
        {
            new PlayerChoice(r2_p1, SortType.Bogo, 20),
            new PlayerChoice(r2_p2, SortType.Bogo, 30)
        };

        Debug.Log($"[開始前] P1: {r2_p1.Points}pt, P2: {r2_p2.Points}pt");
        RoundResult result2 = RoundResolver.Resolve(seq2, choices2);

        Debug.Log($"[結果] ポット: {result2.TotalPot}pt, 勝者数: {result2.Winners.Count}人 (0＝引き分け/返金)");
        Debug.Log($"[終了後] P1: {r2_p1.Points}pt (変動: {result2.PointChanges[r2_p1]}pt), IsOut: {r2_p1.IsOut}");
        Debug.Log($"[終了後] P2: {r2_p2.Points}pt (変動: {result2.PointChanges[r2_p2]}pt), IsOut: {r2_p2.IsOut}");

        // --- ケース3: 片方がオールイン & 敗北して IsOut ---
        Debug.Log("\n--- ケース3: 片方オールイン & 敗北 (IsOut 判定) ---");
        PlayerState r3_p1 = new PlayerState("P1 (オールイン)", isCpu: false, initialPoints: 3);
        PlayerState r3_p2 = new PlayerState("P2", isCpu: true, initialPoints: 100);

        r3_p1.AddCard(SortType.Bubble);    // 速度が遅いカード
        r3_p2.AddCard(SortType.Selection); // 比較的早いカード

        int[] seq3 = new int[] { 5, 4, 3, 2, 1 };

        List<PlayerChoice> choices3 = new List<PlayerChoice>
        {
            new PlayerChoice(r3_p1, SortType.Bubble, 3),   // 残り全額 3pt オールイン
            new PlayerChoice(r3_p2, SortType.Selection, 10)
        };

        Debug.Log($"[開始前] P1: {r3_p1.Points}pt, P2: {r3_p2.Points}pt");
        RoundResult result3 = RoundResolver.Resolve(seq3, choices3);

        string winnerName = result3.Winners.Count > 0 ? result3.Winners[0].Name : "なし";
        Debug.Log($"[結果] ポット: {result3.TotalPot}pt, 勝者: {winnerName}");
        Debug.Log($"[終了後] P1: {r3_p1.Points}pt (変動: {result3.PointChanges[r3_p1]}pt), IsOut: {r3_p1.IsOut}");
        Debug.Log($"[終了後] P2: {r3_p2.Points}pt (変動: {result3.PointChanges[r3_p2]}pt), IsOut: {r3_p2.IsOut}");
    }
}