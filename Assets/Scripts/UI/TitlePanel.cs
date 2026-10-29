using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button howToButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject howToPanel;

    // ゲーム開始イベント
    public event System.Action OnStart;

    private void Awake()
    {
        // ボタンのクリックイベントを登録
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (howToButton != null)
        {
            howToButton.onClick.AddListener(OnHowToButtonClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    private void OnDestroy()
    {
        // イベントの解除（メモリリーク防止）
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        if (howToButton != null)
        {
            howToButton.onClick.RemoveListener(OnHowToButtonClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }

    // タイトルパネルを表示する
    public void Show()
    {
        gameObject.SetActive(true);
        
        // パネル表示時に HowTo パネルを初期状態（非表示）にする
        if (howToPanel != null)
        {
            howToPanel.SetActive(false);
        }
    }

    // スタートボタン押下時
    private void OnStartButtonClicked()
    {
        gameObject.SetActive(false);
        OnStart?.Invoke();
    }

    // 遊び方ボタン押下時
    private void OnHowToButtonClicked()
    {
        if (howToPanel != null)
        {
            howToPanel.SetActive(true);
        }
    }

    // 閉じるボタン押下時
    private void OnCloseButtonClicked()
    {
        if (howToPanel != null)
        {
            howToPanel.SetActive(false);
        }
    }
}