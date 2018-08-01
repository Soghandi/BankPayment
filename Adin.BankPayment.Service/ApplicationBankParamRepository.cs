using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Adin.BankPayment.Service
{
    public class ApplicationBankParamRepository : IRepository<ApplicationBankParam>
    {
        private BankPaymentContext context;
        private DbSet<ApplicationBankParam> entity;
        public ApplicationBankParamRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<ApplicationBankParam>();
        }

        public async Task<ApplicationBankParam> Get(Guid id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ApplicationBankParam>> GetByApplicationBank(Guid id)
        {
            return await entity.Where(x => x.ApplicationBankId == id).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationBankParam>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task Add(ApplicationBankParam obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(ApplicationBankParam obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            ApplicationBankParam ApplicationBankParam = await Get(id);
            entity.Remove(ApplicationBankParam);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ApplicationBankParam>> GetAllBy(Expression<Func<ApplicationBankParam, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }
        public async Task<ApplicationBankParam> GetFirstBy(Expression<Func<ApplicationBankParam, bool>> predicate)
        {
            return await entity.Where(predicate).FirstOrDefaultAsync();
        }

        public Task DeletePermanently(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
