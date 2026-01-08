using Microsoft.EntityFrameworkCore.Storage;
using Promotions.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Infrastructure.Persistence
{
    public class EfAtomicTransaction : IAtomicTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfAtomicTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
