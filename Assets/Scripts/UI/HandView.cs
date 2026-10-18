using System.Collections.Generic;
using UnityEngine;

// 手札（最大3枚）を表示し、1枚を選べるようにする
// 置き場所: Assets/Scripts/UI/HandView.cs（HandArea に付ける）
public class HandView : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] CardView cardPrefab;   // Assets/Prefabs/Card をドラッグする

    // カードが選ばれたときに呼ばれる
    public event System.Action<SortType> OnCardSelected;

    // 選択中のカード（未選択なら null）
    public SortType? SelectedCard => selected != null ? selected.Type : (SortType?)null;

    readonly List<CardView> cards = new List<CardView>();
    CardView selected;          // 同じ種類のカードが2枚あっても区別できるよう、カード自体を覚える
    bool interactable = true;

    // 今あるカードを全部消して、手札の枚数だけカードを作る
    public void Show(List<SortType> hand)
    {
        ClearCards();

        if (cardPrefab == null)
        {
            Debug.LogError("HandView: Card Prefab が設定されていません。Inspector で設定してください。");
            return;
        }
        if (hand == null) return;

        foreach (SortType type in hand)
        {
            CardView card = Instantiate(cardPrefab, transform);
            card.name = $"Card_{type}";
            card.Setup(type, OnCardClicked);
            card.SetSelected(false);
            card.SetInteractable(interactable);
            cards.Add(card);
        }
    }

    // 選択を解除する
    public void ClearSelection()
    {
        selected = null;
        foreach (CardView card in cards) card.SetSelected(false);
    }

    // 全部のカードを押せる・押せないにする（この後 Show で作るカードにも反映される）
    public void SetInteractable(bool on)
    {
        interactable = on;
        foreach (CardView card in cards) card.SetInteractable(on);
    }

    // ---- ここから下は内部用 ----

    void OnCardClicked(CardView clicked)
    {
        if (!interactable) return;

        selected = clicked;
        foreach (CardView card in cards) card.SetSelected(card == clicked);

        OnCardSelected?.Invoke(clicked.Type);
    }

    void ClearCards()
    {
        foreach (CardView card in cards)
        {
            if (card == null) continue;
            // すぐにレイアウトから外すため、親から切り離してから消す
            card.transform.SetParent(null);
            Destroy(card.gameObject);
        }
        cards.Clear();
        selected = null;
    }
}
