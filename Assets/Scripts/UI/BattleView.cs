// 置き場所: Assets/Scripts/UI/BattleView.cs
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BattleView : MonoBehaviour
{
    [Header("アニメーター")]
    [SerializeField] private SortAnimator leftAnimator;
    [SerializeField] private SortAnimator rightAnimator;

    [Header("テキスト・演出部品")]
    [SerializeField] private TextMeshProUGUI leftTitleText;
    [SerializeField] private TextMeshProUGUI rightTitleText;
    [SerializeField] private GameObject leftGoalObject;
    [SerializeField] private GameObject rightGoalObject;

    [Header("設定")]
    [SerializeField] private float maxDuration = 10f; // 全体の最大再生時間（秒）

    private TextMeshProUGUI leftGoalText;
    private TextMeshProUGUI rightGoalText;

    private void Awake()
    {
        // ゴール表示オブジェクト内の TextMeshProUGUI を取得しておく（「あきらめた…」などのテキスト変更用）
        if (leftGoalObject != null)
        {
            leftGoalText = leftGoalObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (rightGoalObject != null)
        {
            rightGoalText = rightGoalObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// 左右のソートアニメーションを同時に再生し、対決を表示する
    /// </summary>
    public IEnumerator PlayBattle(string leftName, SortResult left, string rightName, SortResult right)
    {
        // 自分自身を表示
        gameObject.SetActive(true);

        // 1. タイトルテキストの設定
        if (leftTitleText != null && left != null)
        {
            leftTitleText.text = $"{leftName}：{CardInfo.Name(left.type)}";
        }

        if (rightTitleText != null && right != null)
        {
            rightTitleText.text = $"{rightName}：{CardInfo.Name(right.type)}";
        }

        // 2. ゴール表示の初期化（非表示にする）
        if (leftGoalObject != null)
        {
            leftGoalObject.SetActive(false);
        }

        if (rightGoalObject != null)
        {
            rightGoalObject.SetActive(false);
        }

        // 3. 1ステップあたりの時間を計算
        // 「0.15秒」と「最大再生時間 ÷ 2人のうち多い方のステップ数」の小さい方を採用
        int leftSteps = left?.StepCount ?? 0;
        int rightSteps = right?.StepCount ?? 0;
        int maxSteps = Math.Max(leftSteps, rightSteps);

        float stepTime = 0.15f;
        if (maxSteps > 0 && maxDuration > 0f)
        {
            float calculatedStepTime = maxDuration / maxSteps;
            stepTime = Mathf.Min(0.15f, calculatedStepTime);
        }

        // 4. 左右のアニメーションを同時に開始
        bool leftFinished = false;
        bool rightFinished = false;

        if (leftAnimator != null && left != null)
        {
            StartCoroutine(leftAnimator.Play(left, stepTime, () =>
            {
                leftFinished = true;
                ShowGoal(leftGoalObject, leftGoalText, left.gaveUp);
            }));
        }
        else
        {
            leftFinished = true;
        }

        if (rightAnimator != null && right != null)
        {
            StartCoroutine(rightAnimator.Play(right, stepTime, () =>
            {
                rightFinished = true;
                ShowGoal(rightGoalObject, rightGoalText, right.gaveUp);
            }));
        }
        else
        {
            rightFinished = true;
        }

        // 5. 両方のアニメーションが終わるまで待機
        yield return new WaitUntil(() => leftFinished && rightFinished);

        // 6. 余韻としてさらに1秒待機
        yield return new WaitForSeconds(1.0f);
    }

    /// <summary>
    /// 自分自身を非表示にする
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // --- 内部用処理 ---

    /// <summary>
    /// ゴール表示を有効化し、gaveUp に応じてテキストを切り替える
    /// </summary>
    private void ShowGoal(GameObject goalObj, TextMeshProUGUI goalText, bool gaveUp)
    {
        if (goalObj == null) return;

        if (goalText != null)
        {
            goalText.text = gaveUp ? "あきらめた…" : "GOAL!";
        }

        goalObj.SetActive(true);
    }
}