using System;
using System.Collections.Generic;

public class PlayerState
{
    public string Name { get; set; }
    public int Points { get; set; }
    public List<SortType> Hand { get; private set; }
    public bool IsCpu { get; set; }
    public bool IsOut { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerState(string name, bool isCpu = false, int initialPoints = GameConfig.InitialPoints)
    {
        Name = name;
        Points = initialPoints;
        Hand = new List<SortType>();
        IsCpu = isCpu;
        IsOut = false;
    }

    /// <summary>
    /// 手札が GameConfig.HandLimit 未満なら追加して true、満杯なら追加せず false
    /// </summary>
    public bool AddCard(SortType card)
    {
        if (Hand.Count < GameConfig.HandLimit)
        {
            Hand.Add(card);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 手札から指定のカードを1枚取り除く（存在すれば true、無ければ false）
    /// </summary>
    public bool RemoveCard(SortType card)
    {
        return Hand.Remove(card);
    }

    /// <summary>
    /// 指定額のベットが可能かどうかを判定する
    /// amount が MinBet 以上かつ points 以下なら true。
    /// （ただし points が MinBet 未満のときは、所持ポイント全額（オールイン）なら true）
    /// </summary>
    public bool CanBet(int amount)
    {
        // そもそも手持ち以上のポイントは指定できない
        if (amount > Points)
        {
            return false;
        }

        // 持ち点が MinBet 未満の場合：全額（オールイン）であれば OK
        if (Points < GameConfig.MinBet)
        {
            return amount == Points && amount > 0;
        }

        // 持ち点が MinBet 以上の場合：amount が MinBet 以上であれば OK
        return amount >= GameConfig.MinBet;
    }
}