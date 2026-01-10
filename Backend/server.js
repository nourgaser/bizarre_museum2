require("dotenv").config();
const express = require("express");
const cors = require("cors");
const mongoose = require("mongoose");
const Concoction = require("./models/Concoction");

const app = express();
const PORT = process.env.PORT || 3000;
const MONGO_URI =
  process.env.MONGO_URI || "mongodb://localhost:27017/bizarre_museum";

app.use(cors());
app.use(express.json());

// Constants for code generation
const CODE_ALPHABET = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789"; // skip ambiguous chars
const CODE_LENGTH = 6;
const MAX_CODE_ATTEMPTS = 10;

const generateCode = () => {
  let code = "";
  for (let i = 0; i < CODE_LENGTH; i += 1) {
    const index = Math.floor(Math.random() * CODE_ALPHABET.length);
    code += CODE_ALPHABET[index];
  }
  return code;
};

const generateSeeds = (items) =>
  items.map((slug) => ({ slug, seed: Math.random() }));

const findUniqueCode = async () => {
  for (let attempt = 0; attempt < MAX_CODE_ATTEMPTS; attempt += 1) {
    const candidate = generateCode();
    // Early exit if code is unused
    const exists = await Concoction.exists({ code: candidate });
    if (!exists) {
      return candidate;
    }
  }
  throw new Error("Unable to allocate unique code after multiple attempts");
};

// --- ROUTES ---

app.get("/api/health", (_req, res) => {
  res.json({ ok: true, service: "bizarre-museum-backend" });
});

// POST /api/concoctions -> create new concoction and return code
app.post("/api/concoctions", async (req, res) => {
  const { items } = req.body || {};

  if (!Array.isArray(items) || items.length === 0) {
    return res.status(400).json({ error: "items array is required" });
  }

  if (items.length > 3) {
    return res
      .status(400)
      .json({ error: "max 3 items supported in this prototype" });
  }

  const normalizedItems = items
    .map((slug) => (typeof slug === "string" ? slug.trim() : ""))
    .filter((slug) => slug.length > 0);

  if (normalizedItems.length === 0) {
    return res.status(400).json({ error: "items must be non-empty strings" });
  }

  try {
    const code = await findUniqueCode();
    const concoction = new Concoction({ code, items: generateSeeds(normalizedItems) });
    await concoction.save();

    res.status(201).json({ success: true, code });
  } catch (error) {
    console.error("Failed to create concoction", error);
    res.status(500).json({ error: "Failed to create concoction" });
  }
});

// GET /api/concoctions/:code -> retrieve concoction by code
app.get("/api/concoctions/:code", async (req, res) => {
  const code = (req.params.code || "").trim().toUpperCase();

  if (code.length !== CODE_LENGTH) {
    return res.status(400).json({ error: "code must be 6 characters" });
  }

  try {
    const concoction = await Concoction.findOne({ code });
    if (!concoction) {
      return res.status(404).json({ error: "Concoction not found" });
    }

    return res.json({
      code: concoction.code,
      createdAt: concoction.createdAt,
      items: concoction.items,
    });
  } catch (error) {
    console.error(`Failed to fetch concoction ${code}`, error);
    res.status(500).json({ error: "Failed to fetch concoction" });
  }
});

// GET /api/concoctions -> list recent concoctions (debug)
app.get("/api/concoctions", async (_req, res) => {
  try {
    const results = await Concoction.find().sort({ createdAt: -1 }).limit(50);
    res.json(results);
  } catch (error) {
    console.error("Failed to list concoctions", error);
    res.status(500).json({ error: "Failed to list concoctions" });
  }
});

// --- SERVER BOOTSTRAP ---

const start = async () => {
  try {
    await mongoose.connect(MONGO_URI);
    console.log("Connected to MongoDB");
    app.listen(PORT, "0.0.0.0", () => {
      console.log(`Bizarre Museum backend running on port ${PORT}`);
    });
  } catch (error) {
    console.error("Failed to start server", error);
    process.exit(1);
  }
};

start();
