

namespace SmartAdopt.Models
{
    public class StatisticsReportViewModel
    {
        public int TotalRegisteredUsers { get; set; }
        public int UsersWithCompletedProfile { get; set; }
        public int UsersInteractedWithBlog { get; set; }
        public int TotalRecommendationsGenerated { get; set; }
        public List<dynamic> TopRecommendedAnimals { get; set; }
        public int OrdersFromRecommendations { get; set; }
        public int TotalAnimals { get; set; }
        public List<dynamic> AnimalsBySpecies { get; set; }
        public dynamic AverageAttributes { get; set; }
        public List<Animal> AnimalsNeverRecommended { get; set; }
        public int TotalAnimalsAdopted { get; set; }
        public List<dynamic> OrderStatuses { get; set; }

    }
}
