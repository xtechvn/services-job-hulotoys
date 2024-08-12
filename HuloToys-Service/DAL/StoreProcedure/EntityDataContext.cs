using HuloToys_Service.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HuloToys_Service.DAL.StoreProcedure
{
    public class EntityDataContext : DataMSContext
    {
        private readonly string _connection;

        public EntityDataContext(string connection)
        {
            _connection = connection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connection);
            }
        }
    }
}
