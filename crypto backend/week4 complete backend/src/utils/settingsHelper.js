const Settings = require('../models/settings');

/**
 * Get settings with default values if none exist
 * @returns {Promise<Object>} Settings object with buyRateAdjustment and sellRateAdjustment
 */
async function getSettings() {
    try {
        let settings = await Settings.findOne();
        
        // If no settings exist, create default settings
        if (!settings) {
            settings = new Settings({
                buyRateAdjustment: 0.98,
                sellRateAdjustment: 1.02,
                adminFee: 3
            });
            await settings.save();
        }
        
        return settings;
    } catch (error) {
        console.error('Error fetching settings:', error);
        // Return default values if database error occurs
        return {
            buyRateAdjustment: 0.98,
            sellRateAdjustment: 1.02,
            adminFee: 3
        };
    }
}

module.exports = { getSettings };