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

## References

- IDEA.md
- SPEC.md
- INSTRUCTIONS.md
