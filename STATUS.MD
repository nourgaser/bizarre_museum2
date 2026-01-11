<!-- STATUS.md -->

# 🏛️ PROJECT STATUS: Bizarre Museum

**Session Goal:** 8-Hour Prototype | **Time Remaining:** ~8 Hours

## 📌 Current Context & Narrative

We are building a "Bizarre Museum" MR experience. Players collect dream fragments in AR (Mobile) and curate them in VR.
**Current Focus:** INITIALIZATION. We need to set up the Backend API to handle "Concoctions" (combinations of items).

## 📋 Active To-Do List

### Phase 1: Backend & Connectivity (Hour 0 - 1.5)

- [x] **[Backend]** Initialize Express/Mongoose repo.
- [x] **[Backend]** Create `Item` and `Concoction` Schemas.
- [x] **[Backend]** Implement endpoints: `POST /concoctions`, `GET /concoctions/:code`.
- [x] **[Unity]** Create URP Project (User Action).
- [x] **[Unity]** specific `APIClient.cs` to verify connection (Ping test) — client + tester scripts added; backend round-trip pending Play Mode verification.

### Phase 2: Data & Core Systems (Hour 1.5 - 3)

- [ ] Create `ItemData` ScriptableObjects.
- [ ] Create Primitive Prefabs (A-E).
- [ ] Setup UI Toolkit Base Theme (USS).

### Phase 3: AR Loop (Collector)

- [ ] AR Camera Mock (Editor Movement).
- [ ] Bubble Spawning System.
- [ ] Interaction (Raycast/Tap).
- [ ] Inventory HUD (UI Toolkit).

### Phase 4: VR Loop (Curator)

- [ ] VR Room Setup (Pedestals).
- [ ] Login UI (UI Toolkit).
- [ ] Item Spawning logic (Seed-based).
- [ ] Item Effects (Gravity, Hum, Color).

### Phase 5: Polish

- [ ] Watermark & Styling.
- [ ] Build & Test.

## 🪵 Session Log

- Unity 3D-URP project created; backend API client and tester scripts added for ping/create/get verification.
- Main Menu UI Toolkit layout and styling added with scene navigation controller.
- Fixed main menu USS to comply with UI Toolkit (border/transition shorthands removed).
- AR HUD UI Toolkit skeleton added (watermark, network indicator, inventory slots, upload button) with controller stub.
- AR GameManager stub added: HUD wiring, backend ping, debug collection (keys 1-3), and upload via ApiClient.
- AR GameManager debug input updated to support new Input System key polling.
- AR bubble prefab scripts added (FloatingBob for drift, ItemBubble for pop-and-drop behavior with hint color).
- Item system scaffolding added: ScriptableObject definitions, seeded item interface, and per-item behavior stubs (gravity, hum, chromatic, gaze, particles) for InnerItem prefabs.
- Item behaviors refined: gravity now pushes in random direction with layer mask; hum volume attenuates by camera distance; gaze uses URP Lens Distortion and only triggers when looked at, centered on item.
- AR bubble spawner scaffold added (maintains target bubble count around camera with item definitions).
- AR bubble spawner now spawns over time (interval-based), cleans popped bubbles, and gives visual/audio spawn feedback; bubbles accept definitions via SetDefinition.
- Interaction loop added: click/tap raycasts pop bubbles, adds slot with slug+seed to HUD/inventory.
- UX polish: aiming highlight shows in-range bubble (tint + subtle scale); popped items fall with physics, rest on a virtual/real floor via raycast, and are collected only when the player approaches.
- HUD polish: displays generated code, enables Go-to-VR button after upload, and styles code card for visibility.
- HUD responsive tweak: wraps inventory/actions for mobile, shrinks padding, and hides code card until a code is generated.
- VR scene shell assembled (wall segments, pedestals with lid/button/slot, terminal placeholder, XR Device Simulator added); Terminal UI Toolkit scaffold added (code entry/status, themed USS).

## References

- IDEA.md
- SPEC.md
- INSTRUCTIONS.md
