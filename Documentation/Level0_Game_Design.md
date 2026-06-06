# Backrooms Level 0 High-End Direction

This document records the target direction for the Backrooms Level Zero Android game.

## Research baseline

Level 0 should remain focused on the original Lobby mood: endless yellowed rooms, damp carpet, fluorescent lights, repeating office corridors, and liminal emptiness. The visual direction is based on the uploaded reference images and public Level 0 descriptions: mono-yellow wallpaper, soggy/damp carpet, bright fluorescent panels, empty rooms, and occasional terrifying entity encounters.

Useful reference sources:

- Union St. Journal, "Scents of Wet Carpet and Terrifying Entities: Welcome to the Backrooms"
- Apple App Store page for "Backrooms: Lost in Level 0"
- User-provided reference images in `References/images/`

## User reference interpretation

The uploaded images define the local project style:

- Yellow rooms are the core environment.
- The black shape is the monster.
- The yellow people are the player/player team visual language.
- Large plain pillars, wide empty corridors, ceiling panels, fluorescent rectangles, pits, lone chairs, and strange doorways should be repeated motifs.

## Current implementation direction

This repository currently uses native Android Java so the APK can be built by GitHub Actions without requiring a local PC or Unity installation. The upgraded prototype now includes:

- First-person movement with touch joystick.
- Right-side swipe camera look.
- Sprint and stamina.
- Procedural Level 0 maze.
- Yellow first-person sleeves/hands.
- Red wall-button objective system.
- Strange exit door.
- Black shadow entity with basic hunting AI.
- Damp carpet/fluorescent ambience generated in code.
- Vignette, haze, wall stain, wallpaper detail, fluorescent glow, grain, and head bob.

## High-end future path

A true PC-comparable version should eventually become a Unity 6 LTS URP project, but that requires a Unity-capable build environment. The native Android version remains useful because it can be built entirely through GitHub Actions and installed from a phone.

Future Unity/Godot milestones:

1. Build a modular 3D Level 0 kit.
2. Add baked fluorescent lighting.
3. Add dirty wallpaper, carpet, ceiling tile, stain, and normal-map materials.
4. Add real 3D black entity model.
5. Add hazmat/yellow player model.
6. Add chunk streaming and occlusion culling.
7. Add mobile graphics presets.
8. Build signed Android APK/AAB through CI.

## No copyrighted assets

All committed assets must be original, generated, user-created, or used only as visual references. Do not commit ripped assets, paid asset-store content, copyrighted music, game models, ROMs, secrets, signing keys, or passwords.
