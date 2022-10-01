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

        [Fact]
        public void ReadNumberOfOrderBooks_NonExistingFilePath_ThrowsException()
        {
            Action result = () => Sut.ReadNumberOfOrderBooks("notExistingPath", 1);

            result.Should().Throw<Exception>();
        }

        [Fact]
        public void ReadNumberOfOrderBooks_EmptyFile_ReturnsEmptyList()
        {
            string fullFilePath = CreateEmptyFile();

            IList<OrderBook> result = Sut.ReadNumberOfOrderBooks(fullFilePath, 1);

            result.Should().BeEmpty();
        }

        [Fact]
        public void ReadNumberOfOrderBooks_TwoValidLines_ReadsOnlyFirstOne()
        {
            string fullFilePath = CreateTwoValidLines();

            IList<OrderBook> result = Sut.ReadNumberOfOrderBooks(fullFilePath, 1);

            result.Should().BeEquivalentTo(new List<OrderBook> { ExpectedOrderBooks[0] });
        }

        [Fact]
        public void ReadNumberOfOrderBooks_InvalidLineDetected_ThrowsException()
        {
            string fullFilePath = CreateInvalidLine();

            Action result = () => Sut.ReadNumberOfOrderBooks(fullFilePath, 1);

            result.Should().Throw<Exception>();
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
            const string fileWithNoEventListElements = @"1548759600.25189	{""AcqTime"":""2019-01-29T11:00:00.2518854Z"",""Bids"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.64}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.63}}],""Asks"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}}]}
1548759601.33694	{""AcqTime"":""2019-01-29T11:00:01.3369432Z"",""Bids"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.67}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":1.11117578,""Price"":2960.65}}],""Asks"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}}]}";
            string fullFilePath = Path.Combine(_folderPath, _fileName);
            File.WriteAllText(fullFilePath, fileWithNoEventListElements);

            return fullFilePath;
        }

        public string CreateInvalidLine()
        {
            const string fileWithNoEventListElements = @"not a valid line.
1548759601.33694	{""AcqTime"":""2019-01-29T11:00:01.3369432Z"",""Bids"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":0.01,""Price"":2960.67}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Buy"",""Kind"":""Limit"",""Amount"":1.11117578,""Price"":2960.65}}],""Asks"":[{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.29}},{""Order"":{""Id"":null,""Time"":""0001-01-01T00:00:00"",""Type"":""Sell"",""Kind"":""Limit"",""Amount"":0.405,""Price"":2964.3}}]}";
            string fullFilePath = Path.Combine(_folderPath, _fileName);
            File.WriteAllText(fullFilePath, fileWithNoEventListElements);

            return fullFilePath;
        }

        private static IList<OrderBook> CreateExpectedOrderBooks()
        {
            List<Bid> bids1 = new()
            {
                new Bid { Order = CreateOrder("Buy", 0.01M, 2960.64M) },
                new Bid { Order = CreateOrder("Buy", 0.01M, 2960.63M) }
            };

            List<Bid> bids2 = new()
            {
                new Bid { Order = CreateOrder("Buy", 0.01M, 2960.67M) },
                new Bid { Order = CreateOrder("Buy", 1.11117578M, 2960.65M) }
            };

            List<Ask> asks1 = new()
            {
                new Ask { Order = CreateOrder("Sell", 0.405M, 2964.29M) },
                new Ask { Order = CreateOrder("Sell", 0.405M, 2964.3M) }
            };

            List<Ask> asks2 = new()
            {
                new Ask { Order = CreateOrder("Sell", 0.405M, 2964.29M) },
                new Ask { Order = CreateOrder("Sell", 0.405M, 2964.3M) }
            };

            IList<OrderBook> orderBooks = new List<OrderBook>
            {
                new OrderBook("2019-01-29T11:00:00.2518854Z", bids1, asks1),
                new OrderBook("2019-01-29T11:00:01.3369432Z", bids2, asks2)
            };

            return orderBooks;
        }

        private static Order CreateOrder(string type, decimal amount, decimal price)
        {
            return new Order
            {
                Id = null,
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