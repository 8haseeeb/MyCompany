using System;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Common.Interfaces
{
    public interface IAtomicTransaction : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
