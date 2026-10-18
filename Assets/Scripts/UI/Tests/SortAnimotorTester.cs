using System.Collections.Generic;
using UnityEngine;

public class SortAnimatorTester : MonoBehaviour
{
    public SortAnimator sortAnimator;

    void Start()
    {
        var r = new SortResult();
        r.type = SortType.Bubble;
        r.initialArray = new int[] { 3, 1, 2 };
        r.steps = new List<SortStep>
        {
            new SortStep(StepKind.Compare, 0, 1),
            new SortStep(StepKind.Swap, 0, 1),     // [1,3,2]
            new SortStep(StepKind.Compare, 1, 2),
            new SortStep(StepKind.Swap, 1, 2),     // [1,2,3]
            new SortStep(StepKind.Compare, 0, 1),
            new SortStep(StepKind.Set, 2, 3),      // 書き込みの見え方テスト
            new SortStep(StepKind.Shuffle, 0, 0, new int[] { 2, 3, 1 }), // シャッフルの見え方テスト
            new SortStep(StepKind.Shuffle, 0, 0, new int[] { 1, 2, 3 }),
        };
        r.gaveUp = false;
        StartCoroutine(sortAnimator.Play(r, 0.5f, () => Debug.Log("アニメ終了")));
    }
}