const express = require('express');
const router = express.Router();
const { convertToCurrency } = require('../utils/fixerConvert');
const { getSettings } = require('../utils/settingsHelper');
const {
    getSolanaQuote,
    getSolanaTokens,
    
    getAmountWithDecimals,
} = require('../utils/getQuotation');
const TokenMetadata = require('../models/tokenMetadata');
const Quote = require('../models/quote');
const {
    
    solGenerate,
    walletSolAdmin,
    sol_connection,
    getTokenBalancesSol,
    isValidSolanaAddress,
    formatDecimal,
} = require('../utils/utils');
const {  transferToken, transferSol } = require('../utils/swapSolana');
const { PublicKey, Keypair } = require('@solana/web3.js');
const { Wallet } = require('@project-serum/anchor');
const { ethers } = require('ethers');
const bs58 = require('bs58').default;

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
 *                   description: The quote for Solana
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

/**
 * @swagger
 * /quote/sell:
 *   get:
 *     summary: Get a quote for selling crypto
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
 *         description: The amount of fiat currency to receive
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
 *                   example: 100.50
 *                 slippage:
 *                   type: string
 *                   description: The slippage percentage
 *                   example: '2.5%'
 *                 quote:
 *                   type: number
 *                   description: The amount of fiat currency received from the sale
 *                   example: 100.50
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
 *         description: Bad request if required parameters are missing
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

/**
 * @swagger
 * /quote/create:
 *   post:
 *     summary: Create a new quote
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               amountIn:
 *                 type: string
 *                 description: The amount of currency being exchanged
 *               amountOut:
 *                 type: string
 *                 description: The amount of currency received from the exchange
 *               avgPrice:
 *                 type: string
 *                 description: The average price of the crypto
 *               tokenAddress:
 *                 type: string
 *                 description: The address of the token being exchanged
 *               fiatCurrencyCode:
 *                 type: string
 *                 description: The fiat currency code used in the transaction
 *               transactionType:
 *                 type: string
 *                 enum: [buy, sell]
 *                 description: The type of transaction
 *               userAddress:
 *                 type: string
 *                 description: The user's address (required for buy transactions)
 *               totalLocal:
 *                 type: string
 *                 description: The total local currency amount
 *               totalUSD:
 *                 type: string
 *                 description: The total USD amount
 *     responses:
 *       200:
 *         description: Successfully created quote
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 message:
 *                   type: string
 *                   example: 'successfully created quote'
 *                 quote:
 *                   type: object
 *                   properties:
 *                     _id:
 *                       type: string
 *                     amountIn:
 *                       type: string
 *                     amountOut:
 *                       type: string
 *                     tokenAddress:
 *                       type: string
 *                     fiatCurrencyCode:
 *                       type: string
 *                     transactionType:
 *                       type: string
 *                     swapHash:
 *                       type: string
 *                       default: ''
 *                     txhash:
 *                       type: string
 *                       default: ''
 *                     userAddress:
 *                       type: string
 *                       default: ''
 *                     receiverAddress:
 *                       type: string
 *                       default: ''
 *                     gasFee:
 *                       type: number
 *                     count:
 *                       type: number
 *                     market:
 *                       type: string
 *                       description: The market pair
 *                     avgPrice:
 *                       type: string
 *                       description: The average price of the crypto
 *                     status:
 *                       type: string
 *                       enum: ['pending', 'processing', 'completed', 'failed']
 *                       default: 'pending'
 *                     createdAt:
 *                       type: string
 *                       format: date-time
 *                     totalLocal:
 *                       type: string
 *                       description: The total local currency amount
 *                     totalUSD:
 *                       type: string
 *                       description: The total USD amount
 *       400:
 *         description: Bad request if required fields are missing and in case of transactionType 'buy' if userAddress is missing
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 error:
 *                   type: string
 *                   example: 'fiatCurrencyCode, amountIn, amountOut, transactionType, avgPrice, totalLocal, totalUSD and tokenAddress are required'
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
// lock quote
router.post('/create', async (req, res) => {
    // Get settings for dynamic admin fee
    const settings = await getSettings();
    
    const {
        amountIn,
        amountOut,
        avgPrice,
        tokenAddress,
        fiatCurrencyCode,
        transactionType,
        userAddress,
        totalLocal,
        totalUSD,
    } = req.body;

    if (
        !fiatCurrencyCode ||
        !amountIn ||
        !amountOut ||
        !tokenAddress ||
        !transactionType ||
        !avgPrice ||
        !totalLocal ||
        !totalUSD
    ) {
        return res.status(400).json({
            error: 'fiatCurrencyCode, amountIn, amountOut, transactionType, avgPrice, totalLocal, totalUSD and tokenAddress are required',
        });
    }

    if (transactionType === 'buy' && !userAddress) {
        return res.status(400).json({ error: 'userAddress is required' });
    }

    try {
        let createdQuoteParams = {};
        const quoteCount = await Quote.countDocuments();
        const cryptoMetaData = await TokenMetadata.findOne({ address: tokenAddress });

        if (transactionType.toLowerCase() === 'buy') {
            createdQuoteParams = {
                amountIn: amountIn,
                amountOut: amountOut,
                tokenAddress: tokenAddress,
                transactionType: transactionType,
                fiatCurrencyCode: fiatCurrencyCode,
                userAddress: userAddress,
                gasFee: settings.adminFee,
                count: quoteCount,
                market: `${cryptoMetaData.symbol}-USD`,
                avgPrice: avgPrice,
                totalLocal: totalLocal,
                totalUSD: totalUSD,
            };
        } else {
            
                const { PublicKey } = await solGenerate(quoteCount);
               let receiverAddress = PublicKey;
          
            createdQuoteParams = {
                amountIn: amountIn,
                amountOut: amountOut,
                tokenAddress: tokenAddress,
                transactionType: transactionType,
                fiatCurrencyCode: fiatCurrencyCode,
                receiverAddress: receiverAddress,
                gasFee: settings.adminFee,
                count: quoteCount,
                market: `${cryptoMetaData.symbol}-USD`,
                avgPrice: avgPrice,
                totalLocal: totalLocal,
                totalUSD: totalUSD,
            };
        }
        const createdQuote = await Quote.create(createdQuoteParams);
        console.log('createdQuote:', createdQuote);
        return res.status(200).json({
            message: 'successfully created quote',
            quote: createdQuote,
        });
    } catch (e) {
        console.error(e);
        return res.status(500).json({ error: 'Internal Server Error' });
    }
});

/**
 * @swagger
 * /quote/get/{id}:
 *   get:
 *     summary: Get a quote by ID
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         description: The ID of the quote to retrieve
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: A successful response with the quote details
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 message:
 *                   type: string
 *                   description: Status message regarding the transaction
 *                   example: 'transaction completed successfully'
 *                 quote:
 *                   type: object
 *                   properties:
 *                     _id:
 *                       type: string
 *                       description: ID of the quote in DB
 *                     amountIn:
 *                       type: string
 *                       description: The amount of currency being exchanged
 *                     amountOut:
 *                       type: string
 *                       description: The amount of currency received from the exchange
 *                     tokenAddress:
 *                       type: string
 *                       description: The address of the token being exchanged
 *                     fiatCurrencyCode:
 *                       type: string
 *                       description: The fiat currency code used in the transaction
 *                     transactionType:
 *                       type: string
 *                       description: The type of transaction (buy or sell)
 *                     swapHash:
 *                       type: string
 *                       description: The swap hash
 *                       default: ''
 *                     txhash:
 *                       type: string
 *                       description: The transaction hash
 *                       default: ''
 *                     userAddress:
 *                       type: string
 *                       default: ''
 *                     receiverAddress:
 *                       type: string
 *                       default: ''
 *                     gasFee:
 *                       type: number
 *                     count:
 *                       type: number
 *                     status:
 *                       type: string
 *                       description: The current status of the quote
 *                       enum: ['pending', 'processing', 'completed', 'failed', 'swapping']
 *                       default: 'pending'
 *                     market:
 *                       type: string
 *                       description: The market pair
 *                     avgPrice:
 *                       type: string
 *                       description: The average price of the crypto
 *                     createdAt:
 *                       type: string
 *                       format: date-time
 *                       description: The date and time when the quote was created
 *       404:
 *         description: Quote not found
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 error:
 *                   type: string
 *                   example: 'Quote not found'
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
router.get('/get/:id', async (req, res) => {
    const { id } = req.params; // Get the quote ID from the request parameters

    try {
        const quote = await Quote.findById(id); // Find the quote by ID

        if (!quote) {
            return res.status(404).json({ error: 'Quote not found' }); // Handle case where quote is not found
        }

        const cryptoMetaData = await TokenMetadata.findOne({ address: quote.tokenAddress });

        if (quote.status === 'failed') {
            return res.status(200).json({
                message: 'transaction failed',
                quote: quote,
            });
        } else if (quote.status === 'completed') {
            return res.status(200).json({
                message: 'transaction completed successfully',
                quote: quote,
            });
        } 

        const signer = generateSigner(quote.count);

        if (quote.transactionType === 'buy') {
            
                let txHash;
                const tokenPublicKey = new PublicKey(quote.tokenAddress);
                const balance = await getTokenBalancesSol(
                    walletSolAdmin,
                    tokenPublicKey,
                    sol_connection,
                    cryptoMetaData,
                );
                console.log(balance, 'balance');

                if (+balance >= +quote.amountOut) {
                    const wallet = Keypair.fromSecretKey(bs58.decode(process.env.keySOL));

                    if (cryptoMetaData.symbol === 'SOL') {
                        txHash = await transferSol(
                            sol_connection,
                            wallet,
                            quote.userAddress,
                            quote.amountOut,
                            cryptoMetaData.decimals,
                        );
                    } else {
                        txHash = await transferToken(
                            wallet,
                            quote.tokenAddress,
                            quote.userAddress,
                            quote.amountOut,
                            cryptoMetaData.decimals,
                        );
                    }
                } else {
                    quote.status = 'swapping';
                    quote.save();

                    return res.status(200).json({
                        message: 'swapping amount',
                        quote: quote,
                    });
                }

                if (txHash) {
                    quote.txhash = txHash;
                    quote.status = 'completed';
                    quote.save();
                    return res.status(200).json({
                        message: 'transaction is completed',
                        quote: quote,
                    });
                } else {
                    return res.status(200).json({
                        message: 'trying again',
                        quote: quote,
                    });
                }
            
        } else if (quote.transactionType === 'sell') {
        
                let txHash;
                const tokenPublicKey = new PublicKey(quote.tokenAddress);
                const { PrivateKey: privKey } = await solGenerate(quote.count);

                const middleKeyPair = Keypair.fromSecretKey(bs58.decode(privKey));
                const walletSolMiddle = new Wallet(Keypair.fromSecretKey(bs58.decode(privKey)));
                const fromKeyPair = Keypair.fromSecretKey(bs58.decode(process.env.keySOL));
                let balance = await getTokenBalancesSol(
                    walletSolMiddle,
                    tokenPublicKey,
                    sol_connection,
                    cryptoMetaData,
                );
                console.log('balance before gas: ', balance);

                if (cryptoMetaData.symbol === 'SOL') {
                    if (+balance >= +quote.amountIn) {
                        balance = balance - 0.001;

                        console.log('balance after gas: ', balance);
                        txHash = await transferSol(
                            sol_connection,
                            middleKeyPair,
                            walletSolAdmin.publicKey.toString(),
                            balance,
                            cryptoMetaData.decimals,
                        );
                    } else {
                        return res.status(200).json({
                            message: 'waiting for transfer',
                            quote: quote,
                        });
                    }
                } else {
                    if (+balance >= +quote.amountIn) {
                        if (quote.status === 'processing') {
                            txHash = await transferToken(
                                middleKeyPair,
                                quote.tokenAddress,
                                walletSolAdmin.publicKey.toString(),
                                balance,
                                cryptoMetaData.decimals,
                            );
                        } else {
                            const feeTransfered = await transferSol(
                                sol_connection,
                                fromKeyPair,
                                quote.receiverAddress,
                                0.001,
                                9,
                            );

                            if (feeTransfered) {
                                quote.status = 'processing';
                                quote.save();
                                return res.status(200).json({
                                    message: 'gas fee transfered to transfer wallet',
                                    quote: quote,
                                });
                            } else {
                                return res.status(200).json({
                                    message: 'error transfering gas fee to transfer wallet',
                                    quote: quote,
                                });
                            }
                        }
                    }
                }

                if (txHash) {
                    quote.txhash = txHash;
                    quote.status = 'completed';
                    quote.save();
                    return res.status(200).json({
                        message: 'transaction is completed',
                        quote: quote,
                    });
                }
            
        }
        return res.status(200).json({
            message: 'transaction is pending',
            quote: quote,
        });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ error: 'Internal Server Error' }); // Handle server errors
    }
});

/**
 * @swagger
 * /quote/check-address:
 *   get:
 *     summary: Check if an address is valid
 *     parameters:
 *       - in: query
 *         name: address
 *         required: true
 *         description: The address to check
 *         schema:
 *           type: string
 *       - in: query
 *         name: chain
 *         required: true
 *         description: The blockchain of the address (e.g., SOL)
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Address is valid
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 valid:
 *                   type: boolean
 *                   example: true
 *       400:
 *         description: Invalid address or missing parameters
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 error:
 *                   type: string
 *                   example: 'Address and chain are required'
 */
router.get('/check-address', (req, res) => {
    const { address, chain } = req.query;

    if (!address || !chain) {
        return res.status(400).json({ error: 'Address and chain are required' });
    }

    let isValid = isValidSolanaAddress(address);

    if (isValid) {
        return res.status(200).json({ valid: true });
    } else {
        return res.status(400).json({ valid: false, error: 'Invalid address' });
    }
});

module.exports = router;
