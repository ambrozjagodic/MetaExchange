using FluentAssertions;
using MetaExchange.Core;
using MetaExchange.Logic;
using System.Reflection;

namespace MetaExchange.Tests.Logic
{
    public class FileOrderBookReaderTest : FileOrderBookReaderDriver
    {
        [Fact]
        public void ReadOrderBook_NonExistingFilePath_ThrowsException()
        {
            Func<Task> result = async () => await Sut.ReadOrderBook("notExistingPath");

            result.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ReadOrderBook_EmptyFile_ReturnsEmptyList()
        {
            string fullFilePath = CreateEmptyFile();

            IList<OrderBook> result = await Sut.ReadOrderBook(fullFilePath);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ReadOrderBook_TwoValidLines_ReturnsTwoOrderBooks()
        {
            string fullFilePath = CreateTwoValidLines();

            IList<OrderBook> result = await Sut.ReadOrderBook(fullFilePath);

            result.Should().BeEquivalentTo(ExpectedOrderBooks);
        }

        [Fact]
        public void ReadOrderBook_InvalidLineDetected_ThrowsException()
        {
            string fullFilePath = CreateInvalidLine();

            Func<Task> result = async () => await Sut.ReadOrderBook(fullFilePath);

            result.Should().ThrowAsync<Exception>();
        }
    }

    public class FileOrderBookReaderDriver : IDisposable
    {
        private readonly string _folderLocationPath;
        private readonly string _folderPath;
        private readonly string _fileName;
        private readonly IList<string> _foldersToDeleteAfterTest;

        public string FilePath { get; private set; }
        public IList<OrderBook> ExpectedOrderBooks { get; }

        public IOrderBookReader Sut { get; }

        public FileOrderBookReaderDriver()
        {
            _foldersToDeleteAfterTest = new List<string>();

            _folderLocationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _folderPath = CreateFolder();
            _fileName = $"{Guid.NewGuid()}";

            FilePath = Path.Combine(_folderPath, _fileName);
            ExpectedOrderBooks = CreateExpectedOrderBooks();

            Sut = new FileOrderBookReader();
        }

        public string CreateFolder()
        {
            string folderPath = Path.Combine(_folderLocationPath, $"{Guid.NewGuid()}");
            Directory.CreateDirectory(folderPath);
            _foldersToDeleteAfterTest.Add(folderPath);

            return folderPath;
        }

        public string CreateEmptyFile()
        {
            string fullFilePath = Path.Combine(_folderPath, _fileName);

            File.WriteAllText(fullFilePath, string.Empty);

            return fullFilePath;
        }

        public string CreateTwoValidLines()
        {
            const string fileWithNoEventListElements = @"1548759600.25189	{""Id"":""b0ee9df2-3ffc-4496-848f-178e93d873c5"",""AcqTime"":""2019-01-29T11:00:00.2518854Z"",""BalanceEur"":1234.56,""BalanceBtc"":1.23,""Bids"":[{""Order"":{""Id"":""a247e55a-77ff-480a-95ef-da044c2b1c18"",""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.64}},{""Order"":{""Id"":""33806bb1-cafd-495d-8dc1-9af55a64e044"",""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.63}}],""Asks"":[{""Order"":{""Id"":""40040077-ea97-42af-8e59-b1d5d22e7965"",""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},{""Order"":{""Id"":""de6dbc70-ed35-4903-b91f-07dd678726c6"",""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}}]}
1548759601.33694	{""Id"":""c58bf1a4-abbb-41a8-bfda-62bf0fa72890"",""AcqTime"":""2019-01-29T11:00:01.3369432Z"",""BalanceEur"":9999.99,""BalanceBtc"":9.99,""Bids"":[{""Order"":{""Id"":""48154f0c-4958-4793-a263-8887904304c8"",""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.67}},{""Order"":{""Id"":""22f07d34-feb7-4427-8eff-b045d1c088e2"",""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":1.11117578,""Price"":2960.65}}],""Asks"":[{""Order"":{""Id"":""a02435c8-849b-491f-882a-b677f44fd792"",""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},{""Order"":{""Id"":""e41272a2-961f-4e2a-bf2e-b578af1087ea"",""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}}]}";
            string fullFilePath = Path.Combine(_folderPath, _fileName);
            File.WriteAllText(fullFilePath, fileWithNoEventListElements);

            return fullFilePath;
        }

        public string CreateInvalidLine()
        {
            const string fileWithNoEventListElements = @"not a valid line.
1548759601.33694	{""Id"":""c58bf1a4-abbb-41a8-bfda-62bf0fa72890"",""AcqTime"":""2019-01-29T11:00:01.3369432Z"",""Bids"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.67}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":1.11117578,""Price"":2960.65}}],""Asks"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}}]}";
            string fullFilePath = Path.Combine(_folderPath, _fileName);
            File.WriteAllText(fullFilePath, fileWithNoEventListElements);

            return fullFilePath;
        }

        private static IList<OrderBook> CreateExpectedOrderBooks()
        {
            List<Bid> bids1 = new()
            {
                new Bid { Order = CreateOrder(Guid.Parse("a247e55a-77ff-480a-95ef-da044c2b1c18"), "Buy", 0.01M, 2960.64M) },
                new Bid { Order = CreateOrder(Guid.Parse("33806bb1-cafd-495d-8dc1-9af55a64e044"), "Buy", 0.01M, 2960.63M) }
            };

            List<Bid> bids2 = new()
            {
                new Bid { Order = CreateOrder(Guid.Parse("48154f0c-4958-4793-a263-8887904304c8"), "Buy", 0.01M, 2960.67M) },
                new Bid { Order = CreateOrder(Guid.Parse("22f07d34-feb7-4427-8eff-b045d1c088e2"), "Buy", 1.11117578M, 2960.65M) }
            };

            List<Ask> asks1 = new()
            {
                new Ask { Order = CreateOrder(Guid.Parse("40040077-ea97-42af-8e59-b1d5d22e7965"), "Sell", 0.405M, 2964.29M) },
                new Ask { Order = CreateOrder(Guid.Parse("de6dbc70-ed35-4903-b91f-07dd678726c6"), "Sell", 0.405M, 2964.3M) }
            };

            List<Ask> asks2 = new()
            {
                new Ask { Order = CreateOrder(Guid.Parse("a02435c8-849b-491f-882a-b677f44fd792"), "Sell", 0.405M, 2964.29M) },
                new Ask { Order = CreateOrder(Guid.Parse("e41272a2-961f-4e2a-bf2e-b578af1087ea"), "Sell", 0.405M, 2964.3M) }
            };

            IList<OrderBook> orderBooks = new List<OrderBook>
            {
                new OrderBook(Guid.Parse("b0ee9df2-3ffc-4496-848f-178e93d873c5"), "2019-01-29T11:00:00.2518854Z", 1234.56M, 1.23M, bids1, asks1),
                new OrderBook(Guid.Parse("c58bf1a4-abbb-41a8-bfda-62bf0fa72890"), "2019-01-29T11:00:01.3369432Z", 9999.99M, 9.99M, bids2, asks2)
            };

            return orderBooks;
        }

        private static Order CreateOrder(Guid id, string type, decimal amount, decimal price)
        {
            return new Order
            {
                Id = id,
                Time = "0001-01-01T00:00:00",
                Type = type,
                Kind = "Limit",
                Amount = amount,
                Price = price
            };
        }

        public void Dispose()
        {
            foreach (string folder in _foldersToDeleteAfterTest)
            {
                Directory.Delete(folder, true);
            }

            GC.SuppressFinalize(this);
        }
    }
}