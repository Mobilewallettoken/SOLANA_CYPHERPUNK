const {
	Transaction,
	PublicKey,
	sendAndConfirmTransaction,
	SystemProgram,
} = require("@solana/web3.js");
const {
	getOrCreateAssociatedTokenAccount,
	createTransferInstruction,
} = require("@solana/spl-token");
const { sol_connection } = require("./utils");

const transferToken = async (
	wallet,
	MINT_ADDRESS,
	DESTINATION_WALLET,
	TRANSFER_AMOUNT,
	numberDecimals
) => {
	try {
		const sourceAccount = await getOrCreateAssociatedTokenAccount(
			sol_connection,
			wallet,
			new PublicKey(MINT_ADDRESS),
			wallet.publicKey
		);

		const destinationAccount = await getOrCreateAssociatedTokenAccount(
			sol_connection,
			wallet,
			new PublicKey(MINT_ADDRESS),
			new PublicKey(DESTINATION_WALLET)
		);

		const tx = new Transaction();
		tx.add(
			createTransferInstruction(
				sourceAccount.address,
				destinationAccount.address,
				wallet.publicKey,
				TRANSFER_AMOUNT * Math.pow(10, numberDecimals)
			)
		);

		const latestBlockHash = await sol_connection.getLatestBlockhash(
			"confirmed"
		);
		tx.recentBlockhash = latestBlockHash.blockhash;
		tx.feePayer = wallet.publicKey;

		const txid = await sendAndConfirmTransaction(sol_connection, tx, [wallet]);

		console.log(`https://solscan.io/tx/${txid}`);
		return txid;
	} catch (error) {
		console.log(" ------------");
		console.log("error:", error);
		console.log(" ------------");
		return false;
	}
};

const transferSol = async (
	sol_connection,
	wallet,
	DESTINATION_WALLET,
	TRANSFER_AMOUNT,
	numberDecimals
) => {
	try {
		const tx = new Transaction();
		tx.add(
			SystemProgram.transfer({
				fromPubkey: wallet.publicKey,
				toPubkey: new PublicKey(DESTINATION_WALLET),
				lamports: Number(
					TRANSFER_AMOUNT * Math.pow(10, numberDecimals)
				).toFixed(0),
			})
		);

		const latestBlockHash = await sol_connection.getLatestBlockhash(
			"confirmed"
		);
		tx.recentBlockhash = await latestBlockHash.blockhash;
		tx.feePayer = wallet.publicKey;

		const txid = await sendAndConfirmTransaction(sol_connection, tx, [wallet]);

		console.log(`https://solscan.io/tx/${txid}`);
		return txid;
	} catch (error) {
		console.log(" ------------");
		console.log("error:", error);
		console.log(" ------------");
		return false;
	}
};

module.exports = { transferToken, transferSol };
