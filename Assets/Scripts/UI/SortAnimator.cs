using System.Collections;
using TMPro;
using UnityEngine;

// SortResult（ソートの手順）を1ステップずつ再生して、ArrayView の棒を動かす
// 置き場所: Assets/Scripts/UI/SortAnimator.cs
public class SortAnimator : MonoBehaviour
{
    [Header("部品")]
    [SerializeField] ArrayView arrayView;        // 棒グラフ
    [SerializeField] TextMeshProUGUI stepText;   // 「ステップ：n」を表示するテキスト

    // 再生中なら true
    public bool IsPlaying { get; private set; }

    // 手順を再生するコルーチン
    // 使い方: StartCoroutine(sortAnimator.Play(result, 0.2f, () => Debug.Log("終わり")));
    public IEnumerator Play(SortResult result, float stepTime, System.Action onFinished)
    {
        if (arrayView == null)
        {
            Debug.LogError("SortAnimator: Array View が設定されていません。Inspector で設定してください。");
            yield break;
        }
        if (result == null || result.initialArray == null)
        {
            Debug.LogError("SortAnimator: SortResult が空です。");
            yield break;
        }

        IsPlaying = true;

        // 初期状態を表示
        arrayView.Show(result.initialArray);
        int count = 0;
        SetStepText(count, false);

        if (result.steps != null)
        {
            foreach (SortStep step in result.steps)
            {
                switch (step.kind)
                {
                    case StepKind.Compare:
                        // 2本を比較色にして待ち、元に戻す
                        arrayView.Highlight(step.a, step.b, arrayView.CompareColor);
                        count++;
                        SetStepText(count, false);
                        yield return Wait(stepTime);
                        arrayView.ResetColors();
                        break;

                    case StepKind.Swap:
                        // 2本を交換色にして入れ替え、待って元に戻す
                        arrayView.Highlight(step.a, step.b, arrayView.SwapColor);
                        arrayView.SwapBars(step.a, step.b);
                        count++;
                        SetStepText(count, false);
                        yield return Wait(stepTime);
                        arrayView.ResetColors();
                        break;

                    case StepKind.Set:
                        // a 番目を交換色にして値を書き換え、待って元に戻す
                        arrayView.Highlight(step.a, step.a, arrayView.SwapColor);
                        arrayView.SetValue(step.a, step.b);
                        count++;
                        SetStepText(count, false);
                        yield return Wait(stepTime);
                        arrayView.ResetColors();
                        break;

                    case StepKind.Shuffle:
                        // 全部の値をシャッフル後の並びに置き換える
                        if (step.newOrder != null) arrayView.SetAll(step.newOrder);
                        count++;
                        SetStepText(count, false);
                        yield return Wait(stepTime);
                        break;
                }
            }
        }

        // 最後の表示
        if (result.gaveUp)
        {
            SetStepText(count, true);
        }
        else
        {
            SetStepText(count, false);
            arrayView.MarkAllDone();
        }

        IsPlaying = false;
        onFinished?.Invoke();
    }

    // ---- ここから下は内部用 ----

    void SetStepText(int count, bool gaveUp)
    {
        if (stepText == null) return;
        stepText.text = gaveUp ? $"ステップ：{count}（あきらめた！）" : $"ステップ：{count}";
    }

    // stepTime が 0 以下なら1フレームだけ待つ
    object Wait(float stepTime)
    {
        if (stepTime <= 0f) return null;
        return new WaitForSeconds(stepTime);
    }
}
