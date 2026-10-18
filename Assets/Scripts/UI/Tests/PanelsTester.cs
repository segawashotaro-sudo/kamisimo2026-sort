using System.Collections;
using UnityEngine;

public class PanelsTester : MonoBehaviour
{
    public BetPanel bet;
    public PlayerPanel left;
    public PlayerPanel right;

    IEnumerator Start()
    {
        bet.Setup(5, 100);
        bet.OnConfirm += amount => Debug.Log($"ベット決定：{amount}");
        left.Setup("あなた", 100);
        right.Setup("CPU", 100);
        left.SetStatus("考え中…");
        right.SetStatus("決定！");
        right.ShowCardBack();
        yield return new WaitForSeconds(2f);
        left.ShowCard(SortType.Merge);
        right.ShowCard(SortType.Bogo);
        yield return new WaitForSeconds(1f);
        left.SetWinner(true);
        left.SetPoints(130);
        right.SetPoints(70);
    }
}
