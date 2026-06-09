using UnityEngine;
using UnityEngine.Events;

namespace Desolation.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Inventory Status")]
        [SerializeField] private int maxAlmondWaterBottles = 5;
        [SerializeField] private float almondWaterSanityRestore = 35f;

        [Header("Events")]
        public UnityEvent<int> onAlmondWaterCountChanged;
        public UnityEvent onBottleConsumed;
        public UnityEvent onInventoryFull;

        public int AlmondWaterCount { get; private set; } = 0;

        private Player.SanityManager sanityManager;

        private void Awake()
        {
            sanityManager = GetComponent<Player.SanityManager>();
        }

        public bool AddAlmondWater()
        {
            if (AlmondWaterCount >= maxAlmondWaterBottles)
            {
                onInventoryFull?.Invoke();
                return false; // Could not collect, stock limit met
            }

            AlmondWaterCount++;
            onAlmondWaterCountChanged?.Invoke(AlmondWaterCount);
            return true;
        }

        public void ConsumeAlmondWater()
        {
            if (AlmondWaterCount <= 0) return;

            AlmondWaterCount--;
            onAlmondWaterCountChanged?.Invoke(AlmondWaterCount);
            onBottleConsumed?.Invoke();

            if (sanityManager != null)
            {
                sanityManager.ModifySanity(almondWaterSanityRestore);
            }
        }
    }
}
