using Backend.Models.Enums;
using ComicShelf.PublisherParsers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Services;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ComicShelf.Parsers
{
    [TestClass]
    public class NashaIdeaTestClass
    {

        [TestMethod]
        public async Task NashaIdeaPreorderTest()
        {


            var parser = new PublisherParsersFactory().CreateParser("https://nashaidea.com/product/friren-tom-2/");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 2", result.title);
            Assert.AreEqual("Проводжальниця Фрірен", result.series);
            Assert.AreEqual("", result.authors);
            Assert.AreEqual(2, result.volumeNumber);
            Assert.AreEqual("https://nashaidea.com/wp-content/uploads/2024/07/fri02-x1080.jpg", result.cover);
            Assert.AreEqual("2024-09-15", result.release);
            Assert.AreEqual("NashaIdea", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8396-49-7", result.isbn);
            Assert.AreEqual(13, result.totalVolumes);
            Assert.AreEqual("ongoing", result.seriesStatus);


        }

        [TestMethod]
        public async Task NashaIdeaTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://nashaidea.com/product/chi-tom-10/");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 10", result.title);
            Assert.AreEqual("Чi “Життя однієї киці”", result.series);
            Assert.AreEqual("", result.authors);
            Assert.AreEqual(10, result.volumeNumber);
            Assert.AreEqual("https://nashaidea.com/wp-content/uploads/2024/07/chi10-x1080.jpg", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("NashaIdea", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8396-46-6", result.isbn);
            Assert.AreEqual(12, result.totalVolumes);
            Assert.AreEqual("finished", result.seriesStatus);

        }

        [TestMethod]
        public async Task NashaIdeaOneShotTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://nashaidea.com/product/vitaiemo-v-koto-kafe/");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Вітаємо в кото-кафе", result.title);
            Assert.AreEqual("Вітаємо в кото-кафе", result.series);
            Assert.AreEqual("", result.authors);
            Assert.AreEqual(-1, result.volumeNumber);
            Assert.AreEqual("https://nashaidea.com/wp-content/uploads/2023/10/cat-flat.jpg", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("NashaIdea", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8109-88-2", result.isbn);
            Assert.AreEqual(1, result.totalVolumes);
            Assert.AreEqual("oneshot", result.seriesStatus);

        }


        [TestMethod]
        public async Task NashaIdeaFinishedTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://nashaidea.com/product/proshhavaj-troyandovyj-sade-tom-3/");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Том 3", result.title);
            Assert.AreEqual("Прощавай, трояндовий саде", result.series);
            Assert.AreEqual("", result.authors);
            Assert.AreEqual(3, result.volumeNumber);
            Assert.AreEqual("https://nashaidea.com/wp-content/uploads/2024/08/mrg03-x1080.jpg", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("NashaIdea", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8396-55-8", result.isbn);
            Assert.AreEqual(3, result.totalVolumes);
            Assert.AreEqual("finished", result.seriesStatus);

        }


    }

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
            Assert.AreEqual("https://malopus.com.ua/image/cache/catalog/import_files/my%20dress%20up%20darling/005/Moc_Cover_ЦПЛЗ%205-700x700.png", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("Mal'opus", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8168-12-4", result.isbn);
            Assert.AreEqual(13, result.totalVolumes);
            Assert.AreEqual("ongoing", result.seriesStatus);
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
            Assert.AreEqual("https://malopus.com.ua/image/cache/catalog/import_files/dungeon%20meshi/Moc_Cover_Підземелля%20смакоти_Том%201-700x700.png", result.cover);
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
            Assert.AreEqual("https://malopus.com.ua/image/cache/catalog/import_files/nijigahara/Moc_Cover_Голограф%20веселкового%20поля-700x700.png", result.cover);
            Assert.AreEqual(null, result.release);
            Assert.AreEqual("Mal'opus", result.publisher);
            Assert.AreEqual("Physical", result.type);
            Assert.AreEqual("978-617-8168-11-7", result.isbn);
            Assert.AreEqual(1, result.totalVolumes);
            Assert.AreEqual("oneshot", result.seriesStatus);
            Assert.AreEqual("Nijigahara Holograph", result.originalSeriesName);


        }
    }

    [TestClass]
    public class KoboTestClass
    {
        [TestMethod]
        public async Task KoboTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.kobo.com/ww/en/ebook/pretty-guardian-sailor-moon-eternal-edition-9");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Volume 9", result.title);
            Assert.AreEqual("Pretty Guardian Sailor Moon Eternal Edition", result.series);
            Assert.AreEqual("Naoko Takeuchi", result.authors);
            Assert.AreEqual(9, result.volumeNumber);
            Assert.AreEqual("https://cdn.kobo.com/book-images/b0ac3524-461c-4af5-bb05-d0f799ad87a1/353/569/90/False/pretty-guardian-sailor-moon-eternal-edition-9.jpg", result.cover);
            Assert.AreEqual("2020-11-17", result.release);
            Assert.AreEqual("Kodansha Comics", result.publisher);
            Assert.AreEqual("Digital", result.type);
            Assert.AreEqual("9781646595792", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);

        }

        [TestMethod]
        public async Task KoboTest2()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.kobo.com/ww/en/ebook/spy-x-family-family-portrait");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Spy x Family: Family Portrait", result.title);
            Assert.AreEqual("Spy x Family Novels", result.series);
            Assert.AreEqual("Aya Yajima", result.authors);
            Assert.AreEqual(-1, result.volumeNumber);
            Assert.AreEqual("https://cdn.kobo.com/book-images/a28d0839-b608-49ab-9f79-e100fef90c0f/353/569/90/False/spy-x-family-family-portrait.jpg", result.cover);
            Assert.AreEqual("2023-12-26", result.release);
            Assert.AreEqual("VIZ Media", result.publisher);
            Assert.AreEqual("Digital", result.type);

            Assert.AreEqual("9781974742691", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);


        }

        [TestMethod]
        public async Task KoboPreorderTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.kobo.com/ww/en/ebook/spy-x-family-vol-13");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Volume 13", result.title);
            Assert.AreEqual("Spy x Family", result.series);
            Assert.AreEqual("Tatsuya Endo", result.authors);
            Assert.AreEqual(13, result.volumeNumber);
            Assert.AreEqual("https://cdn.kobo.com/book-images/Images/00000000-0000-0000-0000-000000000000/353/569/90/False/empty_book_cover.jpg", result.cover);
            Assert.AreEqual("2025-01-14", result.release);
            Assert.AreEqual("VIZ Media", result.publisher);
            Assert.AreEqual("Digital", result.type);

            Assert.AreEqual("9781974753246", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);

        }

        [TestMethod]
        public async Task KoboOneShotTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.kobo.com/ww/en/ebook/a-girl-on-the-shore");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("A Girl on the Shore", result.title);
            Assert.AreEqual("A Girl on the Shore", result.series);
            Assert.AreEqual("Inio Asano", result.authors);
            Assert.AreEqual(-1, result.volumeNumber);
            Assert.AreEqual("https://cdn.kobo.com/book-images/be122b39-d133-4162-8b54-bcca043d07bb/353/569/90/False/a-girl-on-the-shore.jpg", result.cover);
            Assert.AreEqual("2016-01-19", result.release);
            Assert.AreEqual("Kodansha USA", result.publisher);
            Assert.AreEqual("Digital", result.type);

            Assert.AreEqual("9781942993766", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);

        }
    }

    [TestClass]
    public class AmazonTestClass
    {
        [TestMethod]
        public async Task AmazonTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.amazon.com/dp/B08FVLVXX6/");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Volume 1", result.title);
            Assert.AreEqual("Solo Leveling", result.series);
            Assert.AreEqual("DUBU", result.authors);
            Assert.AreEqual(1, result.volumeNumber);
            Assert.AreEqual("https://m.media-amazon.com/images/I/81y9XvteVOL._SL1500_.jpg", result.cover);
            Assert.AreEqual("2021-03-02", result.release);
            Assert.AreEqual("Yen Press", result.publisher);
            Assert.AreEqual("Digital", result.type);
            Assert.AreEqual("B08FVLVXX6", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);
        }

        [TestMethod]
        public async Task AmazonPreorderTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.amazon.com/gp/product/B0D7Z8TGNQ");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Volume 8", result.title);
            Assert.AreEqual("[Oshi no Ko]", result.series);
            Assert.AreEqual("Aka Akasaka,Mengo Yokoyari", result.authors);
            Assert.AreEqual(8, result.volumeNumber);
            Assert.AreEqual(string.Empty, result.cover);
            Assert.AreEqual("2024-11-19", result.release);
            Assert.AreEqual("Yen Press", result.publisher);
            Assert.AreEqual("Digital", result.type);
            Assert.AreEqual("B0D7Z8TGNQ", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);

        }

        [TestMethod]
        public async Task AmazonOneShotTest()
        {
            var parser = new PublisherParsersFactory().CreateParser("https://www.amazon.com/dp/B01N0LT06V");

            Assert.IsNotNull(parser);

            var result = await parser.Parse();

            Assert.IsNotNull(result);
            Assert.AreEqual("Nijigahara Holograph", result.title);
            Assert.AreEqual("Nijigahara Holograph", result.series);
            Assert.AreEqual("Inio Asano", result.authors);
            Assert.AreEqual(-1, result.volumeNumber);
            Assert.AreEqual("https://m.media-amazon.com/images/I/91lxpgZLQOL._SL1500_.jpg", result.cover);
            Assert.AreEqual("2014-03-19", result.release);
            Assert.AreEqual("Fantagraphics", result.publisher);
            Assert.AreEqual("Digital", result.type);
            Assert.AreEqual("B01N0LT06V", result.isbn);
            Assert.AreEqual(-1, result.totalVolumes);
            Assert.AreEqual(null, result.seriesStatus);
            Assert.AreEqual(null, result.originalSeriesName);


        }
    }
}
