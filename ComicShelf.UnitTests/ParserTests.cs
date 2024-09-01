using Backend.Models.Enums;
using ComicShelf.PublisherParsers;
using Jint.Parser.Ast;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ComicShelf.UnitTests
{
    [TestClass]
    public class ParserTests
    {
        private PublisherParsersFactory _publisherParsersFactory;

        List<PublisherViewModel> _publisherViewModels = [
            new PublisherViewModel() { Name = "NashaIdea", Id = 2},
            new PublisherViewModel() { Name = "Mal'opus", Id = 3}
        ];

        public ParserTests()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

            var publisherService = new Moq.Mock<PublishersService>();
            publisherService.Setup(x => x.GetAll()).Returns(_publisherViewModels);

            _publisherParsersFactory = new PublisherParsersFactory(configuration, publisherService.Object);
        }

        [TestMethod]
        public async Task NashaIdeaPreorderTest()
        {


            var parser = _publisherParsersFactory.CreateParser("https://nashaidea.com/product/friren-tom-2/");

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


        }

        [TestMethod]
        public async Task NashaIdeaTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://nashaidea.com/product/chi-tom-10/");

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

        }

        [TestMethod]
        public async Task NashaIdeaOneShotTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://nashaidea.com/product/vitaiemo-v-koto-kafe/");

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

        }

        [TestMethod]
        public async Task MalopusTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://malopus.com.ua/manga/manga-cya-porcelyanova-lyalechka-zakohalasya-tom-5");

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

        }

        [TestMethod]
        public async Task MalopusPreorderTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://malopus.com.ua/manga/dungeon-meshi-omnibus1");

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

        }

        [TestMethod]
        public async Task MalopusOneShotTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://malopus.com.ua/manga/nijigahara-holograph");

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


        }


        
    }

    [TestClass]
    public class KoboTestClass
    {
        private PublisherParsersFactory _publisherParsersFactory;

        List<PublisherViewModel> _publisherViewModels = [
            new PublisherViewModel() { Name = "NashaIdea", Id = 2},
            new PublisherViewModel() { Name = "Mal'opus", Id = 3},
            new PublisherViewModel() { Name = "Seven Seas Entertainment", Id = 5 },
            new PublisherViewModel() { Name = "Kodansha Comics", Id = 6 },
            new PublisherViewModel() { Name = "VIZ Media", Id = 7 }
        ];

        //[TestInitialize]
        //public async Task Startup()
        //{
        //    await Task.Delay(10000);
        //}


        public KoboTestClass()
        {
                IServiceCollection services = new ServiceCollection();
                var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

                var publisherService = new Moq.Mock<PublishersService>();
                publisherService.Setup(x => x.GetAll()).Returns(_publisherViewModels);

                _publisherParsersFactory = new PublisherParsersFactory(configuration, publisherService.Object);
            
        }

        [TestMethod]
        public async Task KoboTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://www.kobo.com/ww/en/ebook/pretty-guardian-sailor-moon-eternal-edition-9");

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
        }

        [TestMethod]
        public async Task KoboPreorderTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://www.kobo.com/ww/en/ebook/spy-x-family-vol-13");

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


        }

        [TestMethod]
        public async Task KoboOneShotTest()
        {
            var parser = _publisherParsersFactory.CreateParser("https://www.kobo.com/ww/en/ebook/a-girl-on-the-shore");

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


        }
    }
}
