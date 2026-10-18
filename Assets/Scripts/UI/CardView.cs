using TMPro;
using UnityEngine;
using UnityEngine.UI;

// カード1枚の見た目を担当する
// 置き場所: Assets/Scripts/UI/CardView.cs（Card の Prefab に付ける）
public class CardView : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] TextMeshProUGUI nameText;   // カード名
    [SerializeField] TextMeshProUGUI descText;   // 一言説明
    [SerializeField] Image background;           // カードの背景（Card 自身の Image）
    [SerializeField] GameObject selectedFrame;   // 選択中に出す黄色い枠
    [SerializeField] Button button;              // Card 自身の Button

    [Header("見た目")]
    [SerializeField] Color backColor = new Color(0.45f, 0.45f, 0.50f); // 裏向きのときの色
    [Tooltip("押せないときにカードを暗くするか。PlayerPanel の ChosenCard ではチェックを外す")]
    [SerializeField] bool dimWhenDisabled = true;

    // 今のカードの種類
    public SortType Type { get; private set; }

    bool hasType;                          // Setup 済みかどうか
    System.Action<CardView> clickHandler;  // 押されたときに呼ぶ処理

    void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);

            // 押せないときに暗くしない設定なら、無効時の色を通常と同じにする
            if (!dimWhenDisabled)
            {
                ColorBlock colors = button.colors;
                colors.disabledColor = colors.normalColor;
                button.colors = colors;
            }
        }
        SetSelected(false);
    }

    // カードの種類を設定して表向きに表示する。onClick は null でもよい
    public void Setup(SortType type, System.Action<CardView> onClick)
    {
        Type = type;
        hasType = true;
        clickHandler = onClick;
        ShowFront();
    }

    // 選択中の枠の表示・非表示
    public void SetSelected(bool on)
    {
        if (selectedFrame != null) selectedFrame.SetActive(on);
    }

    // ボタンを押せる・押せないの切り替え
    public void SetInteractable(bool on)
    {
        if (button != null) button.interactable = on;
    }

    // 裏向き表示（相手のカードを公開前に見せるとき）
    public void ShowBack()
    {
        if (nameText != null) nameText.text = "？";
        if (descText != null) descText.text = "";
        if (background != null) background.color = backColor;
    }

    // 表向き表示
    public void ShowFront()
    {
        if (!hasType)
        {
            Debug.LogWarning("CardView: Setup より前に ShowFront が呼ばれました。");
            return;
        }
        if (nameText != null) nameText.text = CardInfo.Name(Type);
        if (descText != null) descText.text = CardInfo.Description(Type);
        if (background != null) background.color = CardInfo.CardColor(Type);
    }

    // ---- ここから下は内部用 ----

    void OnButtonClicked()
    {
        clickHandler?.Invoke(this);
    }
}