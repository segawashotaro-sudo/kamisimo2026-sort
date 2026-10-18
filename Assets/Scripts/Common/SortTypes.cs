// 置き場所: Assets/Scripts/Common/SortTypes.cs
// AさんとBさんの「約束事」ファイル。変更するときは必ず2人で相談すること。
using System.Collections.Generic;

// ソートカードの種類
public enum SortType
{
    Bubble,     // バブルソート
    Selection,  // 選択ソート
    Insertion,  // 挿入ソート
    Quick,      // クイックソート
    Merge,      // マージソート
    Heap,       // ヒープソート
    Shell,      // シェルソート
    Counting,   // カウントソート
    Tim,        // ティムソート
    Bogo        // ボゴソート（ギャンブル）
}

// 1ステップの種類
public enum StepKind
{
    Compare,  // a番目とb番目を比較した
    Swap,     // a番目とb番目を入れ替えた
    Set,      // a番目に値bを書き込んだ（マージソートなど交換を使わない場合）
    Shuffle   // 配列全体をシャッフルした（ボゴソート用。newOrderに結果を入れる）
}

// ソートの1ステップ。アニメーションはこれを1つずつ再生する
public struct SortStep
{
    public StepKind kind;
    public int a;
    public int b;
    public int[] newOrder; // Shuffleのときだけ使う

    public SortStep(StepKind kind, int a, int b, int[] newOrder = null)
    {
        this.kind = kind;
        this.a = a;
        this.b = b;
        this.newOrder = newOrder;
    }
}

// ソート1回分の結果。SortEngine（Aさん）が作り、UI（Bさん）が受け取る
public class SortResult
{
    public SortType type;
    public int[] initialArray;    // ソート前の数列
    public List<SortStep> steps;  // 全ステップの記録
    public bool gaveUp;           // ボゴソートが上限に達して諦めた場合true

    public int StepCount => steps.Count; // 勝敗判定に使うステップ数
}
