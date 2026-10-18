using System.Collections.Generic;
using UnityEngine;

public class HandViewTester : MonoBehaviour
{
    public HandView hand;

    void Start()
    {
        hand.OnCardSelected += t => Debug.Log($"選んだカード：{CardInfo.Name(t)}");
        hand.Show(new List<SortType> { SortType.Bubble, SortType.Quick, SortType.Bogo });
    }
}