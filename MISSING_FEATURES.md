# Missing Features / Upgrade Plan

This file tracks what is still missing before the project feels like a high-end, realistic Backrooms Level 0 Android horror game.

## Current state

The project now has a playable Godot 4.4.1 APK build path, true 3D Level 0 foundation, generated Backrooms materials, and a first real gameplay pass. It is still not a finished commercial game, but the prototype now includes a complete objective loop, menus, mobile controls, checkpointing, atmosphere, and pressure mechanics.

## Implemented in `implement-missing-features`

- Main menu, pause menu, restart, quit, and settings panel.
- Settings sliders for look sensitivity, brightness, and volume.
- Objective loop: collect three fuses, power the service exit, and escape.
- Win and lose states with end screens.
- Interaction/proximity logic for fuses, notes, and the exit door.
- Mobile checkpoint save/load logic using `user://checkpoint.cfg`.
- Polished virtual joystick, look area, sprint button, pause button, stamina display, and sanity display.
- Simple psychological threat/entity that follows the player and drains sanity when close.
- Ambient event messages, carpet step feedback, flicker/hum/thud text cues, fog, vignette-like pressure shade, and brightness tuning.
- More Level 0 variety: pillars, grime patches, notes, maintenance/dead-end geometry, exit landmark, and extra material usage.

## Still missing gameplay

- A deeper narrative route with multiple endings and more varied clue chains.
- Manual interaction prompts for doors, switches, lockers, and inspectable objects.
- More advanced enemy behavior, line-of-sight hiding, distraction, or stealth tools.
- Multiple checkpoints/save slots and resume-from-menu UX.

## Still missing mobile features

- Full control layout customization.
- Frame cap and graphics quality presets.
- Better Android back-button handling across all menu states.
- Low-data download page/asset instructions for the compressed `.apk.7z` file. Android cannot install `.sevenZ`/`.7z` directly; the APK must be extracted first.

## Still missing visual and audio polish

- More modular Level 0 sets: damaged wallpaper variants, maintenance rooms, dead ends, and rare landmarks.
- Real audio assets for fluorescent hum, carpet footsteps, distant thuds, buzzing, flicker sounds, and menu sounds.
- Real post-processing pass for film grain, color grading, vignettes, and tuned head bob.
- More lighting variety with authored broken lights, dark pockets, and flicker zones.

## Still missing optimization

- Release export mode; the current pipeline uses debug export for reliability.
- Smaller texture budgets, audio compression, and unused asset removal.
- Occlusion and visibility optimization.
- Cheap lighting, baked/static lights where possible, and very limited real-time shadows.
- APK size pass: keep only required CPU architecture and minimize Android template bloat.

## Best next order

1. Add real audio assets and connect them to footsteps, hum, ambience, and menus.
2. Add manual interaction prompts and switch/door logic.
3. Expand Level 0 with more room modules and authored landmarks.
4. Add graphics/frame-cap settings and control layout customization.
5. Improve the threat with stealth, hiding, and line-of-sight behavior.
6. Convert debug build into a smaller release build.
7. Continue APK size optimization.
