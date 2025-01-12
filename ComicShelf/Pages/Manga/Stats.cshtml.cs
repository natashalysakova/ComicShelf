using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using System.Linq;

namespace ComicShelf.Pages.Manga
{
    public class StatsModel : PageModel
    {
        private readonly VolumeService volumeService;

        public IList<SelectListItem> Years { get; set; }
        public IList<int> PreorderedByMonth { get; set; }
        public IList<int> PurchsedByMonth { get; set; }
        public IList<int> CollectionGrowth { get; set; }

        public IEnumerable<Volume> Volumes { get; set; }

        public List<string> MonthLables { get; set; }

        public Filter Filters { get; set; }

        public StatsModel(VolumeService volumeService)
        {
            this.volumeService = volumeService;
            MonthLables = new List<string>();
            PurchsedByMonth = new List<int>();
            PreorderedByMonth = new List<int>();
            CollectionGrowth = new List<int>();
        }

        public void OnGet()
        {
            Volumes = volumeService.GetAllEntities().Include(x=>x.Series).OrderBy(x=>x.PurchaseDate).AsEnumerable();

            if (!Volumes.Any())
                return;

            Years = Volumes
                .Where(v => v.PreorderDate.HasValue)
                .Select(v => v.PreorderDate!.Value.Year)
                .Union(
                    Volumes
                    .Where(x => x.PurchaseDate.HasValue)
                    .Select(x => x.PurchaseDate!.Value.Year))
                .Distinct()
                .OrderByDescending(x => x)
                .Select(x => new SelectListItem(x.ToString(), x.ToString())).ToList();
            Years.Insert(0, new SelectListItem("All", "All", true));

            //PreorderedByMonth = volumes.Where(x => x.PreorderDate.HasValue).OrderBy(x => x.PreorderDate).AsEnumerable().GroupBy(x => FormatKey(x.PreorderDate!.Value)).ToDictionary(x => x.Key, x => x.Count());
            //PurchsedByMonth = volumes.Where(x => x.PurchaseDate.HasValue).OrderBy(x => x.PurchaseDate).AsEnumerable().GroupBy(x => FormatKey(x.PurchaseDate!.Value)).ToDictionary(x => x.Key, x => x.Count());
            //CollectionGrowth = new Dictionary<string, int>();


            var dateToAdd = Volumes
                .Where(x => x.PreorderDate.HasValue || x.PurchaseDate.HasValue)
                .Select(x => x.PreorderDate.HasValue ? x.PreorderDate.Value : x.PurchaseDate!.Value)
                .OrderBy(x => x)
                .First();
            do
            {
                string date = FormatKey(dateToAdd);
                MonthLables.Add(date);

                PurchsedByMonth.Add(Volumes.Where(x => x.PurchaseDate.HasValue && x.PurchaseDate!.Value.Year == dateToAdd.Year && x.PurchaseDate!.Value.Month == dateToAdd.Month).Count());
                PreorderedByMonth.Add(Volumes.Where(x => x.PreorderDate.HasValue && x.PreorderDate <= x.PurchaseDate && x.PreorderDate!.Value.Year == dateToAdd.Year && x.PreorderDate!.Value.Month == dateToAdd.Month).Count());

                dateToAdd = dateToAdd.AddMonths(1);

            } while (dateToAdd <= DateTime.Today.AddMonths(1));


            for (int i = 0; i < PurchsedByMonth.Count; i++)
            {
                if(i == 0)
                {
                    CollectionGrowth.Add(PurchsedByMonth.ElementAt(i));
                    continue;
                }
                CollectionGrowth.Add(CollectionGrowth[i - 1] + PurchsedByMonth[i]);
            }



            



        }

        public string FormatKey(DateTime date)
        {
            return $"{date.Year}/{date.Month}";
        }
    }

    public class Filter
    {
        public int Year { get; set; }
    }
}
