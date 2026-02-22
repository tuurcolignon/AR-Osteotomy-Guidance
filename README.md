# AR-Osteotomy-Guidance

**Active Multimodal Surgical Guidance for Freehand Bone Cutting in Augmented Reality**

This repository contains the Unity/C# implementation for a HoloLens 2 surgical guidance system. The system replaces traditional 3D-printed cutting guides with an active, multimodal Augmented Reality (AR) feedback loop designed to reduce surgical cutting deviation and mitigate visual occlusion.

Developed as part of a Master's Thesis at KU Leuven.

## Core Architecture

- **Platform:** Microsoft HoloLens 2.
- **Engine:** Unity with Mixed Reality Toolkit (MRTK).
- **Computer Vision:** Vuforia Engine.
- **External Actuation:** ESP32 MCU over Wi-Fi UDP.

## Tracking Strategy

- **Patient Tracking:** Markerless tracking of complex mandibular geometry using a Vuforia Model Target.
- **Instrument Tracking:** Calibration-free workflow using a Vuforia Image Target attached to the surgical saw, yielding ~1mm accuracy.

## Multimodal Feedback System

To combat the cognitive fatigue and depth perception issues caused by passive AR overlays, the system splits guidance across three sensory channels:

### 1. Visual Guidance
- **Adaptive Box Corridor:** Feedback system (Green = inside safe zone; Red = outside safe zone).
- **Directional Arrows:** 3D UI elements attached to the tool pointing to the target direction.
- **Cross-Section Monitor:** External 2D slice view to minimize occlusion and maximize depth perception.

### 2. Auditory Guidance
"Eyes-free" navigation utilizing HoloLens 2 spatial audio:
- **Tempo:** Indicates distance to the target.
- **Pitch:** Indicates angular deviation.
- **Amplitude:** Used for critical boundary alerts.

### 3. Haptic Guidance
- **Hardware:** Custom wireless wristband powered by an ESP32.
- **Mechanism:** 8 vibration motors triggered to communicate somatic "stop" signals and rotational limits.

## Authors & Acknowledgments

- **Researchers:** Tuur Colignon, Lowie Declercq.
- **Promotor:** Carlos Rodriguez-Guerrero.
- **Co-promotor:** Ewald Ury.
- **Institution:** KU Leuven.
