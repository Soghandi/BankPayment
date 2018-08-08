using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Adin.BankPayment.Service
{
    public class BankRepository : IRepository<Bank>
    {
        private BankPaymentContext context;
        private DbSet<Bank> entity;
        public BankRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<Bank>();
        }

        public async Task<Bank> Get(Guid id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Bank>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task Add(Bank obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(Bank obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            Bank Bank = await Get(id);
            entity.Remove(Bank);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Bank>> GetAllBy(Expression<Func<Bank, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }
        public async Task<Bank> GetFirstBy(Expression<Func<Bank, bool>> predicate)
        {
            return await entity.Where(predicate).FirstOrDefaultAsync();
        }

        public Task DeletePermanently(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
