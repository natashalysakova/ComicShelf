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
}
