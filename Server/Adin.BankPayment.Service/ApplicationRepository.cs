using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Adin.BankPayment.Service
{
    public class ApplicationRepository : IRepository<Application>
    {
        private readonly BankPaymentContext context;
        private readonly DbSet<Application> entity;

        public ApplicationRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<Application>();
        }

        public async Task<Application> Get(Guid id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Application>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task Add(Application obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(Application obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var obj = await Get(id);
            obj.IsDeleted = true;
            await Update(obj);
        }

        public async Task<IEnumerable<Application>> GetAllBy(Expression<Func<Application, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }

        public async Task<Application> GetFirstBy(Expression<Func<Application, bool>> predicate)
        {
            return await entity.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task DeletePermanently(Guid id)
        {
            await entity.Where(x => x.Id == id).DeleteAsync();
        }

        public async Task<Application> GetByPublicKey(string publicKey)
        {
            return await entity.FirstOrDefaultAsync(x => x.PublicKey == publicKey);
        }
    }
}