using UnityEngine;

namespace Desolation.Items
{
    public class AlmondWater : MonoBehaviour
    {
        [Header("Pick Animation")]
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.15f;
        [SerializeField] private float rotateSpeed = 45f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            // Hover animation for visually polished backrooms asset display
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Inventory.InventoryManager inv = other.GetComponent<Inventory.InventoryManager>();
                if (inv != null)
                {
                    if (inv.AddAlmondWater())
                    {
                        // Play pickup cue audio if attached
                        AudioSource pickupSfx = GetComponent<AudioSource>();
                        if (pickupSfx != null && pickupSfx.clip != null)
                        {
                            AudioSource.PlayClipAtPoint(pickupSfx.clip, transform.position);
                        }

                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
