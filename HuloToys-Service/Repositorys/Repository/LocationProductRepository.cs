using Entities.ConfigModels;
using Entities.Models;
using HuloToys_Service.ElasticSearch.DAL;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class LocationProductRepository : ILocationProductRepository
    {
        private readonly LocationProductDAL _locationProductDAL;
        private readonly IConfiguration configuration;
        public LocationProductRepository(IOptions<DataBaseConfig> dataBaseConfig, IConfiguration _configuration)
        {
            _locationProductDAL = new LocationProductDAL("", _configuration);
            configuration = _configuration;
        }

        public async Task<long> Addnew(LocationProduct model)
        {
            return await _locationProductDAL.CreateNewAsync(model);
        }
        public async Task Update(LocationProduct model)
        {
            await _locationProductDAL.UpdateAsync(model);
        }
        public async Task<LocationProduct> GetById(int id)
        {
            return await _locationProductDAL.GetById(id);
        }

        public async Task<List<LocationProduct>> GetByProductCode(string product_code)
        {
            return await _locationProductDAL.GetByProductCode(product_code);
        }

        public async Task<LocationProduct> SearchIfExists(string product_code, int group_id, int order_no)
        {
            return await _locationProductDAL.SearchIfExists(product_code, group_id, order_no);
        }
        public async Task<long> DeleteAsync(LocationProduct model)
        {
            return await _locationProductDAL.DeleteAsync(model);
        }
        public async Task<List<LocationProduct>> GetListByGroupId(int group_id)
        {
            return await _locationProductDAL.GetListByGroupId(group_id);
        }

    }
}
