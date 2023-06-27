using System.Linq;
using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "AvatarUrlCollection", menuName = "Tools/AvatarUrlCollection", order = 1)]
public class AvatarUrlCollection : ScriptableObject
{
    public string[] urls;

    [ContextMenu("Remove Duplicates")]
    public void RemoveDuplicates()
    {
        urls = urls.Distinct().ToArray();
    }
}
