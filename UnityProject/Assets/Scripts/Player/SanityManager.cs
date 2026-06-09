using UnityEngine;
using UnityEngine.Events;

namespace Desolation.Player
{
    public class SanityManager : MonoBehaviour
    {
        [Header("Sanity Parameters")]
        [Range(0f, 100f)] [SerializeField] private float maxSanity = 100f;
        [SerializeField] private float passiveSanityDrain = 0.5f; // Drain when in total darkness or creepy corridors
        [SerializeField] private float proximityDrainRate = 12f;  // Drain when entity draws close
        
        [Header("Proximity Configuration")]
        [SerializeField] private Transform closestEntity;
        [SerializeField] private float warningDistance = 12f;
        [SerializeField] private float dangerDistance = 4.5f;

        [Header("Events")]
        public UnityEvent onSanityDepleted;
        public UnityEvent<float> onSanityChanged; // Sends current sanity percent (0 to 1)

        public float CurrentSanity { get; private set; }
        public float DistortionSeverity { get; private set; } // Scale of 0..1 based on proximity & sanity

        private void Start()
        {
            CurrentSanity = maxSanity;
        }

        private void Update()
        {
            UpdateSanityDepletion();
            CalculateDistortion();
        }

        private void UpdateSanityDepletion()
        {
            float finalDrain = passiveSanityDrain;

            // Proximity of hostile backrooms entities triggers severe sanity drop
            if (closestEntity != null)
            {
                float distance = Vector3.Distance(transform.position, closestEntity.position);
                if (distance < warningDistance)
                {
                    // Linear interpolation scale: gets worse as entity creeps closer
                    float factor = 1.0f - Mathf.Clamp01((distance - dangerDistance) / (warningDistance - dangerDistance));
                    finalDrain += factor * proximityDrainRate;
                }
            }

            // Apply calculated degradation
            if (finalDrain > 0f)
            {
                ModifySanity(-finalDrain * Time.deltaTime);
            }
        }

        private void CalculateDistortion()
        {
            float proximityFactor = 0f;
            if (closestEntity != null)
            {
                float distance = Vector3.Distance(transform.position, closestEntity.position);
                proximityFactor = 1.0f - Mathf.Clamp01(distance / warningDistance);
            }

            // Low sanity also creates visual visual artifact distortion independent of entity
            float sanityFactor = 1.0f - (CurrentSanity / maxSanity);

            // Compute cumulative distortion metric for CRT shaders/screens
            DistortionSeverity = Mathf.Clamp01((proximityFactor * 0.7f) + (sanityFactor * 0.3f));
        }

        public void ModifySanity(float changeAmount)
        {
            float previous = CurrentSanity;
            CurrentSanity = Mathf.Clamp(CurrentSanity + changeAmount, 0f, maxSanity);
            
            if (Mathf.Abs(previous - CurrentSanity) > 0.01f)
            {
                onSanityChanged?.Invoke(CurrentSanity / maxSanity);
            }

            if (CurrentSanity <= 0f)
            {
                onSanityDepleted?.Invoke();
            }
        }

        public void SetClosestEntity(Transform entityTransform)
        {
            closestEntity = entityTransform;
        }
    }
}
