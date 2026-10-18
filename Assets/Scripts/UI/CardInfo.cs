using System.Collections.Generic;
using UnityEngine;

// カードの見た目用の情報（日本語名・一言説明・色）
public static class CardInfo
{
    static readonly Dictionary<SortType, (string name, string desc, Color color)> table = new()
    {
        { SortType.Bubble,    ("バブルソート",   "となり同士を比べてコツコツ交換。遅いけど確実", new Color(0.55f, 0.75f, 1f)) },
        { SortType.Selection, ("選択ソート",     "いちばん小さいのを探して前へ。いつも同じペース", new Color(0.6f, 0.85f, 0.9f)) },
        { SortType.Insertion, ("挿入ソート",     "トランプのように差し込む。ほぼ並んでいると速い！", new Color(0.6f, 0.9f, 0.6f)) },
        { SortType.Quick,     ("クイックソート", "小さい組・大きい組に分ける。速いけど苦手な形あり", new Color(1f, 0.7f, 0.4f)) },
        { SortType.Merge,     ("マージソート",   "半分に分けて合体。どんな形でも安定して速い", new Color(1f, 0.85f, 0.4f)) },
        { SortType.Heap,      ("ヒープソート",   "山を作って大きい順に取り出す。安定して速い", new Color(0.9f, 0.6f, 0.9f)) },
        { SortType.Shell,     ("シェルソート",   "離れた所から整える。形しだいで化ける", new Color(0.7f, 0.7f, 1f)) },
        { SortType.Counting,  ("カウントソート", "数を数えて並べる。同じ数が多いと超速い", new Color(0.5f, 0.9f, 0.8f)) },
        { SortType.Tim,       ("ティムソート",   "挿入とマージのいいとこどり。かなり強い", new Color(1f, 0.6f, 0.6f)) },
        { SortType.Bogo,      ("ボゴソート",     "シャッフルして運だめし！奇跡が起きれば最速", new Color(0.8f, 0.8f, 0.8f)) },
    };

    public static string Name(SortType t) => table[t].name;
    public static string Description(SortType t) => table[t].desc;
    public static Color CardColor(SortType t) => table[t].color;
}