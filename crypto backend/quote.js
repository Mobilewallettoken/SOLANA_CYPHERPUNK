const express = require("express");
const router = express.Router();
const { convertToCurrency } = require("../utils/fixerConvert");
const { getSettings } = require("../utils/settingsHelper");
const {
	getSolanaQuote,
	getSolanaTokens,
	getAmountWithDecimals,
} = require("../utils/getQuotation");
const TokenMetadata = require("../models/tokenMetadata");

const { formatDecimal } = require("../utils/utils");

const { ethers } = require("ethers");

/**
 * @swagger
 * /quote/buy:
 *   get:
 *     summary: Get a quote for buying crypto
 *     parameters:
 *       - in: query
 *         name: fiatCurrencyCode
 *         required: true
 *         description: The fiat currency code (e.g., USD)
 *         schema:
 *           type: string
 *       - in: query
 *         name: amount
 *         required: true
 *         description: The amount of fiat currency
 *         schema:
 *           type: string
 *       - in: query
 *         name: cryptoCurrencyAddress
 *         required: true
 *         description: The address of the cryptocurrency
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: A successful response with the quote
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 market:
 *                   type: string
 *                   description: The market pair
 *                   example: 'SOL-USD'
 *                 avgPrice:
 *                   type: number
 *                   description: The average price of the crypto
 *                   example: 200.50
 *                 slippage:
 *                   type: string
 *                   description: The slippage percentage
 *                   example: '2.5%'
 *                 quote:
 *                   type: number
 *                   description: The quote for Solana or BSC
 *                   example: 200.50
 *                 paymentMethod:
 *                   type: string
 *                   description: The fiat currency code
 *                   example: 'USD'
 *                 gasFee:
 *                   type: number
 *                   description: The gas fee for the transaction
 *                   example: 3
 *                 totalLocal:
 *                   type: number
 *                   description: The total local currency amount
 *                   example: 100
 *                 totalUSD:
 *                   type: number
 *                   description: The total USD amount
 *                   example: 100.50
 *                 exchangeRate:
 *                   type: number
 *                   description: The exchange rate between the fiat and crypto
 *                   example: 1.005
 *       400:
 *         description: Bad request if required parameters are missing or if Fixer API returns an error
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 error:
 *                   type: string
 *                   example: 'Currency code, amount, chain and cryptoCurrencyAddress are required'
 *       404:
 *         description: Chain not found in the database
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 error:
 *                   type: string
 *                   example: 'chain not found in db'
 *       500:
 *         description: Internal server error
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 error:
 *                   type: string
 *                   example: 'Internal Server Error'
 */
router.get("/buy", async (req, res) => {
	const { fiatCurrencyCode, amount, cryptoCurrencyAddress } = req.query;
	const slippage = "20";

	if (!fiatCurrencyCode || !amount || !cryptoCurrencyAddress) {
		return res.status(400).json({
			error:
				"Currency code, amount, chain and cryptoCurrencyAddress are required",
		});
	}

	try {
		// Get settings for dynamic admin fee
		const settings = await getSettings();

		let convertedValue;
		if (fiatCurrencyCode === "USD") convertedValue = +amount;
		else {
			convertedValue = await convertToCurrency(
				fiatCurrencyCode,
				amount,
				"sell"
			);

			if (!convertedValue.success) {
				console.log("Fixer API Error:", convertedValue.error);
				return res.status(400).json({ error: convertedValue.error.info });
			}
			convertedValue = convertedValue.result;
		}

		let cryptoMetaData = await TokenMetadata.findOne({
			address: cryptoCurrencyAddress,
		});

		const { USDT_MINT, token_MINT } = getSolanaTokens(cryptoCurrencyAddress);
		let quote;
		if (cryptoMetaData.symbol === "USDT") {
			quote = 1;
		} else {
			const amountWithDecimal = getAmountWithDecimals(cryptoMetaData, "1");
			quote = await getSolanaQuote(
				token_MINT,
				USDT_MINT,
				amountWithDecimal,
				slippage
			);

			quote = ethers.utils.formatUnits(quote.outAmount, 6);
		}

		return res.status(200).json({
			market: `${cryptoMetaData.symbol}-USD`,
			avgPrice: quote,
			slippage: "2.5%",
			quote: formatDecimal((convertedValue - settings.adminFee) / quote),
			paymentMethod: fiatCurrencyCode,
			gasFee: settings.adminFee,
			totalLocal: amount,
			totalUSD: formatDecimal(convertedValue),
			exchangeRate: formatDecimal(amount / convertedValue),
		});
	} catch (error) {
		console.error(error);
		return res.status(500).json({ error: "Internal Server Error" });
	}
});

router.get("/sell", async (req, res) => {
	const { fiatCurrencyCode, amount, cryptoCurrencyAddress } = req.query;
	const slippage = "20";

	if (!fiatCurrencyCode || !amount || !cryptoCurrencyAddress) {
		return res.status(400).json({
			error:
				"Currency code, amount, chain and cryptoCurrencyAddress are required",
		});
	}

	try {
		// Get settings for dynamic admin fee
		const settings = await getSettings();

		let convertedValue;
		if (fiatCurrencyCode === "USD") convertedValue = amount;
		else {
			convertedValue = await convertToCurrency(
				fiatCurrencyCode,
				amount,
				"sell"
			);

			if (!convertedValue.success) {
				console.log("Fixer API Error:", convertedValue.error);
				return res.status(400).json({ error: convertedValue.error.info });
			}
			convertedValue = convertedValue.result;
		}
		let cryptoMetaData = await TokenMetadata.findOne({
			address: cryptoCurrencyAddress,
		});

		const { USDT_MINT, token_MINT } = getSolanaTokens(cryptoCurrencyAddress);
		let quote;
		if (cryptoMetaData.symbol === "USDT") {
			quote = 1;
		} else {
			const amountWithDecimal = getAmountWithDecimals(cryptoMetaData, "1");
			quote = await getSolanaQuote(
				token_MINT,
				USDT_MINT,
				amountWithDecimal,
				slippage
			);

			quote = ethers.utils.formatUnits(quote.outAmount, 6);
		}
		return res.status(200).json({
			market: `${cryptoMetaData.symbol}-USD`,
			avgPrice: quote,
			slippage: "2.5%",
			quote: formatDecimal((+convertedValue + settings.adminFee) / quote),
			paymentMethod: fiatCurrencyCode,
			gasFee: settings.adminFee,
			totalLocal: amount,
			totalUSD: formatDecimal(+convertedValue + settings.adminFee),
			exchangeRate: formatDecimal(amount / convertedValue),
		});
	} catch (error) {
		console.error(error);
		return res.status(500).json({ error: "Internal Server Error" });
	}
});

module.exports = router;
