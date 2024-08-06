using DAL.Generic;
using Entities.Models;
using HuloToys_Service.DAL.StoreProcedure;
using HuloToys_Service.ElasticSearch.LocationProduct;
using HuloToys_Service.ElasticSearch.NewEs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
    public class LocationProductDAL : GenericService<LocationProduct>
    {
        private readonly IConfiguration configuration;
        public LocationProductESService locationProductESService;
        public LocationProductDAL(string connection, IConfiguration _configuration) : base(connection)
        {
            locationProductESService = new LocationProductESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            configuration = _configuration;
        }
        public async Task<LocationProduct> GetById(int id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.LocationProduct.AsNoTracking().FirstOrDefaultAsync(x => x.LocationProductId == id);
                    if (detail != null)
                    {
                        return await detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<LocationProduct>> GetByProductCode(string product_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.LocationProduct.AsNoTracking().Where(x => x.ProductCode == product_code).ToListAsync();
                    if (detail != null)
                    {
                        return await detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<LocationProduct> SearchIfExists(string product_code, int group_id, int order_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.LocationProduct.AsNoTracking().Where(x => x.ProductCode == product_code && x.GroupProductId == group_id).FirstOrDefaultAsync();
                    if (detail != null)
                    {
                        return await detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<LocationProduct>> GetListByGroupId(int group_id)
        {
            try
            {
                var data = locationProductESService.GetListByGroupId(group_id);
                return data;
            }
            catch (Exception)
            {
            }
            return null;

        }
        public async Task<long> CreateNewAsync(LocationProduct entity)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    await _DbContext.Set<LocationProduct>().AddAsync(entity);
                    await _DbContext.SaveChangesAsync();
                    return Convert.ToInt64(entity.GetType().GetProperty("LocationProductId").GetValue(entity, null));
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<long> DeleteAsync(LocationProduct entity)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Set<LocationProduct>().Remove(entity);
                    await _DbContext.SaveChangesAsync();
                    return Convert.ToInt64(entity.GetType().GetProperty("LocationProductId").GetValue(entity, null));
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
