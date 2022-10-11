# MetaExchange
 
## Introduction

**Two implementations:**
1. **From console**: run the program file with argument *-consoleapp* and [string] - type of order to execute (buy or sell) and [decimal] - amount of btc to buy or sell. Order is executed over the whole dataset provided in configuration file to find the optimal sequence.

Example for buying and selling 2.3 btc:
`.\MetaExchange.exe -consoleapp buy 2.3` or
`.\MetaExchange.exe -consoleapp sell 2.3` 

2. **WebAPI**: HTTP POST request with payload, which contains the type of exchange (buy or sell) and amount of BTC to buy or sell. This implementation will find an optimal solution across the whole dataset provided in configuration file.

Example postman collection is appended in `.\Data\PostmanCollections`. 

To run in this mode, append argument *-webapp* when running the program. Example:

`.\MetaExchange.exe -webapp` 

## Notes

Use `-help` for help.


## TODOs

1. Configurable order book file path [ DONE ]
2. Culture specific decimal separators
3. Improve logging and tracability
4. Order book cache invalidation
5. Persistant storage for API requests
6. E2E test specific order books [ DONE ]
7. E2E edge cases
