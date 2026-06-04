# Missing Features / Upgrade Plan

This file tracks what is still missing before the project feels like a high-end, realistic Backrooms Level 0 Android horror game.

## Current state

The project has a playable Godot 4.4.1 APK build path and a true 3D Level 0 foundation. It is still closer to an atmospheric prototype than a full commercial-feeling Backrooms game.

## Missing gameplay

- A complete objective loop, such as find the exit door, collect fuses, restore lights, or follow environmental clues.
- Win and lose conditions.
- A threat system that stays psychological but creates pressure, such as a simple roaming entity, proximity tension, or sanity.
 - An interaction system for notes, doors, switches, collectibles, and environmental events.
 - Save or checkpoint logic for mobile players.

## Missing mobile features

- Refined virtual joystick, look area, sprint button, pause button, and UI scaling.
- Settings for sensitivity, volume, brightness, graphics, frame cap, and control layout.
- Main menu, pause menu, credits, restart, and back/quit behavior.
- Low-data download asset for the compressed `.apk.7z` file. Android cannot install `.sevenZ`/`.7z` directly; the APK must be extracted first.

## Missing visual and audio polish

- More modular Level 0 variety: damaged wallpaper, pillars, dead ends, maintenance rooms, and rare landmarks.
- More realistic materials: wallpaper, damp carpet, ceiling tiles, stains, dirt, and grime.
- Lighting variety: broken lights, flicker zones, dark pockets, and fluorescent color variation.
- Mobile-safe vignette, film grain, color grading, subtle fog, and head-bob tuning.
- Fluorescent hum, carpet footsteps, distant thuds, buzzing, flicker sounds, and menu sounds.

## Missing optimization

- Release export mode; the current pipeline uses debug export for reliability.
- Smaller texture budgets, audio compression, and unused asset removal.
- Occlusion and visibility optimization.
- Cheap lighting, baked/static lights where possible, and very limited real-time shadows.
- APK size pass: keep only required CPU architecture and minimize Android template bloat.

## Best next order

1. Add main menu, pause menu, and settings.
2. Add objective system and exit-door ending.
3. Add polished Android touch controls.
4. Add footsteps, fluorescent hum, and random ambience.
5. Add a simple psychological threat/entity.
6. Add more Level 0 room variation.
7. Convert debug build into a smaller release build.
8. Continue APK size optimization.
