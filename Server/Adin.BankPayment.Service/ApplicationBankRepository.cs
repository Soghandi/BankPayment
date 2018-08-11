using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Adin.BankPayment.Service
{
    public class ApplicationBankRepository : IRepository<ApplicationBank>
    {
        private BankPaymentContext context;
        private DbSet<ApplicationBank> entity;
        public ApplicationBankRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<ApplicationBank>();
        }

        public async Task<ApplicationBank> Get(Guid id)
        {
            return await entity.Include(x => x.Bank).Include(x => x.Application).Include(x => x.ApplicationBankParams).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ApplicationBank>> GetAll()
        {
            return await entity.Include(x => x.Bank).Include(x => x.Application).Include(x => x.ApplicationBankParams).ToListAsync();
        }

        public async Task Add(ApplicationBank obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(ApplicationBank obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var obj = await Get(id);
            obj.IsDeleted = true;
            await Update(obj);
        }

        public async Task DeletePermanently(Guid id)
        {
            await entity.Where(x => x.Id == id).DeleteAsync();
        }


        public async Task<IEnumerable<ApplicationBank>> GetAllBy(Expression<Func<ApplicationBank, bool>> predicate)
        {
            return await entity.Include(x => x.Bank).Include(x => x.Application).Include(x => x.ApplicationBankParams).Where(predicate).ToListAsync();
        }
        public async Task<ApplicationBank> GetFirstBy(Expression<Func<ApplicationBank, bool>> predicate)
        {
            return await entity.Include(x => x.Bank).Include(x => x.Application).Include(x => x.ApplicationBankParams).Where(predicate).FirstOrDefaultAsync();
        }
    }
}
