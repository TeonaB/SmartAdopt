namespace SmartAdopt.Models
{
    public class RaportStatisticiViewModel
    {
        public int TotalUseri { get; set; }
        public int UseriCuProfilCompletat { get; set; }
        public int UseriCuBlog { get; set; }
        public int TotalRecomandari { get; set; }
        public List<dynamic> TopAnimaleRecomandate { get; set; }
        public int ComenziDinRecomandari { get; set; }
        public int TotalAnimale { get; set; }
        public List<dynamic> AnimalePeSpecie { get; set; }
        public dynamic AverageAtribute { get; set; }
        public List<Animal> AnimaleNerecomandate { get; set; }
        public int TotalAnimaleAdoptate { get; set; }
        public List<dynamic> StatusComenzi { get; set; }

    }
}
