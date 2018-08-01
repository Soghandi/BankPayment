using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Adin.BankPayment.Service
{
    public class TransactionRepository : IRepository<Transaction>
    {
        private BankPaymentContext context;
        private DbSet<Transaction> entity;
        public TransactionRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<Transaction>();
        }

        public async Task<Transaction> Get(Guid id)
        {
            return await entity.Include(x => x.Bank).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task Add(Transaction obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(Transaction obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            Transaction transaction = await Get(id);
            transaction.IsDeleted = true;
            await Update(transaction);
        }

        public async Task DeletePermanently(Guid id)
        {
            await entity.Where(x => x.Id == id).DeleteAsync();           
        }

        public async Task<IEnumerable<Transaction>> GetAllBy(Expression<Func<Transaction, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }
        public async Task<Transaction> GetFirstBy(Expression<Func<Transaction, bool>> predicate)
        {
            return await entity.Where(predicate).FirstOrDefaultAsync();
        }
    }
}
