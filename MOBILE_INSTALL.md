# Mobile install guide

This project is prepared for portrait iPhone and Android play.

## What changed for iPhone/mobile

- Portrait-first layout at 1080x1920.
- UI uses the phone safe area so iPhone notches and the home indicator do not cover the HUD.
- Real touch input was added for balloon taps.
- Pause/HUD/menu/game-over UI scales for phone screens.
- Android and iOS bundle id: `com.gbhide.balloonblast`.

## Quick iPhone preview in Unity

1. Open the project in Unity `2022.3.17f1`.
2. Open `Assets/Scenes/Menu.unity` or `Assets/Scenes/Game.unity`.
3. In the Game view, choose a portrait phone aspect such as `9:16`, `1080x1920`, or add an iPhone size like `390x844`.
4. Press Play. The menu should show `MOBILE READY: iPhone safe area + Android touch controls`.

## Install on Android phone

1. Install Unity Hub with Unity `2022.3.17f1`.
2. Add modules: **Android Build Support**, **Android SDK & NDK Tools**, and **OpenJDK**.
3. On the Android phone, enable **Developer options** and **USB debugging**.
4. Connect the phone by USB.
5. In Unity, open this project.
6. Choose `Balloon Blast > Mobile > Build Android APK`.
7. Install the generated file:
   - Unity creates `Builds/Android/BalloonBlast.apk`.
   - You can also install manually with `adb install -r Builds/Android/BalloonBlast.apk`.

## Install on iPhone

iPhone installs require a Mac with Xcode and Apple signing. Unity on Windows/Linux cannot directly install to iPhone.

1. On a Mac, install Unity `2022.3.17f1` and Xcode.
2. In Unity Hub, add **iOS Build Support** to Unity.
3. Open this project in Unity.
4. Choose `Balloon Blast > Mobile > Build iOS Xcode Project`.
5. Open the generated Xcode project in `Builds/iOS`.
6. In Xcode:
   - Select your Apple Team under **Signing & Capabilities**.
   - Connect your iPhone by USB.
   - Select the iPhone as the run device.
   - Press **Run**.

## Command line builds

Android:

```bash
Unity -batchmode -quit -projectPath . -executeMethod MobileBuild.BuildAndroidApkCommandLine
```

iOS Xcode export:

```bash
Unity -batchmode -quit -projectPath . -executeMethod MobileBuild.BuildIosXcodeProjectCommandLine
```
