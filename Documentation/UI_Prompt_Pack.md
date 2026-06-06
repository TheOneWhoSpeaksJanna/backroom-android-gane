# Backrooms Level Zero UI Prompt Pack

This document captures the uploaded prompt pack and how it was applied to the native Android Canvas build.

## Implementation target

The pack targets a Level 0 mobile horror UI with:

- damp yellow wallpaper
- stained office carpet
- buzzing fluorescent lighting
- low-noise liminal composition
- readable Android landscape touch targets
- dark translucent panels
- dirty yellow and dark brown UI colors

The current Android build implements this direction in code so the APK still builds from GitHub Actions without external art tools.

## Applied in game

- Main menu with `ENTER LEVEL 0`, `SETTINGS`, and `EXIT`.
- Loading screen with fluorescent-tube progress bar and the text `LOADING FLUORESCENT MAZE`.
- Improved in-game HUD with objective text, stamina bar, sanity indicator, transparent joystick, `USE`, `SPRINT`, and pause buttons.
- Pause menu that stays open until `RESUME`, `SETTINGS`, `CONTROL LAYOUT`, or `EXIT GAME` is tapped.
- Settings screen with sensitivity, brightness, volume, frame cap, graphics scale, control layout, and back.
- Control layout screen with joystick, look area, `USE`, `SPRINT`, pause, opacity, size, reset, and back.
- App icon direction refreshed with a yellow wallpaper background, black doorway silhouette, fluorescent light, and carpet strip.

## Original prompt categories

### Main menu background

Cinematic Android horror menu background for `Backrooms Level Zero`: endless yellow stained Level 0 corridor, damp carpet, low ceiling tiles, buzzing fluorescent lights, repeating square pillars, subtle fog, grime, no characters, no gore, center safe area for title and buttons, 16:9, readable contrast.

### Main menu buttons

Android horror UI kit with buttons labeled `ENTER LEVEL 0`, `SETTINGS`, and `EXIT`, dirty yellow and dark brown palette, worn borders, subtle fluorescent glow, rough paper texture, readable text, large touch targets, rounded corners.

### Loading screen

Black-yellow liminal corridor with faint fluorescent buzz, damp carpet leading into darkness, centered `LOADING FLUORESCENT MAZE` text, progress bar made from flickering fluorescent tubes, Android landscape, high contrast.

### HUD

First-person Android HUD with transparent virtual joystick, `USE`, `SPRINT`, pause, objective text, stamina and sanity bars, low opacity dark panels, dirty yellow accents, large touch targets.

### Settings menu

Dark translucent settings panel with dirty yellow border, sliders for sensitivity, brightness, volume, frame cap, graphics scale, buttons for control layout and back, worn office-signage style.

### Control customization

Mobile control customization screen showing joystick, look area, `USE`, `SPRINT`, pause, opacity and size controls, grid overlay, Android landscape safe area.

### App icon

Square Android icon with dark yellow wallpaper texture, black service doorway silhouette, flickering fluorescent light, carpet pattern at bottom, strong silhouette, adaptive icon safe area.

### Texture direction

- Wallpaper: seamless old yellow wallpaper with subtle vertical pattern, stains, faded patches, damp grime.
- Carpet: seamless brown-yellow worn fibers, damp stains, flattened paths, office carpet pattern.
- Ceiling: stained office ceiling tiles with yellowing, water stains, cracks, and fluorescent grime.

## Negative prompt rules

Avoid people, gore, weapons, copyrighted logos, readable brand names, excessive blood, cartoon mascots, anime style, neon cyberpunk, overly clean UI, tiny unreadable text, extreme fisheye, clutter, phone mockups, watermarks, and signatures.

## Future asset workflow

When real generated art is added later:

1. Generate four variants per category.
2. Pick the clearest and least cluttered option.
3. Export menu/loading backgrounds at `1280x720`.
4. Export app icon at `1024x1024`, then adapt to `512x512` and `192x192`.
5. Keep the game readable on low-end Android screens.
