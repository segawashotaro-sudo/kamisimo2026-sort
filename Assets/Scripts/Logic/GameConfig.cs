// 調整用の数値。テストプレイで変えるときはここだけ直す
public static class GameConfig
{
    public const int InitialPoints = 100;   // 最初の持ち点
    public const int MinRounds = 3;         // 最少ラウンド数
    public const int MaxRounds = 5;         // 最多ラウンド数
    public const int DefaultRounds = 4;     // 標準のラウンド数
    public const int MinBet = 5;            // 最低ベット額（仮）
    public const int HandLimit = 3;         // 手札の上限
    public const int SequenceMinLength = 5; // 数列の最短の長さ
    public const int SequenceMaxLength = 8; // 数列の最長の長さ
    public const int ValueMin = 1;          // 数列の値の最小
    public const int ValueMax = 9;          // 数列の値の最大
    public const int BogoMaxSteps = 500;    // ボゴソートの打ち切りステップ数（仮）
}
