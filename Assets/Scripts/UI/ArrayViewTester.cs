using UnityEngine;

public class ArrayViewTester : MonoBehaviour
{
    public ArrayView view;

    void Start()
    {
        view.Show(new int[] { 5, 3, 8, 1, 4 });
        view.Highlight(0, 1, Color.yellow);
    }
}