using ComicShelf.PublisherParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicShelf.UnitTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void NashaIdeaParserTest()
        {
            IPublisherParser parser = new NashaIdeaParser("https://nashaidea.com/product/friren-tom-2/");

            var result = parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 2", result.title);
            Assert.AreEqual("Проводжальниця Фрірен", result.series);
            Assert.AreEqual("", result.authors);
            Assert.AreEqual(2, result.volumeNumber);
            Assert.AreEqual("https://nashaidea.com/wp-content/uploads/2024/07/fri02-x1080.jpg", result.cover);

        }

        [TestMethod]
        public void NashaIdeaParserTest2()
        {
            IPublisherParser parser = new NashaIdeaParser("https://nashaidea.com/product/chi-tom-10/");

            var result = parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 10", result.title);
            Assert.AreEqual("Чi “Життя однієї киці”", result.series);
            Assert.AreEqual("", result.authors);
            Assert.AreEqual(10, result.volumeNumber);
            Assert.AreEqual("https://nashaidea.com/wp-content/uploads/2024/07/chi10-x1080.jpg", result.cover);

        }

        [TestMethod]
        public void MalopusParserTest()
        {
            IPublisherParser parser = new MalopusParser("https://malopus.com.ua/manga/manga-cya-porcelyanova-lyalechka-zakohalasya-tom-5");

            var result = parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 5", result.title);
            Assert.AreEqual("Ця порцелянова лялечка закохалася", result.series);
            Assert.AreEqual("Шін'ічі Фукуда", result.authors);
            Assert.AreEqual(5, result.volumeNumber);
            Assert.AreEqual("https://malopus.com.ua/image/cache/catalog/import_files/my%20dress%20up%20darling/005/Moc_Cover_ЦПЛЗ%205-700x700.png", result.cover);

        }

        [TestMethod]
        public void MalopusParserTest2()
        {
            IPublisherParser parser = new MalopusParser("https://malopus.com.ua/manga/dungeon-meshi-omnibus1");

            var result = parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Омнібус 1 (Томи 1–2)", result.title);
            Assert.AreEqual("Підземелля смакоти", result.series);
            Assert.AreEqual("Рьоко Куі", result.authors);
            Assert.AreEqual(1, result.volumeNumber);
            Assert.AreEqual("https://malopus.com.ua/image/cache/catalog/import_files/dungeon%20meshi/Moc_Cover_Підземелля%20смакоти_Том%201-700x700.png", result.cover);

        }
    }
}
