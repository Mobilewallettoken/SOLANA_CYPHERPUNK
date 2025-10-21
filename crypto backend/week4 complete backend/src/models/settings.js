const mongoose = require('mongoose');

const settingsSchema = new mongoose.Schema({
    buyRateAdjustment: {
        type: Number,
        default: 0.98,
        min: 0,
        max: 1
    },
    sellRateAdjustment: {
        type: Number,
        default: 1.02,
        min: 1
    },
    adminFee: {
        type: Number,
        default: 3,
        min: 0
    }
}, {
    timestamps: true
});

const Settings = mongoose.model('Settings', settingsSchema);

module.exports = Settings;
