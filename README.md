# Bizarre Museum: Mixed Reality Dream Curator

University project for **Extended Realities (Winter 2025/2026)** at **German International University (GIU)**.

**Authors:** Nouraldin Gaser Yousri Algendi, Debaj Shady Elmaghraby

## Concept
Bizarre Museum bridges Mobile AR and VR. Players collect “dream fragments” in AR, then curate and experiment with them inside a surreal VR museum. A shared seed-based data model keeps behaviors deterministic across devices with minimal payloads.

## Core Loop
- **Collector (AR):** Find and tap floating “dream bubbles,” pop them, and pick up dropped primitives to add to inventory.
- **Curator (VR):** Enter the AR-generated access code at a terminal, spawn the same items, place them on pedestals, and toggle their seeded effects.

## Architecture
- **Seed/DNA model:** Server issues a normalized float `seed` per item; Unity clients unpack it into deterministic parameters (gravity direction/magnitude, color gradients, audio pitch/modulation, particle patterns).
- **Backend:** Node.js + Express + MongoDB; passive state container issuing seeds and retrieving concoctions by code.
- **Client:** Unity 6 LTS + URP + XR Interaction Toolkit + UI Toolkit. Input abstraction supports mouse/keyboard in Editor and XR later.

## Interaction Design
- **AR HUD (UI Toolkit):** Flex inventory, network status, upload machine prompt when enough items are collected.
- **Bubbles & Pickup:** Raycast taps to pop bubbles; proximity pickup adds items to inventory.
- **VR Terminal:** UI Toolkit keypad/text input for the 6-character code; fetches concoction from backend.
- **Pedestals:** XR sockets accept items; pedestal button toggles seeded effects (gravity anomaly, chromatic shifter, humming relic, gaze distorter, particle emitter). Floating bob animation remains active while socketed.

## Implementation Highlights
- **Deterministic unpacker:** `System.Random` seeded from server float → reproducible parameters across AR/VR and devices.
- **InputAdapter:** Switches between Editor (mouse/WASD) and device/XR profiles without code changes.
- **Primitives-only constraint:** All visuals use cubes/spheres/capsules to emphasize behavior over assets.

## Quick Start
1) **Backend:** `cd Backend && npm install && npm run dev` (Express + MongoDB). Ensure Mongo is running.
2) **Unity:** Open project in Unity 6 (URP). Scene includes AR/VR loops, terminal, and pedestals. Use XR Device Simulator for Editor testing.
3) **Flow:** Run backend → Enter Play Mode → In AR flow, collect bubbles until a code is generated → Switch to VR scene, enter code at terminal, place spawned items on pedestals, and toggle effects.

## Future Work
- Replace primitives with scanned assets; add spatial audio layers for the humming relic.
- Expand bubble spawning variety and UX polish; strengthen networking robustness.
