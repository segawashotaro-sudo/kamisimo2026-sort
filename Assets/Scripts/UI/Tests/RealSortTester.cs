using UnityEngine;

public class RealSortTester : MonoBehaviour
{
    public SortAnimator sortAnimator;
    public SortType type = SortType.Bubble;            // Inspector で切り替えて試す
    public int[] sequence = { 7, 2, 9, 4, 1, 6, 3 };   // Inspector で書き換えて試す
    public float maxTotalSeconds = 10f;                // アニメ全体の最大の長さ

    void Start()
    {
        SortResult r = SortEngine.Run(type, sequence);
        float stepTime = Mathf.Min(0.15f, maxTotalSeconds / Mathf.Max(1, r.StepCount));
        Debug.Log($"{CardInfo.Name(type)}：{r.StepCount}ステップ、1ステップ {stepTime:F3}秒、諦めた={r.gaveUp}");
        StartCoroutine(sortAnimator.Play(r, stepTime, () => Debug.Log("アニメ終了")));
    }
}