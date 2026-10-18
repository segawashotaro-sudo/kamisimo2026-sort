// 置き場所: Assets/Scripts/UI/ResultPanel.cs
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }
    }

    /// <summary>
    /// リザルトパネルを表示し、勝者と全員のスコアを設定する
    /// </summary>
    /// <param name="winner">勝者のPlayerState（引き分けの場合はnull）</param>
    /// <param name="players">参加プレイヤー全員のリスト</param>
    public void Show(PlayerState winner, List<PlayerState> players)
    {
        gameObject.SetActive(true);

        // 勝者の表示
        if (winnerText != null)
        {
            if (winner != null)
            {
                winnerText.text = $"勝者：{winner.Name}";
            }
            else
            {
                winnerText.text = "勝者：なし（引き分け）";
            }
        }

        // 全員のスコア表示
        if (scoreText != null && players != null)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                sb.Append($"{player.Name} {player.Points} pt");

                if (i < players.Count - 1)
                {
                    sb.AppendLine();
                }
            }
            scoreText.text = sb.ToString();
        }
    }

    /// <summary>
    /// リトライボタンクリック時：現在のシーンを再読み込みする
    /// </summary>
    private void OnRetryClicked()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}