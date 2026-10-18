using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 数列を棒グラフとして表示する
// 置き場所: Assets/Scripts/UI/ArrayView.cs
public class ArrayView : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] GameObject barPrefab;          // 子に TextMeshProUGUI がある Image

    [Header("大きさ")]
    [SerializeField] float maxBarHeight = 400f;     // 最大値のときの棒の高さ
    [SerializeField] int maxValue = 9;              // 数列の値の最大
    [SerializeField] float minBarHeight = 10f;      // 小さい値でも見えるようにする最低の高さ

    [Header("色")]
    [SerializeField] Color normalColor = new Color(0.55f, 0.80f, 1.00f); // 通常
    [SerializeField] Color compareColor = new Color(1.00f, 0.90f, 0.30f); // 比較中
    [SerializeField] Color swapColor = new Color(1.00f, 0.40f, 0.40f);    // 交換中
    [SerializeField] Color doneColor = new Color(0.40f, 0.90f, 0.50f);    // 完了

    // SortAnimator などから色を使えるようにする
    public Color NormalColor => normalColor;
    public Color CompareColor => compareColor;
    public Color SwapColor => swapColor;
    public Color DoneColor => doneColor;

    // 棒1本分の部品
    class Bar
    {
        public GameObject obj;
        public RectTransform rect;
        public Image image;
        public TextMeshProUGUI label;
    }

    readonly List<Bar> bars = new List<Bar>();
    int[] values = new int[0];

    // 今ある棒を全部消してから、values の数だけ棒を作る
    public void Show(int[] newValues)
    {
        ClearBars();

        if (newValues == null) newValues = new int[0];
        values = (int[])newValues.Clone();

        if (barPrefab == null)
        {
            Debug.LogError("ArrayView: Bar Prefab が設定されていません。Inspector で設定してください。");
            return;
        }

        for (int i = 0; i < values.Length; i++)
        {
            GameObject obj = Instantiate(barPrefab, transform);
            obj.name = $"Bar_{i}";

            var bar = new Bar
            {
                obj = obj,
                rect = obj.GetComponent<RectTransform>(),
                image = obj.GetComponent<Image>(),
                label = obj.GetComponentInChildren<TextMeshProUGUI>()
            };
            bars.Add(bar);

            UpdateBar(i);
            SetBarColor(i, normalColor);
        }
    }

    // i 番目と j 番目の棒の色を変える
    public void Highlight(int i, int j, Color color)
    {
        SetBarColor(i, color);
        SetBarColor(j, color);
    }

    // 全部の棒を通常色に戻す
    public void ResetColors()
    {
        for (int i = 0; i < bars.Count; i++) SetBarColor(i, normalColor);
    }

    // i 番目と j 番目の値を入れ替える（オブジェクト自体は動かさない）
    public void SwapBars(int i, int j)
    {
        if (!IsValidIndex(i) || !IsValidIndex(j)) return;
        (values[i], values[j]) = (values[j], values[i]);
        UpdateBar(i);
        UpdateBar(j);
    }

    // i 番目を指定の値にする
    public void SetValue(int i, int value)
    {
        if (!IsValidIndex(i)) return;
        values[i] = value;
        UpdateBar(i);
    }

    // 全部の値を一度に置き換える（数が同じ前提）
    public void SetAll(int[] newValues)
    {
        if (newValues == null) return;
        if (newValues.Length != values.Length)
        {
            Debug.LogWarning("ArrayView: SetAll の配列の長さが違うため、作り直します。");
            Show(newValues);
            return;
        }
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = newValues[i];
            UpdateBar(i);
        }
    }

    // 全部の棒を完了色にする
    public void MarkAllDone()
    {
        for (int i = 0; i < bars.Count; i++) SetBarColor(i, doneColor);
    }

    // 今表示している値の配列を返す（コピーを返すので、外で書き換えても表示は変わらない）
    public int[] CurrentValues()
    {
        return (int[])values.Clone();
    }

    // ---- ここから下は内部用 ----

    void UpdateBar(int i)
    {
        if (!IsValidIndex(i)) return;
        Bar bar = bars[i];

        float ratio = maxValue > 0 ? (float)values[i] / maxValue : 0f;
        float height = Mathf.Max(minBarHeight, ratio * maxBarHeight);

        if (bar.rect != null)
        {
            Vector2 size = bar.rect.sizeDelta;
            size.y = height;
            bar.rect.sizeDelta = size;
        }
        if (bar.label != null) bar.label.text = values[i].ToString();
    }

    void SetBarColor(int i, Color color)
    {
        if (!IsValidIndex(i)) return;
        if (bars[i].image != null) bars[i].image.color = color;
    }

    bool IsValidIndex(int i)
    {
        return i >= 0 && i < bars.Count && i < values.Length;
    }

    void ClearBars()
    {
        foreach (Bar bar in bars)
        {
            if (bar.obj == null) continue;
            // すぐにレイアウトから外すため、親から切り離してから消す
            bar.obj.transform.SetParent(null);
            Destroy(bar.obj);
        }
        bars.Clear();
    }
}