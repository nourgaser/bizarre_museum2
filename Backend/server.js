require("dotenv").config();
const express = require("express");
const cors = require("cors");
const bodyParser = require("body-parser");
const mongoose = require("mongoose");
const Dream = require("./models/Dream");

const app = express();
const PORT = 3000;

app.use(cors());
app.use(bodyParser.json());

// Helper: Generate random float between min and max
const randomFloat = (min, max) => Math.random() * (max - min) + min;

// Helper: Generate a random hex color
const randomColor = () => {
  const letters = "0123456789ABCDEF";
  let color = "#";
  for (let i = 0; i < 6; i++) {
    color += letters[Math.floor(Math.random() * 16)];
  }
  return color;
};

// --- ENDPOINTS ---

// 1. AR APP CALLS THIS: Upload collected items to mix a dream
app.post("/api/mix", async (req, res) => {
  const { items } = req.body; // Expects array of strings e.g. ["mug", "cube"]

  // Generate a 3-digit code (e.g., "808")
  const code = Math.floor(100 + Math.random() * 900).toString();

  // THE ALCHEMIST LOGIC (RNG based on input)
  // We generate weird physics parameters for the VR scene
  const dreamConfig = {
    code: code,
    items: items || [],
    parameters: {
      gravityY: randomFloat(-2.0, 1.0), // Low or negative gravity
      timeScale: randomFloat(0.5, 1.5), // Slow motion or fast forward
      fogDensity: randomFloat(0.0, 0.05),
      skyColor: randomColor(),
      distortionLevel: randomFloat(0.0, 1.0),
    },
  };

  try {
    const newDream = new Dream(dreamConfig);
    await newDream.save();
    console.log(
      `[ALCHEMIST] Mixed Dream ${code} and saved to MongoDB:`,
      dreamConfig.parameters,
    );
    res.json({ code: code });
  } catch (err) {
    console.error("Error saving dream:", err);
    res.status(500).json({ error: "Failed to save dream" });
  }
});

// 2. VR APP CALLS THIS: Retrieve the dream parameters
app.get("/api/dream/:code", async (req, res) => {
  const code = req.params.code;

  try {
    const config = await Dream.findOne({ code: code });
    if (config) {
      console.log(
        `[ALCHEMIST] Serving Dream ${code} from MongoDB to VR Client`,
      );
      res.json(config);
    } else {
      res.status(404).json({ error: "Dream not found. Did you wake up?" });
    }
  } catch (err) {
    console.error("Error fetching dream:", err);
    res.status(500).json({ error: "Failed to fetch dream" });
  }
});

// 3. DEBUG: Check all dreams
app.get("/api/debug", async (req, res) => {
  try {
    const dreams = await Dream.find();
    res.json(dreams);
  } catch (err) {
    res.status(500).json({ error: "Failed to fetch debug data" });
  }
});

mongoose
  .connect(process.env.MONGO_URI)
  .then(() => {
    console.log("Connected to MongoDB");
    app.listen(PORT, "0.0.0.0", () => {
      console.log(`The Somnarium Backend is running on port ${PORT}`);
    });
  })
  .catch((err) => console.error("Could not connect to MongoDB", err));
