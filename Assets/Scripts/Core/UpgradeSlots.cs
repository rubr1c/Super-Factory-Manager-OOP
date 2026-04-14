using Data.Items;

namespace Core
{
    public class UpgradeSlots
    {
        private readonly UpgradeCardItem[] _cards;

        public int Capacity => _cards.Length;

        public UpgradeSlots(int capacity)
        {
            _cards = new UpgradeCardItem[capacity];
        }

        public bool TryInstall(UpgradeCardItem card)
        {
            if (card == null)
            {
                return false;
            }

            for (var i = 0; i < _cards.Length; i++)
            {
                if (_cards[i] == null)
                {
                    _cards[i] = card;
                    return true;
                }
            }

            return false;
        }

        public UpgradeCardItem TryRemove(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _cards.Length || _cards[slotIndex] == null)
            {
                return null;
            }

            var removed = _cards[slotIndex];
            _cards[slotIndex] = null;
            return removed;
        }

        public UpgradeCardItem GetCard(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _cards.Length)
            {
                return null;
            }

            return _cards[slotIndex];
        }

        public MachineModifiers ComputeModifiers()
        {
            var result = MachineModifiers.Default;
            for (var i = 0; i < _cards.Length; i++)
            {
                var card = _cards[i];
                if (card == null)
                {
                    continue;
                }

                result.Speed *= card.SpeedMultiplier;
                result.Energy *= card.EnergyMultiplier;
                result.Yield *= card.YieldMultiplier;
            }

            return result;
        }
    }
}
