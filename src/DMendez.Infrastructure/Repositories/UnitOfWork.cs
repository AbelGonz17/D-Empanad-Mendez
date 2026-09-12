using System;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Interfaces;
using DMendez.Infrastructure.Contex;

namespace DMendez.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DMendezDbContext _context;
        private bool _disposed;

        public UnitOfWork(DMendezDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
