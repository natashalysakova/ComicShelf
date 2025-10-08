using ComicShelf.PublisherParsers;

namespace ComicShelf.Parsers
{
    [TestClass]
    public class MalopusTestClass
    {

        [TestMethod]
        public async Task MalopusTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://malopus.com.ua/manga/manga-cya-porcelyanova-lyalechka-zakohalasya-tom-5");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 5", result.title);
            Assert.AreEqual("Ця порцелянова лялечка закохалася", result.series);
            Assert.AreEqual("Шін'ічі Фукуда", result.authors);
            Assert.AreEqual(5, result.volumeNumber);
            Assert.AreEqual("https://malopus.com.ua//content/images/30/600x600l80mc0/manga-cya-porcelyanova-lyalechka-zakohalasya-tom-5-63577913695375.png", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("Mal'opus", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8168-12-4", result.isbn);
            Assert.AreEqual(15, result.totalVolumes);
            Assert.AreEqual("finished", result.seriesStatus);
            Assert.AreEqual("Sono Bisque Doll wa Koi wo Suru", result.originalSeriesName);
        }

        [TestMethod]
        public async Task MalopusPreorderTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://malopus.com.ua/manga/dungeon-meshi-omnibus1");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Омнібус 1 (Томи 1–2)", result.title);
            Assert.AreEqual("Підземелля смакоти", result.series);
            Assert.AreEqual("Рьоко Куі", result.authors);
            Assert.AreEqual(1, result.volumeNumber);
            Assert.AreEqual("https://malopus.com.ua//content/images/41/600x600l80mc0/dungeon-meshi-omnibus1-85734836570482.png", result.cover);
            Assert.AreEqual("2025-01-31", result.release);
            Assert.AreEqual("Mal'opus", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8168-27-8", result.isbn);
            Assert.AreEqual(7, result.totalVolumes);
            Assert.AreEqual("finished", result.seriesStatus);
            Assert.AreEqual("Dungeon Meshi", result.originalSeriesName);

        }

        [TestMethod]
        public async Task MalopusOneShotTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://malopus.com.ua/manga/nijigahara-holograph");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Голограф Веселкового поля", result.title);
            Assert.AreEqual("Голограф Веселкового поля", result.series);
            Assert.AreEqual("Ініо Асано", result.authors);
            Assert.AreEqual(-1, result.volumeNumber);
            Assert.AreEqual("https://malopus.com.ua//content/images/24/600x600l80mc0/nijigahara-holograph-55453872054888.png", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("Mal'opus", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8168-11-7", result.isbn);
            Assert.AreEqual(1, result.totalVolumes);
            Assert.AreEqual("oneshot", result.seriesStatus);
            Assert.AreEqual("Nijigahara Holograph", result.originalSeriesName);


        }

        [TestMethod]
        public async Task Malopus_VolumeNumber_ShouldBe()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://malopus.com.ua/manga/dark-souls-redemption-vol1");
            var result = await parser.Parse();


            Assert.AreEqual(1, result.volumeNumber);
            Assert.AreEqual(1, result.totalVolumes);
            Assert.AreEqual("ongoing", result.seriesStatus);
        }

        [TestMethod]
        public async Task Malopus_ReleaseDate_ShouldBe_Parsed()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://malopus.com.ua/manga/reborn-as-a-vending-machine-vol-2/");
            var result = await parser.Parse();

            Assert.AreEqual("2026-02-28", result.release);
        }

        [TestMethod]
        [DataRow("https://malopus.com.ua/manga/bocchi-the-rock-vol2", "Самітниця-рокерка", "Том 2", 2)]
        [DataRow("https://malopus.com.ua/manga/reborn-as-a-vending-machine-vol-2/", "Я переродився торговим автоматом і тепер блукаю підземеллям", "Том 2", 2)]
        [DataRow("https://malopus.com.ua/manga/manga-shlyah-domogospodarya-tom-8/", "Шлях домогосподаря", "Том 8", 8)]
        [DataRow("https://malopus.com.ua/manga/dungeon-meshi-omnibus-4/", "Підземелля смакоти", "Омнібус 4 (Томи 7–8)", 4)]
        public async Task Malopus_TitleAndSeries_ShouldBe_Parsed(string url, string expectedSeries, string expectedTitle, int expectedVolumeNumber)
        {
            var parser = new PublisherParsersFactory().CreateParser(url);
            var result = await parser.Parse();

            Assert.AreEqual(expectedSeries, result.series);
            Assert.AreEqual(expectedTitle, result.title);
            Assert.AreEqual(expectedVolumeNumber, result.volumeNumber);
            //Assert.AreEqual(DateTime.Parse("2024-06-17"), result.PreorderStartDate);
        }
    }
}
