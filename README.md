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

## TODOs

1. Configurable order book file path
2. Environmentally specific decimal separators
3. Improve logging and tracability
4. Order book cache invalidation
5. Persistant storage for API requests
