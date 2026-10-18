using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// プレイヤー1人分の表示（名前・持ち点・状態・出したカード）
// 置き場所: Assets/Scripts/UI/PlayerPanel.cs（PlayerPanelLeft と PlayerPanelRight に付ける）
public class PlayerPanel : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] TextMeshProUGUI nameText;     // 名前
    [SerializeField] TextMeshProUGUI pointsText;   // 持ち点（「n pt」）
    [SerializeField] TextMeshProUGUI statusText;   // 状態（「考え中…」など）
    [SerializeField] CardView chosenCard;          // 出したカード（ChosenCard）
    [SerializeField] Image background;             // パネル自身の Image

    [Header("見た目")]
    [SerializeField] Color winnerColor = new Color(0.85f, 0.65f, 0.15f, 0.9f); // 勝者のときの背景色（金色）
    [SerializeField] float pointsAnimSeconds = 0.8f;                          // 持ち点の数字が動く時間

    Color normalColor;          // 元の背景色
    int displayedPoints;        // 今表示している持ち点
    Coroutine pointsRoutine;    // 持ち点アニメ中のコルーチン

    void Awake()
    {
        if (background != null) normalColor = background.color;
    }

    // 名前と持ち点を表示し、カードを隠し、状態を空にする
    public void Setup(string playerName, int points)
    {
        if (nameText != null) nameText.text = playerName;

        StopPointsAnimation();
        displayedPoints = points;
        UpdatePointsText(points);

        HideCard();
        SetStatus("");
        SetWinner(false);
    }

    // 表示中の値から新しい値まで、数字が増減するアニメで表示する
    public void SetPoints(int points)
    {
        StopPointsAnimation();

        // パネルが非表示のときはアニメできないので、すぐに表示を変える
        if (!gameObject.activeInHierarchy || pointsAnimSeconds <= 0f)
        {
            displayedPoints = points;
            UpdatePointsText(points);
            return;
        }
        pointsRoutine = StartCoroutine(AnimatePoints(displayedPoints, points));
    }

    // 状態の文字を表示する
    public void SetStatus(string text)
    {
        if (statusText != null) statusText.text = text;
    }

    // 出したカードを裏向きで表示する
    public void ShowCardBack()
    {
        if (chosenCard == null) return;
        chosenCard.gameObject.SetActive(true);
        chosenCard.SetSelected(false);
        chosenCard.ShowBack();
    }

    // 出したカードを表向きで表示する
    public void ShowCard(SortType type)
    {
        if (chosenCard == null) return;
        chosenCard.gameObject.SetActive(true);   // 先に表示してから Setup する
        chosenCard.Setup(type, null);
        chosenCard.SetSelected(false);
    }

    // 出したカードを隠す
    public void HideCard()
    {
        if (chosenCard != null) chosenCard.gameObject.SetActive(false);
    }

    // 勝者なら背景を金色に、そうでなければ元の色にする
    public void SetWinner(bool on)
    {
        if (background != null) background.color = on ? winnerColor : normalColor;
    }

    // ---- ここから下は内部用 ----

    IEnumerator AnimatePoints(int from, int to)
    {
        float t = 0f;
        while (t < pointsAnimSeconds)
        {
            t += Time.deltaTime;
            int value = Mathf.RoundToInt(Mathf.Lerp(from, to, t / pointsAnimSeconds));
            displayedPoints = value;
            UpdatePointsText(value);
            yield return null;
        }
        displayedPoints = to;
        UpdatePointsText(to);
        pointsRoutine = null;
    }

    void StopPointsAnimation()
    {
        if (pointsRoutine != null)
        {
            StopCoroutine(pointsRoutine);
            pointsRoutine = null;
        }
    }

    void UpdatePointsText(int points)
    {
        if (pointsText != null) pointsText.text = $"{points} pt";
    }
}
