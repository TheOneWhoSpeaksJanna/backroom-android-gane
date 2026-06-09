using UnityEngine;

namespace Desolation.Environment
{
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        [Header("Flicker Setup")]
        [SerializeField] private float minIntensity = 0.1f;
        [SerializeField] private float maxIntensity = 1.3f;
        [Range(0.01f, 1f)] [SerializeField] private float flickerSpeed = 0.08f;
        
        [Header("Ambient Sound Hum")]
        [SerializeField] private AudioSource fluorescentHumSource;
        [Min(0f)] [SerializeField] private float baseHumVolume = 0.4f;

        private Light targetLight;
        private float nextActionTime = 0f;

        private void Awake()
        {
            targetLight = GetComponent<Light>();
        }

        private void Start()
        {
            // Sync initial state of volume
            float soundEnabled = PlayerPrefs.GetInt("Settings_SoundEnabled", 1);
            if (fluorescentHumSource != null)
            {
                fluorescentHumSource.volume = (soundEnabled == 1) ? baseHumVolume : 0f;
                if (soundEnabled == 1 && !fluorescentHumSource.isPlaying)
                {
                    fluorescentHumSource.Play();
                }
            }
        }

        private void Update()
        {
            // Backrooms fluorescent light hum flickering logic based on periodic timer checks
            if (Time.time >= nextActionTime)
            {
                // Generate creepy unstable flickering timings
                float randomWeight = Random.value;
                if (randomWeight < 0.12f)
                {
                    targetLight.intensity = minIntensity; // Glitch off / dim state
                    if (fluorescentHumSource != null) fluorescentHumSource.pitch = Random.Range(0.7f, 0.9f);
                }
                else if (randomWeight > 0.88f)
                {
                    targetLight.intensity = Random.Range(minIntensity, maxIntensity); // Spontaneous sparking flicker
                    if (fluorescentHumSource != null) fluorescentHumSource.pitch = Random.Range(1.1f, 1.3f);
                }
                else
                {
                    targetLight.intensity = maxIntensity; // Normal stable buzzing state
                    if (fluorescentHumSource != null) fluorescentHumSource.pitch = 1.0f;
                }

                // Push next update threshold randomized slightly
                nextActionTime = Time.time + Random.Range(flickerSpeed * 0.5f, flickerSpeed * 1.5f);
            }
        }

        public void SetHumVolume(float masterSettingsMultiplier)
        {
            if (fluorescentHumSource != null)
            {
                fluorescentHumSource.volume = baseHumVolume * masterSettingsMultiplier;
            }
        }
    }
}
