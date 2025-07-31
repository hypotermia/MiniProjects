using Microsoft.EntityFrameworkCore;
using TrxServices.Models;
using TrxServices.Interfaces;

namespace TrxServices.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TrxServicesContext _dbContext;
        public TransactionRepository(TrxServicesContext context)
        {
            _dbContext = context;
        }
        public async Task AddTransactionAsync(Transaction trx)
        {
            var newTrx = new Transaction
            {
                Id = Guid.NewGuid(),
                ProductId = trx.ProductId,
                TotalPerTrx = trx.TotalPerTrx,
                Quantity = trx.Quantity,
                CreatedDate = DateTime.Now
            };
            _dbContext.Transactions.Add(newTrx);
            await _dbContext.SaveChangesAsync();
            var newReport = new Reporting
            {
                Id = Guid.NewGuid(),
                TransactionId= newTrx.Id,
                TotalPayment= newTrx.TotalPerTrx,
                CreatedDate = DateTime.Now
            };
            _dbContext.Reportings.Add(newReport);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Guid uId)
        {
            var trx = await _dbContext.Transactions.FindAsync(uId);
            if (trx == null)
            {
                return;
            }
            _dbContext.Transactions.Remove(trx);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<DetailTransaction>> GetTransactionAsync()
        {
            var _trx = await _dbContext.DetailTransactions.ToListAsync();
            return _trx.Select(transaction => new DetailTransaction
            {
                Id = transaction.Id,
                ProductId = transaction.ProductId,
                ProductsName = transaction.ProductsName,
                TotalPerTrx = transaction.TotalPerTrx,
                Quantity = transaction.Quantity,
                CreatedDate = transaction.CreatedDate

            });
        }

        public async Task<DetailTransaction> GetTransactionByIdAsync(Guid uId)
        {
            var trx = await _dbContext.DetailTransactions.FirstOrDefaultAsync(u => u.Id == uId);

            if (trx == null)
            {
                return null;
            }

            return new DetailTransaction
            {
                Id = trx.Id,
                ProductId = trx.ProductId,
                ProductsName = trx.ProductsName,
                TotalPerTrx = trx.TotalPerTrx,
                Quantity = trx.Quantity,
                CreatedDate = trx.CreatedDate
            };
        }

        public async Task UpdateTransactionAsync(Guid uId, Transaction trx)
        {
            var existingtrx = await _dbContext.Transactions.FindAsync(uId);
            if (existingtrx == null)
            {
                return;
            }
            existingtrx.ProductId = trx.ProductId;
            existingtrx.TotalPerTrx = trx.TotalPerTrx;
            existingtrx.Quantity = trx.Quantity;

            _dbContext.Transactions.Update(existingtrx);
            await _dbContext.SaveChangesAsync();
        }
    }
}
