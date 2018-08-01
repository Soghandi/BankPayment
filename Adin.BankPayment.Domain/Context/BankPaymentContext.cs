using Adin.BankPayment.Domain.Cache;
using Adin.BankPayment.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Adin.BankPayment.Domain.Context
{
    public class BankPaymentContext : DbContext
    {

        public BankPaymentContext(DbContextOptions<BankPaymentContext> options) : base(options)
        {
            Init();
        }

        public BankPaymentContext()
        {
            Init();
        }
        protected internal virtual void Init()
        {
          //  this.InitializeDynamicFilters();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionStringGetter.ConStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Transaction>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<Bank>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Bank>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<Application>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Application>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<ApplicationBank>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationBank>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<ApplicationBankParam>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationBankParam>().HasQueryFilter(p => p.IsDeleted == false);


        }
      
        //public DbSet<Application> Applications { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<ApplicationBank> ApplicationBanks { get; set; }

        public DbSet<ApplicationBankParam> ApplicationBankParams { get; set; }

        
    }

   

}

