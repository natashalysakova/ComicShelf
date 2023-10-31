using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.UnitTests
{
    [TestClass]
    public class PublisherMappingTest : BasicTest
    {
        public PublisherMappingTest():base(new MapperConfiguration(c => { c.AddMaps(typeof(Profiles.PublisherProfile)); }))
        {
            
        }
        [TestMethod]
        public override void TestCreateModel()
        {
            var model = new PublisherCreateModel()
            {
                Name = "Test",
                CountryId = 42
            };

            var entity = _mapper.Map<Publisher>(model);

            Assert.IsNotNull(entity);
            Assert.AreEqual(0, entity.Id);
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.CountryId, entity.CountryId);
        }

        [TestMethod]
        public override void TestUpdateModel()
        {
            var model = new PublisherUpdateModel()
            {
                Id = 1,
                Name = "Test",
                CountryId = 42
            };

            var entity = _mapper.Map<Publisher>(model);

            Assert.IsNotNull(entity);
            Assert.AreEqual(model.Id, entity.Id);
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.CountryId, entity.CountryId);
        }

        [TestMethod]
        public override void TestViewModel()
        {
            var entity = DataSet.Publisher;
            var viewModel = _mapper.Map<PublisherViewModel>(entity);

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(entity.Id, viewModel.Id);
            Assert.AreEqual(entity.Name, viewModel.Name);
            Assert.AreEqual(entity.CountryId, viewModel.CountryId);
            Assert.AreEqual(entity.Series.Count, viewModel.SeriesCount);
            Assert.AreEqual(entity.Country.Name, viewModel.CountryName);
            Assert.AreEqual(entity.Country.FlagPNG, viewModel.CountryFlagPNG);
            Assert.IsTrue(entity.Series.Select(x=>x.Name).Distinct().SequenceEqual(viewModel.Series));
        }
    }
}