# MetaExchange
 
## Introduction

**Two implementations:**
1. **From console**: run the program file with argument *<int>* - number of order books to check for solutions. Random amount and balance are generated and optimal solution search is performed for each order book separately. Both buy and sell options are executed on all order books. 

Example which takes 10 latest order books:
`.\MetaExchange.exe 10` 

2. **WebAPI**: HTTP POST request with payload, which contains the type of exchange (buy or sell), amount of BTC to buy or sell and user balances (in EUR and BTC). This implementation will find an optimal solution across the whole dataset of orderbooks (combinations of orderbooks are possible).

Example postman collection is appended in `.\Data\PostmanCollections`. 

To run in this mode, do not append argument when running the program. Example:

`.\MetaExchange.exe` 

## Notes:

Currently, you do have to specify the path to the OrderBook file in the code. We are working on making it configurable in the config file.


## TODOs

1. Configurable order book file path [ DONE ]
2. Culture specific decimal separators
3. Improve logging and tracability
4. Order book cache invalidation
5. Persistant storage for API requests
6. E2E test specific order books [ DONE ]
7. E2E edge cases
