# DBlockchain
DBlockchain is .net blockchain which includes:

  - Node
  - Built in miner
  - Block explorer
  - Faucet

## How to use?
- Run node (DBlockchain console app)
- Run block explorer (DBlockchain.BlockExplorer web app)

### Commands
{p} means parameter
| Syntax | Description |
| ------ | ------ |
| create-wallet | Creates wallet if not exist |
| account-info | Shows your address and public key |
| balance -address {p} -confirms {p} | Shows balance of address with confirms |
| mine | start mining |
| send -to {p} -amount {p} | sends money to address |
| connect -ip {p} -p {p} | connects to peer |

#### Storage
All data is stored in Storage folder
```sh
Blocks folder - stores blocks (example block_0.json, block_1.json...)
Wallet folder - stores encrypted private key (encryptedPrivateKey.json)
peers.json - stores connected peers to you
pendingTransactions.json - stores pending transactions 
```
