# Geography3D

Geography3D is an interactive virtual-reality quiz that challenges players to identify every country on a fully 3D globe. The experience is built with Unity and targets Meta Quest hardware using the Oculus Integration package.

## Demonstration
- [Google Drive Video Demo](https://drive.google.com/file/d/10Mq8T8x0_TQQ8q4QhuDiDzh6qXnjphGc/view)

## Features
- Interactive globe with 173 playable countries sourced from real GeoJSON data.
- Visual feedback for each guess: blue while hovering, green for correct answers, red flashes for incorrect attempts.
- Sequential quiz flow managed in VR using on-headset prompts and controller input.
- Modular Unity scripts (`Assets/Scripts`) to manage globe rendering, user interaction, and data loading.
- Ready-to-build Unity project configured for Meta Quest / Android deployment.

## Requirements
- Unity `2022.3.17f1` (LTS) or later.
- Meta Quest headset with Oculus Integration installed in the project.
- Optional: Android SDK/NDK and Meta Quest developer tools for deploying builds to hardware.

## Getting Started
1. **Clone or download** the repository into your local workspace.
2. **Open with Unity Hub**, selecting Unity `2022.3.17f1` when prompted.
3. **Install required packages** when Unity requests them (Oculus XR Plugin, TextMeshPro, Newtonsoft JSON, etc.).
4. **Switch Build Target** to Android (`File > Build Settings`) and ensure Oculus/Quest support is enabled under `Project Settings > XR Plug-in Management`.

To deploy to a Quest headset:
1. Enable developer mode on the headset and connect it to your machine.
2. In Unity, open `File > Build Settings`, select `Android`, and click `Build & Run`.
3. The resulting APK can also be sideloaded using Meta Quest Developer Hub or `adb`.

## Gameplay Overview
- The HUD lists a single country name at a time. Use the controllers to highlight and select the country on the globe.
- Correct answers advance the quiz; incorrect picks briefly flash red so you can try again.
- After all countries are found, the HUD displays a completion message and prompts you to reset.

## Controls
- **Hover country:** Point your gaze at the country (Vision input).
- **Select country:** Right-hand controller, `A` button.
- **Restart quiz:** Left-hand controller, `X` button.
- **Rotate globe:** Left and right joystick inputs.

## Assets & Data
- GeoJSON data: `Assets/Data/countries.json` (173 countries). Loaded at runtime via `LoadGeoData.cs` with Newtonsoft JSON.
- Interaction & gameplay scripts: located in `Assets/Scripts` (e.g., `GameManager.cs`, `CountryInteract.cs`, `CameraOrbit.cs`).
- Oculus configuration assets: see `Assets/Oculus` and `Assets/Resources`.

To replace or extend the dataset, place a new GeoJSON file in `Assets/Data`, update the `TextAsset` reference on the `LoadGeoData` component, and ensure the schema matches the existing `GeoJson` classes.

## Acknowledgements
- Unity scripts in `Assets/Scripts`.
- [Sebastian Lague – Geographical Adventures](https://github.com/SebLague/Geographical-Adventures/tree/main)
- [world.geo.json dataset](https://github.com/johan/world.geo.json/blob/master/countries.geo.json)
- [StackOverflow: mapping lat/long to a sphere in Unity](https://stackoverflow.com/questions/47658708/longitude-and-latitude-to-location-on-sphere-in-unity)
