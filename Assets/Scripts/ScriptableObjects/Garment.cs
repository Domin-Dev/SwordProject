using UnityEngine;

[CreateAssetMenu(fileName = "Garment", menuName = "GameAsset/Items/Garment")]
public class Garment : Item
{
    [Header("Garment Stats")]
    public Sprite top;
    public Sprite rigth;
    public Sprite bottom;
    public Sprite left;

    public Sprite[] GetArray()
    {
        Sprite[] array = {top, rigth, bottom,left};
        return array;
    }
}
