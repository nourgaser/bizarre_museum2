<!-- SPEC.md -->
# 🏛️ Bizarre Museum (BR) - Prototype Specification

**Version:** 0.1.1 (Client-Authority Update)
**Tech Stack:** Unity (URP), Node.js/Express/MongoDB
**Timeframe:** ~8 Hours

## 1. Project Overview

**Bizarre Museum** is a mixed-reality experience bridging mobile AR (collection) and VR (exhibition). This prototype focuses on the end-to-end data flow, system architecture, and core interaction loops using **UI Toolkit** and **Geometric Primitives**.

### Constraints & Requirements

- **Rendering:** Universal Render Pipeline (URP).
- **UI System:** **UI Toolkit (UXML/USS)** for all HUDs, menus, and overlays.
- **Assets:** Primitive shapes (Cube, Sphere, Capsule) only. No external asset hunting.
- **Testing:** **100% Editor Compatible.** logic must support mouse/keyboard simulation for AR taps and VR look/interact.
- **Branding:** Persistent "Bizarre Museum" watermark (Top-Left). Consistent HUD styling.

---

## 2. Backend Architecture (API)

**Host:** `http://bm-api.nourgaser.com/` (or `localhost:3000`)
**Stack:** ExpressJS / Mongoose / MongoDB.

### Philosophy: "The DNA Bank"

The backend is a passive state container. It does **not** know about physics, colors, or sounds. It simply assigns and stores a random `seed` (0.0 - 1.0) for each item. This seed acts as a deterministic "DNA" marker that the Unity client unpacks into complex behavior.

### Data Models

**Concoction Schema (MongoDB)**

```json
{
  "_id": "ObjectId",
  "code": "String (Unique, 6-char alphanumeric, e.g., 'A7X99')",
  "createdAt": "Date",
  "items": [
    {
      "slug": "String (e.g., 'gravity-anomaly')",
      "seed": "Number (0.0-1.0)"
    }
  ]
}
```

### Endpoints

1. **POST** `/api/concoctions`

- **Input:** `{ "items": ["slug-a", "slug-b"] }`
- **Logic:** Generates unique `code`. Assigns random `seed` to each item.
- **Response:** `{ "success": true, "code": "A7X99" }`

2. **GET** `/api/concoctions/:code`

- **Input:** `code` in URL.
- **Response:** Returns the list of items + their seeds.

3. **GET** `/api/concoctions`

- **Purpose:** Debugging/Dashboard.

---

## 3. Unity Architecture

### Directory Structure

```text
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── API/          # HTTP Client & DTOs
│   │   ├── Core/         # Managers (Game, Interaction)
│   │   ├── Items/        # ScriptableObjects & Behaviors
│   │   ├── UI/           # UI Toolkit Logic
│   │   └── Utils/        # Helpers
│   ├── ScriptableObjects/
│   │   ├── Items/        # Item Data Assets
│   └── Scenes/ ...

```

### Core Systems

1. **ItemDatabase (ScriptableObject):** Source of truth. Maps `slug` strings to Item Prefabs and static Data (Name, Icon).
2. **APIClient (Singleton):** Handles `UnityWebRequest` and deserialization.
3. **InputAdapter:** Abstracts Editor (Mouse/WASD) vs Device (Touch/XR) input.

### Item Parameterization (The "DNA Unpacker")

Every interactive item script implements a standard configuration interface.

**The Pattern:**

1. **Input:** A single float `seed` (from Server).
2. **Process:** Initialize `System.Random(seed * 100000)`.
3. **Output:** Deterministically generate all needed parameters (Force, Color, Pitch) from that specific RNG instance.

_Example:_

> Seed `0.45` -> `rng` -> NextDouble() for Color, NextDouble() for Speed.
> _Result:_ Always "Fast Green" for seed 0.45.

---

## 4. Visual Identity & UI Toolkit

### Style Guide (USS)

- **Colors:** Deep Purple (`#2A0044`) Background, Neon Cyan (`#00FFFF`) Accents.
- **Font:** Unity Default (Clean/Raw).
- **Watermark:** Top-Left, Opacity 0.5, "BIZARRE MUSEUM // PROTO [DEBUG]".

---

## 5. Implementation Roadmap (8 Hours)

### Phase 1: Backend & Connectivity (Hour 0 - 1.5)

- [x] Initialize Express/Mongoose repo.
- [x] Implement 3 Endpoints (Seed generation).
- [ ] Create Unity Project (URP).
- [ ] Write `APIClient.cs` in Unity to verify connection.

### Phase 2: Data & Core Systems (Hour 1.5 - 3)

- [ ] Create `ItemData` ScriptableObjects.
- [ ] **Implement `ItemBehavior` base class with Seed unpacking logic.**
- [ ] Create Primitive Prefabs (A-E) with specific behaviors (Gravity, Hum, etc.).
- [ ] Setup UI Toolkit Base Theme (USS).

### Phase 3: AR Loop - "The Collector" (Hour 3 - 5)

- [ ] AR Camera Mock (Editor Movement).
- [ ] Bubble Spawning System.
- [ ] Interaction (Raycast/Tap).
- [ ] Inventory HUD (UI Toolkit).
- [ ] Upload Machine UI -> `POST /api/concoctions`.

### Phase 4: VR Loop - "The Curator" (Hour 5 - 7)

- [ ] VR Room Setup (3 Pedestals).
- [ ] Login UI -> `GET /api/concoctions/:code`.
- [ ] Spawning Logic: Instantiate Prefab -> Call `Configure(seed)`.
- [ ] Interaction: Toggle buttons on pedestals.

### Phase 5: Polish & Packaging (Hour 7 - 8)

- [ ] Unify USS styles.
- [ ] Final Build/Run test.

---

## 6. Simulation Controls (Editor)

**AR Mode:**

- **WASD:** Move camera on X/Z plane.
- **Right Click + Drag:** Rotate Camera.
- **Left Click:** Interact/Collect.

**VR Mode:**

- **Alt + Mouse:** Look around (Simulate Head).
- **Left Click:** Interact with UI/Pedestals.

## References

- IDEA.md
- STATUS.md
- INSTRUCTIONS.md
