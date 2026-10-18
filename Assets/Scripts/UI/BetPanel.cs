using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ベット額を決めて「決定」するパネル
// 置き場所: Assets/Scripts/UI/BetPanel.cs（BetPanel オブジェクトに付ける）
public class BetPanel : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] Slider slider;             // BetSlider
    [SerializeField] TextMeshProUGUI betText;   // BetText（「ベット：n」）
    [SerializeField] Button allInButton;        // AllInButton
    [SerializeField] Button confirmButton;      // ConfirmButton

    // 決定ボタンが押されたときに、ベット額を渡して呼ばれる
    public event System.Action<int> OnConfirm;

    // 今のベット額
    public int CurrentBet => slider != null ? Mathf.RoundToInt(slider.value) : 0;

    bool panelInteractable = true;    // パネル全体を操作できるか
    bool confirmAllowed = true;       // 決定ボタンを押してよいか（カード未選択なら false にする）

    void Awake()
    {
        if (slider != null)
        {
            slider.wholeNumbers = true;   // 整数だけにする
            slider.onValueChanged.AddListener(_ => UpdateText());
        }
        if (allInButton != null) allInButton.onClick.AddListener(OnAllInClicked);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);

        UpdateText();
    }

    // スライダーの範囲を min〜max にし、値を min にする
    public void Setup(int min, int max)
    {
        if (slider == null)
        {
            Debug.LogError("BetPanel: Slider が設定されていません。Inspector で設定してください。");
            return;
        }

        if (max < 0) max = 0;
        if (max < min) min = max;   // 持ち点が最低ベット額より少ないときは全額だけ

        // 範囲の設定順で値がずれないよう、一度広げてから設定する
        slider.minValue = 0;
        slider.maxValue = max;
        slider.minValue = min;
        slider.value = min;

        UpdateText();
    }

    // パネル全体（スライダー・オールイン・決定）の有効・無効
    public void SetInteractable(bool on)
    {
        panelInteractable = on;
        ApplyInteractable();
    }

    // 決定ボタンだけの有効・無効（カードを選ぶまで押せないようにするときに使う）
    public void SetConfirmInteractable(bool on)
    {
        confirmAllowed = on;
        ApplyInteractable();
    }

    // ---- ここから下は内部用 ----

    void ApplyInteractable()
    {
        if (slider != null) slider.interactable = panelInteractable;
        if (allInButton != null) allInButton.interactable = panelInteractable;
        // 決定ボタンは「パネルが有効」かつ「決定してよい」ときだけ押せる
        if (confirmButton != null) confirmButton.interactable = panelInteractable && confirmAllowed;
    }

    void OnAllInClicked()
    {
        if (slider == null) return;
        slider.value = slider.maxValue;
        UpdateText();
    }

    void OnConfirmClicked()
    {
        OnConfirm?.Invoke(CurrentBet);
    }

    void UpdateText()
    {
        if (betText != null) betText.text = $"ベット：{CurrentBet}";
    }
}
