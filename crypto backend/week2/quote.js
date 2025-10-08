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
