const mongoose = require("mongoose");

const itemSchema = new mongoose.Schema(
  {
    slug: { type: String, required: true, trim: true },
    seed: { type: Number, required: true, min: 0, max: 1 },
  },
  { _id: false },
);

const concoctionSchema = new mongoose.Schema(
  {
    code: {
      type: String,
      required: true,
      unique: true,
      minlength: 6,
      maxlength: 6,
      uppercase: true,
      trim: true,
    },
    items: { type: [itemSchema], default: [] },
    createdAt: { type: Date, default: Date.now },
  },
  { versionKey: false },
);

module.exports = mongoose.model("Concoction", concoctionSchema);
