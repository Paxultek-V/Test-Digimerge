using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObject_Extention
{
    public static bool RemoveComponent<Component>(this GameObject go)
    {
        Component component = go.GetComponent<Component>();

        if (component != null)
        {
            Object.DestroyImmediate(component as Object, true);
            return true;
        }

        return false;
    }
}