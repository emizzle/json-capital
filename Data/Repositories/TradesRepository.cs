using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSONCapital.Data;
using JSONCapital.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace JSONCapital.Data.Repositories
{
    public interface ITradesRepository
    {
        Task AddTradeAsync(Trade trade);
        Task<IEnumerable<Trade>> GetAllTradesAsync();
        Task InsertTradeAsync(Trade trade);
        Task SaveChangesAsync();
        Task AddOrUpdateTradeNoSaveAsync(Trade trade);
        Task<Trade> FindTradeAsync(int tradeKey);
        Task<Trade> FindTradeByCoinTrackingTradeIdAsync(int coinTrackingTradeId);
        Task DeleteTradesNoSaveAsync(IEnumerable<int> coinTrackingTradeIds);
        Task UpdateTradeAsync(Trade trade);
        void UpdateTradeNoSaveAsync(Trade trade);
        IEnumerable<int> GetAllCoinTrackingTradeIds();
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
        /// Asynchronously updates the trade and saves the changes to the context.
        /// </summary>
        /// <returns>The trade async.</returns>
        /// <param name="trade">Trade.</param>
        public async Task UpdateTradeAsync(Trade trade)
        {
            UpdateTradeNoSaveAsync(trade);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the trade without saving changes to the context.
        /// </summary>
        /// <returns>The trade with updated values.</returns>
        /// <param name="trade">Trade.</param>
        public void UpdateTradeNoSaveAsync(Trade trade)
        {
            _logger.LogTrace($"Updating trade in database with trade id of {trade.TradeID}");

            //trackedEntity.BuyAmount = untrackedEntityWithNewVals.BuyAmount;
            //trackedEntity.BuyCurrency = untrackedEntityWithNewVals.BuyCurrency;
            //trackedEntity.CoinTrackingTradeID = untrackedEntityWithNewVals.CoinTrackingTradeID;
            //trackedEntity.Comment = untrackedEntityWithNewVals.Comment;
            //trackedEntity.Exchange = untrackedEntityWithNewVals.Exchange;
            //trackedEntity.FeeAmount = untrackedEntityWithNewVals.FeeAmount;
            //trackedEntity.FeeCurrency = untrackedEntityWithNewVals.FeeCurrency;
            //trackedEntity.Group = untrackedEntityWithNewVals.Group;
            //trackedEntity.ImportedFrom = untrackedEntityWithNewVals.ImportedFrom;
            //trackedEntity.ImportedTime = untrackedEntityWithNewVals.ImportedTime;
            //trackedEntity.ImportedTradeID = untrackedEntityWithNewVals.ImportedTradeID;
            //trackedEntity.SellAmount = untrackedEntityWithNewVals.SellAmount;
            //trackedEntity.SellCurrency = untrackedEntityWithNewVals.SellCurrency;
            //trackedEntity.Time = untrackedEntityWithNewVals.Time;
            //trackedEntity.TradeType = untrackedEntityWithNewVals.TradeType;

            _dbContext.Trades.Update(trade);
            //await _dbContext.Trades.UpdateAsync(t => new Trade(){
            //    BuyAmount = trade.BuyAmount,
            //    BuyCurrency = trade.BuyCurrency,
            //    CoinTrackingTradeID = trade.CoinTrackingTradeID,
            //    Comment = trade.Comment,
            //    Exchange = trade.Exchange,
            //    FeeAmount = trade.FeeAmount,
            //    FeeCurrency = trade.FeeCurrency,
            //    Group = trade.Group,
            //    ImportedFrom = trade.ImportedFrom,
            //    ImportedTime = trade.ImportedTime,
            //    ImportedTradeID = trade.ImportedTradeID,
            //    SellAmount = trade.SellAmount,
            //    SellCurrency = trade.SellCurrency,
            //    Time = trade.Time,
            //    TradeID = trade.TradeID,
            //    TradeType = trade.TradeType
            //}); // update the whole object
        }

        /// <summary>
        /// If exists, the trade is updated with the values of the Trade passed. The trade is 
        /// queried for by it's primary key first, and if not found, is queried by the unique
        /// index CoinTrackingTradeID.
        /// If the trade doesn't exist, it is added.
        /// NOTE: The DBContext changes are NOT saved, and much be saved using the SaveChangesAsync
        /// method.
        /// </summary>
        /// <returns>The or update trade async.</returns>
        /// <param name="trade">Trade.</param>
        public async Task AddOrUpdateTradeNoSaveAsync(Trade trade)
        {
            var existingTrade = await _dbContext.Trades.AsNoTracking().SingleOrDefaultAsync(t => t.TradeID == trade.TradeID);
            if (existingTrade == null)
            {
                existingTrade = await _dbContext.Trades.AsNoTracking().SingleOrDefaultAsync(t => t.CoinTrackingTradeID == trade.CoinTrackingTradeID);//FindTradeByCoinTrackingTradeIdAsync(trade.CoinTrackingTradeID);
            }
            if (existingTrade != null)
            {
                _logger.LogTrace($"Updating trade in database with trade id of {trade.TradeID}");
                trade.TradeID = existingTrade.TradeID;
                UpdateTradeNoSaveAsync(trade);
            }
            else
            {
                await AddTradeAsync(trade);
            }
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

        public IEnumerable<int> GetAllCoinTrackingTradeIds()
        {
            _logger.LogTrace($"Getting all trades' CoinTrackingTradeIDs from database");
            return _dbContext.Trades.Select(t => t.CoinTrackingTradeID).AsEnumerable();
        }

        /// <summary>
        /// Finds a trade by it's primary key TradeID.
        /// </summary>
        /// <returns>All trade data async.</returns>
        public async Task<Trade> FindTradeAsync(int tradeId)
        {
            _logger.LogTrace($"Finding trade by primary key '{tradeId}'");
            return await _dbContext.Trades.FindAsync(tradeId);
        }

        /// <summary>
        /// Finds a trade by it's primary key TradeID.
        /// </summary>
        /// <returns>All trade data async.</returns>
        public async Task<Trade> FindTradeByCoinTrackingTradeIdAsync(int coinTrackingTradeId)
        {
            _logger.LogTrace($"Finding trade by primary key '{coinTrackingTradeId}'");
            return await _dbContext.Trades.FirstOrDefaultAsync(t => t.CoinTrackingTradeID == coinTrackingTradeId);
        }

        public async Task DeleteTradesNoSaveAsync(IEnumerable<int> coinTrackingTradeIds)
        {
            await _dbContext.Trades.Where<Trade>(t => coinTrackingTradeIds.Contains(t.CoinTrackingTradeID)).DeleteAsync();
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
