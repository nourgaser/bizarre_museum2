<!-- STATUS.md -->
# 🏛️ PROJECT STATUS: Bizarre Museum

**Session Goal:** 8-Hour Prototype | **Time Remaining:** ~8 Hours

## 📌 Current Context & Narrative

We are building a "Bizarre Museum" MR experience. Players collect dream fragments in AR (Mobile) and curate them in VR.
**Current Focus:** INITIALIZATION. We need to set up the Backend API to handle "Concoctions" (combinations of items).

## 📋 Active To-Do List

### Phase 1: Backend & Connectivity (Hour 0 - 1.5)

- [ ] **[Backend]** Initialize Express/Mongoose repo.
- [ ] **[Backend]** Create `Item` and `Concoction` Schemas.
- [ ] **[Backend]** Implement endpoints: `POST /concoctions`, `GET /concoctions/:code`.
- [ ] **[Unity]** Create URP Project (User Action).
- [ ] **[Unity]** specific `APIClient.cs` to verify connection (Ping test).

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

- _Session Started._

## References

- IDEA.md
- SPEC.md
- INSTRUCTIONS.md
