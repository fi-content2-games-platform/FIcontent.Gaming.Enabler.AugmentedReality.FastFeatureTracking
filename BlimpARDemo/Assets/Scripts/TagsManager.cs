using UnityEngine;
using System.Collections;

public static class TagsManager
{
    public static string GetEnemyOf(string tag)
    {
        switch (tag)
        {
            case "Emperror": return "Resistance";

            case "Resistance": return "Emperror";

            default: return "";
        }
    }
}
