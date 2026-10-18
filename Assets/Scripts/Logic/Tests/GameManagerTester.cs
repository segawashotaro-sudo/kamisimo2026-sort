using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerTester : MonoBehaviour
{
    public GameManager gameManager;

    private List<PlayerState> testPlayers;

    private void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("[GameManagerTester] GameManager がアサインされていません。");
            return;
        }

        // 1. 各種イベントの購読登録
        gameManager.OnRoundStarted += HandleRoundStarted;
        gameManager.OnChoicePhaseStarted += HandleChoicePhaseStarted;
        gameManager.OnRoundResolved += HandleRoundResolved;
        gameManager.OnGameOver += HandleGameOver;

        // 2. テスト用プレイヤーの作成（プレイヤー1名、CPU1名）
        testPlayers = new List<PlayerState>
        {
            new PlayerState("プレイヤー1", isCpu: false),
            new PlayerState("CPU1", isCpu: true)
        };

        // 3. ゲーム開始
        Debug.Log("========== [GameManagerTester] ゲームを開始します ==========");
        gameManager.StartGame(testPlayers, GameConfig.DefaultRounds);
    }

    private void OnDestroy()
    {
        // イベント解除（メモリリーク防止）
        if (gameManager != null)
        {
            gameManager.OnRoundStarted -= HandleRoundStarted;
            gameManager.OnChoicePhaseStarted -= HandleChoicePhaseStarted;
            gameManager.OnRoundResolved -= HandleRoundResolved;
            gameManager.OnGameOver -= HandleGameOver;
        }
    }

    // --- イベントハンドラー ---

    private void HandleRoundStarted(int round, int[] sequence)
    {
        Debug.Log($"\n--- 第 {round} / {gameManager.TotalRounds} ラウンド開始 ---");
        Debug.Log($"生成された数列: [{string.Join(", ", sequence)}]");
    }

    private void HandleChoicePhaseStarted()
    {

        // 脱落していないプレイヤー全員について処理
        foreach (var player in gameManager.ActivePlayers)
        {
            if (player.IsOut) continue;

            // 手札からランダムに1枚選択
            int randomIndex = Random.Range(0, player.Hand.Count);
            SortType chosenCard = player.Hand[randomIndex];

            // 最低ベット額でベット（持ち点が MinBet 未満の場合は CanBet で通る全額が選択される）
            int betAmount = player.Points < GameConfig.MinBet ? player.Points : GameConfig.MinBet;

            Debug.Log($"[選択] {player.Name} -> カード: {chosenCard}, ベット: {betAmount}pt (残り手札数: {player.Hand.Count - 1})");
            
            gameManager.SubmitChoice(player, chosenCard, betAmount);
        }
    }

    private void HandleRoundResolved(RoundResult result)
    {
        Debug.Log($"\n[ラウンド {gameManager.CurrentRound} 解決結果]");
        Debug.Log($"総ポット額: {result.TotalPot}pt");

        // 各自のカードとステップ数を表示
        foreach (var pair in result.SortResults)
        {
            var player = pair.Key;
            var sortResult = pair.Value;
            string status = sortResult.gaveUp ? "ギブアップ(gaveUp)" : $"{sortResult.StepCount} ステップ";
            
            Debug.Log($" - {player.Name}: [{sortResult.type}] -> {status}");
        }

        // 勝者の表示
        if (result.Winners.Count > 0)
        {
            string winnerNames = string.Join(", ", result.Winners.Select(w => w.Name));
            Debug.Log($" 勝者: {winnerNames}");
        }
        else
        {
            Debug.Log(" 勝者: なし（全員ギブアップ等のため引き分け）");
        }

        // 各プレイヤーのポイントと状態を表示
        Debug.Log("【現在のスコア】");
        foreach (var player in gameManager.ActivePlayers)
        {
            int change = result.PointChanges.ContainsKey(player) ? result.PointChanges[player] : 0;
            string outStatus = player.IsOut ? " [脱落]" : "";
            Debug.Log($" - {player.Name}: {player.Points}pt (変動: {(change >= 0 ? "+" : "")}{change}pt){outStatus}");
        }

        // 1秒待ってから次ラウンドを呼び出すコルーチンを開始
        StartCoroutine(WaitAndNextRoundRoutine());
    }

    private IEnumerator WaitAndNextRoundRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        gameManager.NextRound();
    }

    private void HandleGameOver(PlayerState winner)
    {
        Debug.Log("\n========== [ゲームセット] ==========");
        if (winner != null)
        {
            Debug.Log($"🏆 最終勝者: {winner.Name} (最終ポイント: {winner.Points}pt)");
        }
        else
        {
            Debug.Log("🏆 最終勝者: なし（全員脱落）");
        }
        Debug.Log("====================================");
    }
}