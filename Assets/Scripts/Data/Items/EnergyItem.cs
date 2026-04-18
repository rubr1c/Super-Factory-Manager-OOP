using Core;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "EnergyItem", menuName = "GameItems/Energy Item")]
    public class EnergyItem : Item, IEnergyItem
    {
    }
}
