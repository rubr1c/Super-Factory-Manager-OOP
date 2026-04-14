using Core;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "ResourceItem", menuName = "GameItems/Resource Item")]
    public class ResourceItem : Item, IResourceItem
    {
    }
}
