using UnityEngine;

namespace Desolation.Items
{
    public class BatteryAction : MonoBehaviour
    {
        [Header("Pick Animation Settings")]
        [SerializeField] private float bobSpeed = 2.5f;
        [SerializeField] private float bobHeight = 0.12f;
        [SerializeField] private float rotationSpeed = 60f;

        [Header("Stamina Mechanics")]
        [SerializeField] private float staminaBoost = 50f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            // Simple hover & spin effect
            float nextY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPos.x, nextY, startPos.z);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player.PlayerMovement playerMovement = other.GetComponent<Player.PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.BoostStamina(staminaBoost);

                    AudioSource audioSrc = GetComponent<AudioSource>();
                    if (audioSrc != null && audioSrc.clip != null)
                    {
                        AudioSource.PlayClipAtPoint(audioSrc.clip, transform.position);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}
