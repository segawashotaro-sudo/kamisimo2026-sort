using UnityEngine;

public class SortEngineTester : MonoBehaviour
{
    // 試したいソートをここに並べる。実装したら1つずつ追加する
    SortType[] targets = { SortType.Bubble, SortType.Selection, SortType.Insertion, SortType.Quick, SortType.Merge, SortType.Heap, SortType.Shell};

    void Start()
    {
        int[] sample = { 5, 3, 8, 1, 4 };
        foreach (var type in targets)
        {
            SortResult r = SortEngine.Run(type, sample);
            int[] final = Replay(r);
            string ok = IsSorted(final) ? "OK" : "NG！";
            Debug.Log($"{type}: {ok} ステップ数={r.StepCount} 結果=[{string.Join(",", final)}] 諦めた={r.gaveUp}");
        }
        Debug.Log($"元の配列が変わっていないか: [{string.Join(",", sample)}]");
    }

    // 記録された手順を最初の配列に当てはめ直す
    static int[] Replay(SortResult r)
    {
        int[] a = (int[])r.initialArray.Clone();
        foreach (var s in r.steps)
        {
            if (s.kind == StepKind.Swap) { (a[s.a], a[s.b]) = (a[s.b], a[s.a]); }
            else if (s.kind == StepKind.Set) { a[s.a] = s.b; }
            else if (s.kind == StepKind.Shuffle) { a = (int[])s.newOrder.Clone(); }
        }
        return a;
    }

    static bool IsSorted(int[] a)
    {
        for (int i = 1; i < a.Length; i++) if (a[i - 1] > a[i]) return false;
        return true;
    }
}