using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// プレイヤーのラウンド選択データ
/// </summary>
public class PlayerChoice
{
    public PlayerState Player { get; set; }
    public SortType SelectedCard { get; set; }
    public int BetAmount { get; set; }

    public PlayerChoice(PlayerState player, SortType selectedCard, int betAmount)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        SelectedCard = selectedCard;
        BetAmount = betAmount;
    }
}

/// <summary>
/// ラウンドの解決結果データ
/// </summary>
public class RoundResult
{
    public Dictionary<PlayerState, SortResult> SortResults { get; } = new Dictionary<PlayerState, SortResult>();
    public List<PlayerState> Winners { get; } = new List<PlayerState>();
    public Dictionary<PlayerState, int> PointChanges { get; } = new Dictionary<PlayerState, int>();
    public int TotalPot { get; set; }
}

/// <summary>
/// ラウンドの判定およびポイント計算を行うクラス
/// </summary>
public static class RoundResolver
{
    public static RoundResult Resolve(int[] sequence, List<PlayerChoice> choices)
    {
        if (sequence == null) throw new ArgumentNullException(nameof(sequence));
        if (choices == null) throw new ArgumentNullException(nameof(choices));

        RoundResult result = new RoundResult();

        if (choices.Count == 0)
        {
            return result;
        }

        // 1. 各プレイヤーのベット適用、カード消費、およびソート実行
        int totalPot = 0;
        foreach (var choice in choices)
        {
            var player = choice.Player;
            int bet = choice.BetAmount;

            // 持ち点からベット額を引き、ポットに加算
            player.Points -= bet;
            totalPot += bet;

            // 手札から使用したカードを取り除く
            player.RemoveCard(choice.SelectedCard);

            // ソートを実行して記録
            SortResult sortResult = SortEngine.Run(choice.SelectedCard, sequence);
            result.SortResults[player] = sortResult;

            // ポイント増減の初期化（マイナスベット分）
            result.PointChanges[player] = -bet;
        }

        result.TotalPot = totalPot;

        // 2. 勝者の判定
        // gaveUp が false のプレイヤーのみを対象とする
        var validChoices = choices.Where(c => !result.SortResults[c.Player].gaveUp).ToList();

        if (validChoices.Count == 0)
        {
            // 全員 gaveUp の場合：ベット額を各自に返却
            foreach (var choice in choices)
            {
                var player = choice.Player;
                int bet = choice.BetAmount;

                player.Points += bet;
                result.PointChanges[player] += bet; // 収支は0になる
            }
        }
        else
        {
            // ステップ数が最少の値を求める
            int minSteps = validChoices.Min(c => result.SortResults[c.Player].StepCount);

            // 最少ステップを持つプレイヤー全員を勝者とする
            var winners = validChoices
                .Where(c => result.SortResults[c.Player].StepCount == minSteps)
                .Select(c => c.Player)
                .ToList();

            result.Winners.AddRange(winners);

            // 3. ポットの分配（端数切り捨て）
            int share = totalPot / winners.Count;
            foreach (var winner in winners)
            {
                winner.Points += share;
                result.PointChanges[winner] += share;
            }
        }

        // 4. 脱落チェック（持ち点が0以下なら IsOut = true）
        foreach (var choice in choices)
        {
            var player = choice.Player;
            if (player.Points <= 0)
            {
                player.IsOut = true;
            }
        }

        return result;
    }
}