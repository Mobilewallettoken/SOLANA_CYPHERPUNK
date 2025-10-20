const express = require('express');
const router = express.Router();
const Settings = require('../models/settings');
const { getSettings } = require('../utils/settingsHelper');

/**
 * @swagger
 * /settings:
 *   get:
 *     summary: Get conversion rate settings
 *     tags: [Settings]
 *     responses:
 *       200:
 *         description: Settings retrieved successfully
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 success:
 *                   type: boolean
 *                 data:
 *                   type: object
 *                   properties:
 *                     buyRateAdjustment:
 *                       type: number
 *                     sellRateAdjustment:
 *                       type: number
 *                     adminFee:
 *                       type: number
 *       500:
 *         description: Server error
 */
router.get('/', async (req, res) => {
    try {
        const settings = await getSettings();
        
        res.json({
            success: true,
            data: {
                buyRateAdjustment: settings.buyRateAdjustment,
                sellRateAdjustment: settings.sellRateAdjustment,
                adminFee: settings.adminFee
            }
        });
    } catch (error) {
        console.error('Error fetching settings:', error);
        res.status(500).json({
            success: false,
            message: 'Error fetching settings',
            error: error.message
        });
    }
});

/**
 * @swagger
 * /settings:
 *   put:
 *     summary: Update conversion rate settings
 *     tags: [Settings]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               buyRateAdjustment:
 *                 type: number
 *                 minimum: 0
 *                 maximum: 1
 *                 description: Rate adjustment for buy operations (should be <= 1.0)
 *               sellRateAdjustment:
 *                 type: number
 *                 minimum: 1
 *                 description: Rate adjustment for sell operations (should be >= 1.0)
 *               adminFee:
 *                 type: number
 *                 minimum: 0
 *                 description: Admin fee to add/subtract from quote calculations
 *     responses:
 *       200:
 *         description: Settings updated successfully
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 success:
 *                   type: boolean
 *                 message:
 *                   type: string
 *                 data:
 *                   type: object
 *                   properties:
 *                     buyRateAdjustment:
 *                       type: number
 *                     sellRateAdjustment:
 *                       type: number
 *                     adminFee:
 *                       type: number
 *       400:
 *         description: Invalid input
 *       500:
 *         description: Server error
 */
router.put('/', async (req, res) => {
    try {
        const { buyRateAdjustment, sellRateAdjustment, adminFee } = req.body;
        
        // Validation
        if (buyRateAdjustment !== undefined && (buyRateAdjustment < 0 || buyRateAdjustment > 1)) {
            return res.status(400).json({
                success: false,
                message: 'Buy rate adjustment must be between 0 and 1'
            });
        }
        
        if (sellRateAdjustment !== undefined && sellRateAdjustment < 1) {
            return res.status(400).json({
                success: false,
                message: 'Sell rate adjustment must be greater than or equal to 1'
            });
        }
        
        if (adminFee !== undefined && adminFee < 0) {
            return res.status(400).json({
                success: false,
                message: 'Admin fee must be greater than or equal to 0'
            });
        }
        
        let settings = await Settings.findOne();
        
        if (!settings) {
            // Create new settings if none exist
            settings = new Settings({
                buyRateAdjustment: buyRateAdjustment || 0.98,
                sellRateAdjustment: sellRateAdjustment || 1.02,
                adminFee: adminFee || 3
            });
        } else {
            // Update existing settings
            if (buyRateAdjustment !== undefined) {
                settings.buyRateAdjustment = buyRateAdjustment;
            }
            if (sellRateAdjustment !== undefined) {
                settings.sellRateAdjustment = sellRateAdjustment;
            }
            if (adminFee !== undefined) {
                settings.adminFee = adminFee;
            }
        }
        
        await settings.save();
        
        res.json({
            success: true,
            message: 'Settings updated successfully',
            data: {
                buyRateAdjustment: settings.buyRateAdjustment,
                sellRateAdjustment: settings.sellRateAdjustment,
                adminFee: settings.adminFee
            }
        });
    } catch (error) {
        console.error('Error updating settings:', error);
        res.status(500).json({
            success: false,
            message: 'Error updating settings',
            error: error.message
        });
    }
});

module.exports = router;