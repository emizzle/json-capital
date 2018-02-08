using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JSONCapital.Data;
using JSONCapital.Data.Models;
using Microsoft.Extensions.Logging;

namespace JSONCapital.Services.Repositories
{
    public interface ITradesRepository
    {
        Task AddTradeAsync(Trade trade);
        Task<IEnumerable<Trade>> GetAllTradesAsync();
        Task InsertTradeAsync(Trade trade);
        Task SaveChangesAsync();
    }

    public class TradesRepository : ITradesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;

        public TradesRepository(ApplicationDbContext dbContext, ILogger<TradesRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Adds trade to db context, then saves the DB context. To add without saving, use the <see cref="AddTradeAsync"/> method 
        /// </summary>
        /// <returns>The trade async.</returns>
        /// <param name="trade">Trade.</param>
        public async Task InsertTradeAsync(Trade trade)
        {
            _logger.LogTrace($"Adding trade to database with trade id of {trade.TradeID}");
            await _dbContext.Trades.AddAsync(trade);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds the trade to the db context without saving the changes.
        /// </summary>
        /// <returns>The trade async.</returns>
        /// <param name="trade">Trade.</param>
        public async Task AddTradeAsync(Trade trade)
        {
            _logger.LogTrace($"Adding trade to database with trade id of {trade.TradeID}");
            await _dbContext.Trades.AddAsync(trade);
        }

        /// <summary>
        /// Gets all trade data async.
        /// </summary>
        /// <returns>All trade data async.</returns>
        public async Task<IEnumerable<Trade>> GetAllTradesAsync()
        {
            _logger.LogTrace($"Getting all trades from database");
            return await _dbContext.Trades.ToListAsync();
        }

        /// <summary>
        /// Saves the changes to the db context.
        /// </summary>
        /// <returns>The changes async.</returns>
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
