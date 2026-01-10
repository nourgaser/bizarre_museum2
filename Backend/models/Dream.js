const mongoose = require('mongoose');

const dreamSchema = new mongoose.Schema({
    code: { type: String, required: true, unique: true },
    items: [String],
    parameters: {
        gravityY: Number,
        timeScale: Number,
        fogDensity: Number,
        skyColor: String,
        distortionLevel: Number
    },
    createdAt: { type: Date, default: Date.now, expires: 3600 } // Optionally expire after 1 hour
});

const Dream = mongoose.model('Dream', dreamSchema);

module.exports = Dream;
