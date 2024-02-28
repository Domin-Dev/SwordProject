using UnityEngine;

public static class MyTools
{
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 0;
        pos = Camera.main.ScreenToWorldPoint(pos);
        return pos;
    }
}
