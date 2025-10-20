const axios = require('axios');
const { getSettings } = require('./settingsHelper');

async function convertToCurrency(fromCurrency, amount, endpoint) {
    try {
        // Get settings from database
        const settings = await getSettings();
        
        let adjustedAmount;
        if (endpoint === 'buy') {
            adjustedAmount = amount * settings.buyRateAdjustment;
        } else {
            adjustedAmount = amount * settings.sellRateAdjustment;
        }
        
        const apiUrl = `https://data.fixer.io/api/convert?access_key=${
            process.env.FIXER_API_KEY
        }&from=${fromCurrency}&to=USD&amount=${adjustedAmount}`;
        
        const response = await axios.get(apiUrl);
        return response.data;
    } catch (error) {
        console.error('Error converting currency:', error.message);
        throw error;
    }
}

module.exports = { convertToCurrency };
