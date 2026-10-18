// 置き場所: Assets/Scripts/Logic/GameManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ゲームの進行状態
public enum GamePhase
{
    Setup,          // 初期化・開始前
    ChoosingCards,  // カード選択・ベット受け付け中
    Resolving,      // ソート実行・結果解決中
    WaitingNext,    // ラウンド結果表示中（次ラウンド待ち）
    GameOver        // ゲーム終了
}

public class GameManager : MonoBehaviour
{
    // --- イベント（UI担当のBさんがアタッチして使用） ---
    public event Action<int, int[]> OnRoundStarted;       // (現在のラウンド数, 生成された数列)
    public event Action OnChoicePhaseStarted;           // プレイヤー入力受付開始
    public event Action<RoundResult> OnRoundResolved;    // ラウンド結果決定
    public event Action<PlayerState> OnGameOver;         // ゲーム終了 (勝者)

    // --- プロパティ ---
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;
    public int CurrentRound { get; private set; } = 0;
    public int TotalRounds { get; private set; } = GameConfig.DefaultRounds;
    public int[] CurrentSequence { get; private set; }
    public IReadOnlyList<PlayerState> ActivePlayers => players.AsReadOnly();

    private List<PlayerState> players = new List<PlayerState>();
    private Dictionary<PlayerState, PlayerChoice> currentChoices = new Dictionary<PlayerState, PlayerChoice>();

    /// <summary>
    /// ゲームの初期化と開始
    /// </summary>
    public void StartGame(List<PlayerState> initialPlayers, int rounds = GameConfig.DefaultRounds)
    {
        if (initialPlayers == null || initialPlayers.Count == 0)
        {
            Debug.LogError("[GameManager] 参加プレイヤーが指定されていません。");
            return;
        }

        players = new List<PlayerState>(initialPlayers);
        TotalRounds = Mathf.Clamp(rounds, GameConfig.MinRounds, GameConfig.MaxRounds);
        CurrentRound = 0;

        // 全プレイヤーに初期手札を上限まで配る
        foreach (var player in players)
        {
            while (player.Hand.Count < GameConfig.HandLimit)
            {
                player.AddCard(CardDeck.Draw());
            }
        }

        StartNextRound();
    }

    /// <summary>
    /// 選択の送信（プレイヤーまたはCPUが呼び出す）
    /// </summary>
    public bool SubmitChoice(PlayerState player, SortType card, int bet)
    {
        if (CurrentPhase != GamePhase.ChoosingCards)
        {
            Debug.LogWarning("[GameManager] 現在はカード選択フェーズではありません。");
            return false;
        }

        if (player == null || player.IsOut || !players.Contains(player))
        {
            Debug.LogWarning("[GameManager] 無効なプレイヤーです。");
            return false;
        }

        if (currentChoices.ContainsKey(player))
        {
            Debug.LogWarning($"[GameManager] {player.Name} は既に選択済みです。");
            return false;
        }

        // 手札所持チェック
        if (!player.Hand.Contains(card))
        {
            Debug.LogWarning($"[GameManager] {player.Name} の手札に {card} が存在しません。");
            return false;
        }

        // ベット可能性チェック
        if (!player.CanBet(bet))
        {
            Debug.LogWarning($"[GameManager] {player.Name} は {bet}pt をベットできません。");
            return false;
        }

        // 選択を記録
        currentChoices[player] = new PlayerChoice(player, card, bet);

        // 生存している全プレイヤーの選択が揃ったか判定
        var survivingPlayers = players.Where(p => !p.IsOut).ToList();
        if (currentChoices.Count >= survivingPlayers.Count)
        {
            ResolveRound();
        }

        return true;
    }

    /// <summary>
    /// 次のラウンドに進む（UIの「次へ」ボタンなどから呼ぶ）
    /// </summary>
    public void NextRound()
    {
        if (CurrentPhase != GamePhase.WaitingNext)
        {
            Debug.LogWarning("[GameManager] ラウンド結果の待機中のみ呼び出せます。");
            return;
        }

        // 脱落していないプレイヤーが1人以下、または規定ラウンド終了でゲームオーバー
        var survivingPlayers = players.Where(p => !p.IsOut).ToList();
        if (survivingPlayers.Count <= 1 || CurrentRound >= TotalRounds)
        {
            EndGame();
            return;
        }

        // 手札を補充して次のラウンドを開始
        foreach (var player in survivingPlayers)
        {
            while (player.Hand.Count < GameConfig.HandLimit)
            {
                player.AddCard(CardDeck.Draw());
            }
        }

        StartNextRound();
    }

    // --- 内部進行処理 ---

    private void StartNextRound()
    {
        CurrentRound++;
        currentChoices.Clear();

        // 1. 数列の生成
        CurrentSequence = SequenceGenerator.Generate();

        CurrentPhase = GamePhase.ChoosingCards;

        // UIへ通知
        OnRoundStarted?.Invoke(CurrentRound, CurrentSequence);
        OnChoicePhaseStarted?.Invoke();

        // CPUプレイヤーの自動カード選択・ベット
        ProcessCpuChoices();
    }

    /// <summary>
    /// 生存しているCPUプレイヤーの選択を自動実行する
    /// </summary>
    private void ProcessCpuChoices()
    {
        // currentChoices への追加に伴うループ中の変更を避けるためToListで抽出
        var cpuPlayers = players.Where(p => !p.IsOut && p.IsCpu).ToList();

        foreach (var cpu in cpuPlayers)
        {
            // フェーズが切り替わっている可能性（プレイヤーが全員CPUなどで既にResolveされた場合）を考慮
            if (CurrentPhase != GamePhase.ChoosingCards)
            {
                break;
            }

            var (card, bet) = CpuPlayer.Decide(cpu, CurrentSequence);
            SubmitChoice(cpu, card, bet);
        }
    }

    private void ResolveRound()
    {
        CurrentPhase = GamePhase.Resolving;

        // 判定の実行
        List<PlayerChoice> choicesList = currentChoices.Values.ToList();
        RoundResult result = RoundResolver.Resolve(CurrentSequence, choicesList);

        CurrentPhase = GamePhase.WaitingNext;

        // UIへ結果を通知（アニメーション再生などを開始させる）
        OnRoundResolved?.Invoke(result);
    }

    private void EndGame()
    {
        CurrentPhase = GamePhase.GameOver;

        // 脱落していないプレイヤーの中から最もポイントの高い人を勝者とする
        PlayerState winner = players
            .Where(p => !p.IsOut)
            .OrderByDescending(p => p.Points)
            .FirstOrDefault();

        OnGameOver?.Invoke(winner);
    }
}