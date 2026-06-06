# Unity implementation guide

## 1. Import the files

1. Extract the ZIP.
2. Copy the `Assets/UI` and `Assets/Scripts` folders into your Unity project.
3. In Unity, select each UI image.
4. In the Inspector, set:
   - Texture Type: `Sprite (2D and UI)`
   - Sprite Mode: `Single`
   - Compression: `High Quality` or `None`
   - Max Size: `2048`
5. Click `Apply`.

## 2. Create the menu scene

1. Create a new scene named `MainMenu`.
2. Add a `Canvas`.
3. Set Canvas to:
   - Render Mode: `Screen Space - Overlay`
   - Canvas Scaler: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`
   - Match: `0.5`
4. Add an `Image` object under the Canvas.
5. Rename it to `ScreenBackground`.
6. Stretch it to fill the whole canvas:
   - Anchor Min: `(0, 0)`
   - Anchor Max: `(1, 1)`
   - Left, Right, Top, Bottom: `0`
7. Assign `01_main_menu_clean.png` as the Source Image.

## 3. Add invisible clickable buttons

Because the image already includes the visible menu design, create transparent buttons over the text areas.

On the main menu:
- Add Button over `PLAY`
- Add Button over `SETTINGS`
- Add Button over `CREDITS`
- Add Button over `FEEDBACK`

For each button:
1. Delete or hide the button text.
2. Set the Button Image color alpha to around `0`.
3. Keep the button object in the correct clickable position.
4. Connect the button `OnClick()` to the `DesolationMenuController` script.

## 4. Add the menu controller script

Create an empty GameObject named `MenuController`.

Attach `DesolationMenuController.cs`.

In the Inspector:
- Drag the background Image into `screenBackground`
- Assign the sprites:
  - `01_main_menu_clean.png`
  - `02_save_slots_clean.png`
  - `03_settings_clean.png`
  - `04_credits_clean.png`
  - `05_feedback_clean.png`
- Drag your UI button groups into the matching group slots.

Recommended groups:
- `mainButtonsGroup`
- `saveButtonsGroup`
- `settingsButtonsGroup`
- `creditsButtonsGroup`
- `feedbackButtonsGroup`

This lets the script swap the full-screen image and activate the right invisible buttons.

## 5. Save slot buttons

On the Saves screen, place invisible buttons over:
- Save 1 card
- Save 2 card
- Save 3 card
- Back button

Connect save buttons to:
- `StartGameFromSave1()`
- `StartGameFromSave2()`
- `StartGameFromSave3()`

For now, all 3 can load the same Level 0 scene.

## 6. Settings sliders

Use real Unity sliders over the slider areas.

Recommended PlayerPrefs keys:
- `MasterVolume`
- `MusicVolume`
- `SfxVolume`
- `Brightness`
- `Sensitivity`
- `GraphicsQuality`

You can later connect these to your AudioMixer, brightness post-processing, camera look sensitivity, and quality settings.

## 7. Feedback UI

Use TextMeshPro input fields over the feedback boxes:
- `FeedbackInput`
- `EmailInput`
- `IssueTypeDropdown`
- `SendButton`

Attach `DesolationFeedbackController.cs` and connect the fields in the Inspector.

The starter script saves feedback locally with PlayerPrefs and prints it in the console. Later you can replace that part with a web request, Discord webhook, Firebase, or your own server.

## 8. Set the Android app icon

1. Go to `Edit > Project Settings > Player`.
2. Select the Android tab.
3. Open `Icon`.
4. Drag `06_app_icon_1024.png` or `06_app_icon_512.png` into the icon slots.
5. For adaptive icons, you can use the same icon as foreground while using a black or dark-gold background color.

## 9. Add scenes to Build Settings

Go to `File > Build Settings`.

Add these scenes:
1. `MainMenu`
2. Your first gameplay scene, for example `Level0`

Your menu script can load the gameplay scene using:

```csharp
SceneManager.LoadScene("Level0");
```

## 10. Recommended Unity hierarchy

```text
MainMenu
└── Canvas
    ├── ScreenBackground
    ├── MainButtonsGroup
    │   ├── PlayButton
    │   ├── SettingsButton
    │   ├── CreditsButton
    │   └── FeedbackButton
    ├── SaveButtonsGroup
    │   ├── Save1Button
    │   ├── Save2Button
    │   ├── Save3Button
    │   └── BackButton
    ├── SettingsButtonsGroup
    │   ├── MasterVolumeSlider
    │   ├── MusicVolumeSlider
    │   ├── SfxVolumeSlider
    │   ├── BrightnessSlider
    │   ├── SensitivitySlider
    │   ├── LowButton
    │   ├── MediumButton
    │   ├── HighButton
    │   └── BackButton
    ├── CreditsButtonsGroup
    │   └── BackButton
    └── FeedbackButtonsGroup
        ├── FeedbackInput
        ├── EmailInput
        ├── IssueTypeDropdown
        ├── SendButton
        └── BackButton
```

## 11. Best quality tip

Use these images as backgrounds first. Later, recreate each UI piece using Unity UI panels, TextMeshPro, buttons, sliders, and masks. That will make the menu scale better across different phone screen sizes.
