using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSONCapital.Data;
using JSONCapital.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JSONCapital.Controllers
{
    public interface ITradesController
    {
        void Delete(int id);
        Task<IEnumerable<Trade>> GetAsync();
        string Get(int id);
        void Post([FromBody] string value);
        void Put(int id, [FromBody] string value);
    }

    [Route("api/[controller]")]
    public class TradesController : Controller, ITradesController
    {
        private ApplicationDbContext _dbContext;
        private ILogger<TradesController> _logger;

        public TradesController(ApplicationDbContext dbContext, ILogger<TradesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<Trade>> GetAsync()
        {
            return await _dbContext.Trades.ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
