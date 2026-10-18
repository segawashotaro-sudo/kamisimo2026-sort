// 置き場所: Assets/Scripts/UI/GameUIController.cs
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("参照管理")]
    [SerializeField] private GameManager gameManager;

    [Header("プレイヤーパネル")]
    [SerializeField] private PlayerPanel playerPanelLeft;   // あなた（人間）
    [SerializeField] private PlayerPanel playerPanelRight;  // CPU

    [Header("場の表示")]
    [SerializeField] private ArrayView arrayView;

    [Header("テキストUI")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI potText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("操作UI")]
    [SerializeField] private HandView handView;
    [SerializeField] private BetPanel betPanel;
    [SerializeField] private Button nextButton;

    private PlayerState humanPlayer;
    private PlayerState cpuPlayer;

    private void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("[GameUIController] GameManager が Inspector で設定されていません。");
            return;
        }

        // 1. プレイヤーStateの生成（あなた と CPU）
        humanPlayer = new PlayerState("あなた", isCpu: false, initialPoints: GameConfig.InitialPoints);
        cpuPlayer = new PlayerState("CPU", isCpu: true, initialPoints: GameConfig.InitialPoints);

        // 2. GameManager のイベントに登録
        gameManager.OnRoundStarted += HandleRoundStarted;
        gameManager.OnChoicePhaseStarted += HandleChoicePhaseStarted;
        gameManager.OnRoundResolved += HandleRoundResolved;
        gameManager.OnGameOver += HandleGameOver;

        // 3. UIのイベント登録
        if (handView != null)
        {
            handView.OnCardSelected += HandleCardSelected;
        }

        if (betPanel != null)
        {
            betPanel.OnConfirm += HandleBetConfirmed;
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(HandleNextButtonClicked);
            nextButton.gameObject.SetActive(false);
        }

        // 4. ゲームの開始
        List<PlayerState> initialPlayers = new List<PlayerState> { humanPlayer, cpuPlayer };
        gameManager.StartGame(initialPlayers, GameConfig.DefaultRounds);
    }

    private void OnDestroy()
    {
        // イベント解除
        if (gameManager != null)
        {
            gameManager.OnRoundStarted -= HandleRoundStarted;
            gameManager.OnChoicePhaseStarted -= HandleChoicePhaseStarted;
            gameManager.OnRoundResolved -= HandleRoundResolved;
            gameManager.OnGameOver -= HandleGameOver;
        }

        if (handView != null)
        {
            handView.OnCardSelected -= HandleCardSelected;
        }

        if (betPanel != null)
        {
            betPanel.OnConfirm -= HandleBetConfirmed;
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(HandleNextButtonClicked);
        }
    }

    // --- イベントハンドラ ---

    /// <summary>
    /// ラウンド開始時
    /// </summary>
    private void HandleRoundStarted(int currentRound, int[] sequence)
    {

        // ラウンド数表示
        if (roundText != null)
        {
            roundText.text = $"ラウンド {currentRound} / {gameManager.TotalRounds}";
        }

        // ポット表示のリセット
        if (potText != null)
        {
            potText.text = "ポット：0 pt";
        }

        // 場の数列表示
        if (arrayView != null)
        {
            arrayView.Show(sequence);
        }

        // プレイヤーパネルの初期化・状態クリア
        if (playerPanelLeft != null)
        {
            playerPanelLeft.Setup(humanPlayer.Name, humanPlayer.Points);
            playerPanelLeft.SetStatus("考え中…");
            playerPanelLeft.HideCard();
            playerPanelLeft.SetWinner(false);
        }

        if (playerPanelRight != null)
        {
            playerPanelRight.Setup(cpuPlayer.Name, cpuPlayer.Points);
            playerPanelRight.SetStatus("考え中…");
            playerPanelRight.HideCard();
            playerPanelRight.SetWinner(false);
        }

        // 次へボタンの非表示
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤーの選択フェーズ開始時
    /// </summary>
    private void HandleChoicePhaseStarted()
    {
        // 手札の表示と操作許可
        if (handView != null)
        {
            handView.Show(humanPlayer.Hand);
            handView.SetInteractable(true);
            handView.ClearSelection();
        }

        // ベットパネルのセットアップ（持ち点が MinBet 未満なら全額ベット用に可変）
        if (betPanel != null)
        {
            int minBet = Mathf.Min(GameConfig.MinBet, humanPlayer.Points);
            int maxBet = humanPlayer.Points;

            betPanel.Setup(minBet, maxBet);
            betPanel.SetInteractable(true);
            // カードを選択するまで決定ボタンを押せないようにする
            betPanel.SetConfirmInteractable(false);
        }

        if (messageText != null)
        {
            messageText.text = "カードを選んでベットしよう！";
        }
    }

    /// <summary>
    /// 手札からカードが選択された時
    /// </summary>
    private void HandleCardSelected(SortType selectedCard)
    {
        // カードが選ばれたので決定ボタンを有効化
        if (betPanel != null)
        {
            betPanel.SetConfirmInteractable(true);
        }
    }

    /// <summary>
    /// ベットパネルの決定ボタンが押された時
    /// </summary>
    private void HandleBetConfirmed(int betAmount)
    {
        if (!handView.SelectedCard.HasValue)
        {
            return;
        }

        SortType selectedCard = handView.SelectedCard.Value;

        // 手札とベットパネルの操作不可化
        if (handView != null)
        {
            handView.SetInteractable(false);
        }

        if (betPanel != null)
        {
            betPanel.SetInteractable(false);
        }

        // GameManager へ選択を提出
        bool success = gameManager.SubmitChoice(humanPlayer, selectedCard, betAmount);

        if (success)
        {
            // 左パネル（人間）の表示更新
            if (playerPanelLeft != null)
            {
                playerPanelLeft.SetStatus("決定！");
                playerPanelLeft.ShowCard(selectedCard);
            }

            // 右パネル（CPU）の表示更新（裏向き）
            if (playerPanelRight != null)
            {
                playerPanelRight.SetStatus("決定！");
                playerPanelRight.ShowCardBack();
            }

            if (messageText != null)
            {
                messageText.text = "勝負！";
            }
        }
    }

    /// <summary>
    /// ラウンド結果解決時
    /// </summary>
    private void HandleRoundResolved(RoundResult result)
    {
        // CPUが出したカードの開示
        if (playerPanelRight != null && result.SortResults.ContainsKey(cpuPlayer))
        {
            SortType cpuCard = result.SortResults[cpuPlayer].type;
            playerPanelRight.ShowCard(cpuCard);
        }

        // ポット額の更新表示
        if (potText != null)
        {
            potText.text = $"ポット：{result.TotalPot} pt";
        }

        // 勝者判定と結果表示メッセージの構成
        bool humanIsWinner = result.Winners.Contains(humanPlayer);
        bool cpuIsWinner = result.Winners.Contains(cpuPlayer);

        if (playerPanelLeft != null)
        {
            playerPanelLeft.SetWinner(humanIsWinner);
            playerPanelLeft.SetPoints(humanPlayer.Points);
        }

        if (playerPanelRight != null)
        {
            playerPanelRight.SetWinner(cpuIsWinner);
            playerPanelRight.SetPoints(cpuPlayer.Points);
        }

        if (messageText != null)
        {
            if (humanIsWinner && cpuIsWinner)
            {
                messageText.text = "引き分け！";
            }
            else if (humanIsWinner)
            {
                messageText.text = "あなたの勝ち！";
            }
            else if (cpuIsWinner)
            {
                messageText.text = "CPUの勝ち…";
            }
            else
            {
                // 全員 gaveUp （ボゴソート等の諦め）の場合
                messageText.text = "引き分け！（全員諦め）";
            }
        }

        // 「次へ」ボタンを表示
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 次へボタンクリック時
    /// </summary>
    private void HandleNextButtonClicked()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }

        gameManager.NextRound();
    }

    /// <summary>
    /// ゲームオーバー時
    /// </summary>
    private void HandleGameOver(PlayerState winner)
    {

        // 操作UIの完全ロック
        if (handView != null)
        {
            handView.SetInteractable(false);
        }

        if (betPanel != null)
        {
            betPanel.SetInteractable(false);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }

        // 勝者メッセージの更新
        if (messageText != null)
        {
            if (winner != null)
            {
                messageText.text = $"ゲーム終了！勝者：{winner.Name}";
            }
            else
            {
                messageText.text = "ゲーム終了！引き分け";
            }
        }
    }
}