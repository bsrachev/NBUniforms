namespace NBUniforms.Services.Statistics
{
    using System.Linq;
    using NBUniforms.Data;

    public class StatisticsService : IStatisticsService
    {
        private readonly NBUniformsDbContext data;

        public StatisticsService(NBUniformsDbContext data)
            => this.data = data;

        public StatisticsServiceModel Total()
        {
            var totalProducts = this.data.Products.Count();

            return new StatisticsServiceModel
            {
                TotalProducts = totalProducts,
            };
        }
    }
}