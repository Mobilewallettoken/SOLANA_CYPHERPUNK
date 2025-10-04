const { ethers } = require("ethers");

const { Keypair, Connection, PublicKey } = require("@solana/web3.js");
const bip39 = require("bip39");
const ecc = require("tiny-secp256k1");
const { BIP32Factory } = require("bip32");
const bip32 = BIP32Factory(ecc);
const bs58 = require("bs58").default;
const { Wallet } = require("@project-serum/anchor");

const solGenerate = async (walletCount) => {
	try {
		const seed = await bip39.mnemonicToSeed(process.env.MID_MNEMONIC);

		// Derive a private key using a BIP44 derivation path
		const root = bip32.fromSeed(seed);
		// eslint-disable-next-line
		const derivationPath = "m/44'/501'/0'/";
		const fullpath = derivationPath + `${walletCount}'`;
		const child = root.derivePath(fullpath);

		// Create a Keypair from the derived private key
		const keypair = Keypair.fromSeed(child.privateKey.slice(0, 32));

		// Extracting the secret key
		const secretKey = keypair.secretKey;
		console.log("Secret Key Uint8Array:", secretKey);

		// Converting the private key to a base58 string
		const PrivateKey = bs58.encode(keypair.secretKey);

		const PublicKey = keypair.publicKey.toBase58();

		return { PrivateKey, PublicKey };
	} catch (e) {
		console.log("error=======>", e.message);
	}
};

const providers = {
	sol: "https://api.mainnet-beta.solana.com",
};

const sol_connection = new Connection(providers.sol, "confirmed");

const walletSolAdmin = new Wallet(
	Keypair.fromSecretKey(bs58.decode(process.env.keySOL))
);

async function getTokenBalancesSol(
	wallet,
	tokenPublicKey,
	connection,
	cryptoMetaData
) {
	try {
		let balance_sol;
		if (cryptoMetaData.symbol === "SOL") {
			balance_sol = await connection.getBalance(
				new PublicKey(wallet.publicKey.toString())
			);
		} else {
			const tokenAccounts = await connection.getParsedTokenAccountsByOwner(
				new PublicKey(wallet.publicKey.toString()),
				{
					mint: tokenPublicKey,
				}
			);

			balance_sol = tokenAccounts.value.reduce((acc, tokenAccount) => {
				return acc + tokenAccount.account.data.parsed.info.tokenAmount.uiAmount;
			}, 0);
		}

		if (balance_sol === 0) {
			return balance_sol;
		} else {
			if (cryptoMetaData.symbol === "SOL") {
				let x = ethers.utils.formatUnits(
					balance_sol.toString(),
					cryptoMetaData.decimals
				);
				return +x;
			} else return +balance_sol;
		}
	} catch (error) {
		console.log(" ------------");
		console.log("error:", error);
		console.log(" ------------");
		return 0;
	}
}

module.exports = {
	solGenerate,
	sol_connection,
	walletSolAdmin,
	getTokenBalancesSol,
};
