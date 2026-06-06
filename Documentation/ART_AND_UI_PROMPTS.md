# Backrooms Level Zero Art and UI Prompt Guide

A downloadable prompt ZIP was generated for main-menu art, loading-screen art, app icon concepts, HUD/mobile controls, settings UI, and texture direction.

## Core visual direction

- Level 0 liminal horror.
- Yellow stained wallpaper.
- Damp brown/yellow office carpet.
- Low office ceiling tiles.
- Flickering fluorescent lighting.
- Minimal UI, large Android touch targets, high readability.
- No gore, no logos, no copyrighted brand marks.

## Screens to generate

1. Main menu background at 1280x720.
2. Loading screen at 1280x720.
3. App icon at 1024x1024 and 512x512.
4. Button/UI kit with normal/pressed/disabled/focused states.
5. Mobile HUD with joystick, USE, SPRINT, pause, objective, stamina, sanity.
6. Settings menu with sensitivity, brightness, volume, frame cap, graphics scale, and control layout.

## Integration notes

Generated UI art should be added under:

```text
godot/assets/ui/
```

Recommended filenames:

```text
main_menu_background.png
loading_background.png
button_normal.png
button_pressed.png
button_disabled.png
settings_panel.png
app_icon_1024.png
```

For Android icon replacement, also export:

```text
godot/icon192.png
godot/icon.svg
app/src/main/res/drawable/ic_launcher_foreground.xml
```

Keep all mobile UI elements readable at 1280x720 and 1920x1080.
