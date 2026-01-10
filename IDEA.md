<!-- IDEA.md -->
# 🏛️ Bizarre Museum (BR)

_A surreal mixed-reality experience for capturing dreams and curating the impossible._

**Bizarre Museum** is a work-in-progress mixed-reality project that bridges **mobile AR** and **VR**. Players collect fragments of strange dreams in the real world, then later explore and experiment with those fragments inside a surreal virtual museum.

Everything is intentionally off-kilter: gravity misbehaves, objects hum and watch back, colors bleed into space, and the museum itself feels faintly alive. The current build is a **bare-bones prototype** focused on the core loop and systems; visuals, audio, UI, and narrative framing are intentionally incomplete.

---

## Core Fantasy

You are a courier of dreams.

In waking spaces, you encounter impossible objects trapped in bubbles — remnants of someone else’s subconscious. You collect them, mix them through a bizarre machine, and receive an access code.

Later, inside VR, that code opens a private museum space where your collected artifacts come alive. Their effects overlap, interfere, and resonate, creating unique surreal environments that can be revisited or shared.

---

## High-Level Experience Flow

### 1. Dream Mode (AR – Mobile)

- Dream bubbles drift through the player’s real environment.
- Each bubble contains a surreal object fragment.
- The player freezes, collects, and stores up to **three items**.
- A strange machine appears to process the collection.
- Pulling a lever uploads the mix and produces an **access code**.

### 2. Museum Mode (VR)

- The player enters a surreal museum room with **three pedestals**.
- An access code is entered at a terminal.
- The corresponding items are spawned.
- Items can be placed on pedestals, activated, and rearranged.
- Active items emit localized effects (“auras”) that interact with each other.
- The player experiments, observes, and inhabits the resulting surreal space.

---

## Museum Items (Current Design Set)

Each item has a **localized aura** (short range) and parameterized behavior. Effects are subtle, layered, and meant to interact.

### 🌀 Item A — _Gravity Anomaly_

Creates an area where gravity becomes unpredictable.

**Parameters**

- Force magnitude (0.0–1.0)
- Randomized force direction
- Aura radius: ~3m

---

### 🎶 Item B — _Humming Relic_

Emits a low, unsettling hum that intensifies as the player approaches.

**Parameters**

- Base volume (0.0–1.0)
- Pitch variation
- Distance attenuation model
- Max distance: ~5m

---

### 🎨 Item C — _Chromatic Shifter_

Cycles colors and textures on itself and nearby surfaces using lighting and shaders.

**Parameters**

- Color cycle speed (0.0–1.0)
- Color palette selection
- Texture / noise patterns
- Aura radius: ~2m

---

### 👁️ Item D — _Gaze Distorter_

Warps nearby space when stared at directly. The longer the gaze, the stronger the effect.

**Parameters**

- Distortion strength (0.0–1.0)
- Time to max distortion
- Decay time after gaze ends
- Aura radius: ~4m

---

### ✨ Item E — _Particle Emitter_

Releases floating particles that react to player movement.

**Parameters**

- Emission rate
- Particle size
- Particle color & shape
- Particle speed
- Player interaction strength
- Aura radius: ~3m

---

## Pedestal Interaction Rules

- The museum room contains **three pedestals** (Left, Center, Right).
- Each pedestal can hold one item.
- Each item affects:
  - Its own pedestal area
  - Adjacent pedestals only

- Interactions are intentionally constrained to encourage experimentation.
- Each pedestal has a toggle button to activate/deactivate the item.

---

## AR Interaction Details

- Dream bubbles float slowly around the player.
- Tapping a bubble freezes or bursts it.
- The object falls into the space.
- Walking through the object collects it (with haptics feedback).
- A sticky HUD shows the player’s inventory (max 3 items).
- Items can be dragged out of the inventory and dropped back into the world.
- Once full, a bizarre upload machine appears:
  - Three item slots
  - A lever labeled **“Create Concoction”**
  - Pulling the lever uploads the mix and generates an access code.

---

## VR Interaction Details

- Sticky HUD mirrors the AR inventory UI.
- Items can be picked up, stored, and placed on pedestals.
- Environmental reactions include:
  - Subtle pulsing
  - Scaling
  - Audio shifts
  - Physics changes

- The space reacts to item combinations, not just individual items.
- Codes can be reused to regenerate the same setup (seed-based).

---

## Tone & Narrative Direction

- **Playful surrealism with unease**
- Objects feel reactive, almost sentient.
- The museum is not neutral — it observes.
- Long-term narrative direction:
  - A dream curator
  - A whispering PA system or wandering docent
  - A growing archive of impossible artifacts
  - Hints of a larger dream diary being reconstructed

---

## Known Gaps (Intentional)

- Placeholder art, materials, shaders, and audio
- Minimal onboarding and UI polish
- Limited fragment variety
- No comfort or accessibility settings yet
- Performance not tuned for production hardware

This is a **systems-first prototype**.

---

## Future Opportunities

- Curated rooms that morph based on dominant item traits
- Social dream codes and remixing
- Spatial audio beds and fog volumes
- Bespoke shader-driven visual identity
- Expanded interaction (breakable props, time skips, gravity pockets)
- Light narrative progression tied to repeated visits

---

## Quick Start (Prototype)

**AR**

1. Open the AR scene on a mobile device.
2. Freeze and collect three dream bubbles.
3. Upload the collection and note the access code.

**VR**

1. Open the VR scene.
2. Enter the access code at the terminal.
3. Place items on pedestals and explore the effects.

## Detailed User Flow

- The player is on their Android phone and opens the app.
- The main menu shows the app's title and logo and branding in general, and two buttons: Dream Mode (AR) and Museum Mode (VR).
- The player taps on Dream Mode to enter the AR experience.
- The player sees the AR view of their surroundings, with dream bubbles floating slowly around them. Each bubble contains inside it one of the five museum items listed above, and a hint of what effect the item has when placed in the museum (color, light, sound, movement pattern, etc).
- The player taps on a bubble to burst it, causing the object inside to fall to the ground.
- When the player is close enough to the object, they can walk through it to collect it, which adds it to their inventory shown in a sticky HUD UI at the bottom of the screen. The player can collect up to three objects before their inventory is full.
- The player can drag items out of their inventory to remove them if they want to make space for new items. The items will fall back to the ground when removed from inventory.
- Once the player has collected three objects, a bizarre machine appears in front of them with three slots for the items and a lever to "upload" their collection labelled "create concoction".
- The player drags and drops their collected items into the slots on the machine.
- When 3 items are in the slots, the lever on the machine lights up, indicating that the player can slide it down to upload their collection.
- The player slides the lever down, causing the machine to hum and whir as it processes the items. After a few seconds, the machine spits out a slip of paper with an access code on it.
- The player notes down the access code and exits the AR experience using a button in the HUD UI.
- The player returns to the main menu and taps on Museum Mode to enter the VR experience.
- The player puts on their VR headset and sees the VR view of a surreal museum room with three empty pedestals.
- The player approaches a terminal in the room and inputs the access code they received from the AR experience.
- Upon entering the code, the machine spits out three items corresponding to the ones collected in AR, placing them on the ground.
- The player picks up the items and places them in their inventory shown in a sticky HUD UI at the bottom of the screen (same UI/UX as AR).
- The player can drag and drop an item from their inventory onto a pedestal to place it there.
- Each pedestal has a push button that toggles weather the item is active or not. When active, the item's effect is applied to the surrounding area (aura) and interacts with other active items on nearby pedestals.
- The player can experiment with different combinations of items and their placements to create unique surreal effects in the museum room.
- The player can remove items from pedestals back into their inventory by dragging and dropping them.
- The player can exit the VR experience using a button in the HUD UI, returning to the main menu.
- Visiting the AR scene just starts it anew with new bubbles to collect, while visiting the VR scene again allows the player to input a valid access code to load a new set of items into the museum.
- The backend server stores the mapping of access codes to item combinations, allowing players to revisit their previous collections by entering the same code again in VR (seeds) so they can re-experience their surreal museum with the same items and potentially share the same seed with others for those crazy combinations (thank you RNGesus).

## References

- SPEC.md
- STATUS.md
- INSTRUCTIONS.md