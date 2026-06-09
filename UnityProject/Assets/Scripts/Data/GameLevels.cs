using System.Collections.Generic;
using UnityEngine;

namespace Desolation.Data
{
    [System.Serializable]
    public class LevelData
    {
        public int id;
        public string name;
        public string description;
        public List<string> grid;
        public float spawnX;
        public float spawnY;
        public Color wallColor;
        public Color wallShadowColor;
        public Color floorColor;
        public Color ceilingColor;
        public float floorAmbientIntensity; // ambient light baseline
        public float ambientHumFrequency; // sound effect feedback
    }

    public static class GameLevels
    {
        public static readonly List<LevelData> levels = new List<LevelData>()
        {
            // Level 0: Classic Yellow Carpets and Wallpaper
            new LevelData()
            {
                id = 0,
                name = "Level 0: The Lobby",
                description = "An endless maze of yellow plaster walls, damp carpets, and buzzing fluorescent hum.",
                grid = new List<string>()
                {
                    "1111111111111111",
                    "1000001000000001",
                    "1011101011111101",
                    "1010001010000101",
                    "1010111010110101",
                    "1000100000110001",
                    "1110111110111101",
                    "1000000010000101",
                    "1011111011110101",
                    "1010001000010001",
                    "1010101111011101",
                    "1000101001000101",
                    "1111101001110101",
                    "1000001000010101",
                    "1011111111000001", // Exit door positioned here
                    "1111111111111111"
                },
                spawnX = 1.5f,
                spawnY = 1.5f,
                wallColor = new Color(0.84f, 0.76f, 0.45f), // Dull Mustard plaster
                wallShadowColor = new Color(0.62f, 0.56f, 0.33f), // Shadowed Mustard
                floorColor = new Color(0.30f, 0.26f, 0.13f), // Wet brownish carpet
                ceilingColor = new Color(0.75f, 0.74f, 0.70f), // Off-white office panels
                floorAmbientIntensity = 0.55f,
                ambientHumFrequency = 120f
            },

            // Level 1: Pipe Dreams - Industrial Metallic Tunnels
            new LevelData()
            {
                id = 1,
                name = "Level 1: Pipe Dreams",
                description = "Dark concrete corridors surrounded by metal pipes, rust, dripping steam, and puddles.",
                grid = new List<string>()
                {
                    "1111111111111111",
                    "1000100000000001",
                    "1010101111110111",
                    "1010001000010001",
                    "1011101011011101",
                    "1000100011000101",
                    "1110111011110101",
                    "1000001000010001",
                    "1011101111011101",
                    "1010100001000101",
                    "1010111101110101",
                    "1000000100010001",
                    "1111110111011101",
                    "1000010001010001",
                    "1011000000000021", // 2 represents Exit Hatch
                    "1111111111111111"
                },
                spawnX = 1.5f,
                spawnY = 1.5f,
                wallColor = new Color(0.37f, 0.39f, 0.41f), // Rusty Steel Iron Gray
                wallShadowColor = new Color(0.25f, 0.26f, 0.27f), // Shadowed steel
                floorColor = new Color(0.12f, 0.13f, 0.14f), // Damp slate-grey concrete
                ceilingColor = new Color(0.17f, 0.18f, 0.19f), // Heavy industrial ceiling piping
                floorAmbientIntensity = 0.38f,
                ambientHumFrequency = 80f
            },

            // Level 2: The Habitable Zone - Grim dark office structures
            new LevelData()
            {
                id = 2,
                name = "Level 2: Habitable Zone",
                description = "A labyrinth of cold, dark office basement corridors with severe flickering lights.",
                grid = new List<string>()
                {
                    "1111111111111111",
                    "1000000100000001",
                    "1011110101111101",
                    "1010010001000101",
                    "1010011111010101",
                    "1000000000010001",
                    "1111110111111011",
                    "1000010100000001",
                    "1011010101111101",
                    "1010010001000001",
                    "1011111011111101",
                    "1000001010000101",
                    "1110101010110101",
                    "1000100000110001",
                    "1011111111111021", // Exit elevator threshold
                    "1111111111111111"
                },
                spawnX = 1.5f,
                spawnY = 1.5f,
                wallColor = new Color(0.23f, 0.23f, 0.24f), // Extreme Dark Iron-Grey pillars
                wallShadowColor = new Color(0.12f, 0.12f, 0.12f), // Dark pitch shadows
                floorColor = new Color(0.08f, 0.08f, 0.08f), // Deep Charcoal
                ceilingColor = new Color(0.11f, 0.11f, 0.11f), // Obsidian ceiling panels
                floorAmbientIntensity = 0.25f,
                ambientHumFrequency = 50f
            }
        };
    }
}
