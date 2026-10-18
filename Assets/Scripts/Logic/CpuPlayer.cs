// 置き場所: Assets/Scripts/Logic/CpuPlayer.cs
using System;
using System.Collections.Generic;
using System.Linq;

public static class CpuPlayer
{
    /// <summary>
    /// CPUの出すカードとベット額を決定する
    /// </summary>
    /// <param name="cpu">CPUプレイヤーの状態</param>
    /// <param name="sequence">対象となる数列</param>
    /// <returns>選択したカードとベット額のタプル</returns>
    public static (SortType card, int bet) Decide(PlayerState cpu, int[] sequence)
    {
        if (cpu == null)
        {
            throw new ArgumentNullException(nameof(cpu));
        }

        if (cpu.Hand == null || cpu.Hand.Count == 0)
        {
            throw new InvalidOperationException("CPUの手札が空です。");
        }

        SortType selectedCard = cpu.Hand[0]; // 初期値を手札の1枚目に指定（CS0165対策）
        int stepDifference = 0; // 最善と次点の手数差（ベット額計算用）

        // 1. 手札が1枚だけの場合は無条件でそれを選択
        if (cpu.Hand.Count == 1)
        {
            selectedCard = cpu.Hand[0];
        }
        else
        {
            bool hasChosenBogo = false;

            // 2. 手札に Bogo があれば、一定の確率(CpuBogoRate)で Bogo を出す（試算なし）
            if (cpu.Hand.Contains(SortType.Bogo) && UnityEngine.Random.value < GameConfig.CpuBogoRate)
            {
                selectedCard = SortType.Bogo;
                hasChosenBogo = true;
            }

            // Bogo が選ばれなかった場合
            if (!hasChosenBogo)
            {
                // Bogo 以外のカードを抽出
                List<SortType> candidates = cpu.Hand.Where(card => card != SortType.Bogo).ToList();

                // 手札が Bogo のみで構成されていた場合
                if (candidates.Count == 0)
                {
                    selectedCard = SortType.Bogo;
                }
                else if (candidates.Count == 1)
                {
                    selectedCard = candidates[0];
                }
                else
                {
                    // 3. Bogo 以外のカードを SortEngine.Run で試算
                    var evaluatedCards = candidates
                        .Select(card => new
                        {
                            Card = card,
                            Result = SortEngine.Run(card, sequence)
                        })
                        .OrderBy(x => x.Result.StepCount)
                        .ToList();

                    // 最良のカードと2番目に良いカードのステップ差を記録
                    stepDifference = evaluatedCards[1].Result.StepCount - evaluatedCards[0].Result.StepCount;

                    // GameConfig.CpuBestChoiceRate の確率で最少ステップのカードを選択
                    if (UnityEngine.Random.value < GameConfig.CpuBestChoiceRate)
                    {
                        selectedCard = evaluatedCards[0].Card;
                    }
                    else
                    {
                        // 残りの確率でランダムなカードを選択
                        int randomIndex = UnityEngine.Random.Range(0, candidates.Count);
                        selectedCard = candidates[randomIndex];
                    }
                }
            }
        }

        // 4. ベット額の計算
        int bet = CalculateBet(cpu, stepDifference);

        return (selectedCard, bet);
    }

    /// <summary>
    /// ステップ差や所持ポイントに応じてベット額を計算する
    /// </summary>
    private static int CalculateBet(PlayerState cpu, int stepDifference)
    {
        // 持ち点が MinBet 未満なら全額ベット（CanBetの仕様に合わせる）
        if (cpu.Points < GameConfig.MinBet)
        {
            return Math.Max(0, cpu.Points);
        }

        // 上限額（持ち点の CpuMaxBetRate %）
        int maxBet = (int)(cpu.Points * (GameConfig.CpuMaxBetRate / 100.0f));
        maxBet = Math.Max(GameConfig.MinBet, maxBet); // 上限が MinBet を下回らないように調整
        maxBet = Math.Min(cpu.Points, maxBet);        // 所持ポイントを超過しないように調整

        // ステップ差に応じてベット額をスケール（差が大きいほど高額）
        float differenceRatio = Math.Min(1.0f, stepDifference / 10.0f);
        
        int bet = GameConfig.MinBet + (int)((maxBet - GameConfig.MinBet) * differenceRatio);

        // 持ち点と設定範囲内に収まるようクランプ
        bet = Math.Clamp(bet, GameConfig.MinBet, maxBet);

        return bet;
    }
}