using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 定義資料表
        public DbSet<House> Houses { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Agent> Agents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定關聯與刪除行為 (Fluent API)

            // 1. 房仲與房源 (1對多)
            modelBuilder.Entity<House>()
                .HasOne(h => h.Agent)
                .WithMany(a => a.Houses)
                .HasForeignKey(h => h.AgentId)
                .OnDelete(DeleteBehavior.Restrict); // 避免刪除房仲時誤刪房源

            // 2. 房源與預約 (1對多)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.House)
                .WithMany(h => h.Appointments)
                .HasForeignKey(a => a.HouseId)
                .OnDelete(DeleteBehavior.Restrict); // 重要！防止誤刪有預約的房源

            // 3. 客戶與預約 (1對多)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // 刪除客戶時，連同預約一起刪除 (視需求而定)

            // 3. 設定金額的精確度 (避免 SQL Server 警告)
            modelBuilder.Entity<House>()
                .Property(h => h.Price)
                .HasPrecision(18, 2);

            // 全域篩選：預設自動過濾掉已刪除的資料
            // GetAllAsync() 自動看不到被刪除的房子
            modelBuilder.Entity<House>().HasQueryFilter(h => !h.IsDeleted);
        }
    }
}
