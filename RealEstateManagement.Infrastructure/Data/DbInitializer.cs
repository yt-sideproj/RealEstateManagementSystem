using RealEstateManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // 1. 確保資料庫已建立 (這行如果通過，代表連線成功)
            context.Database.EnsureCreated();

            // 2. 檢查是否已經有房源資料
            if (context.Houses.Any())
            {
                return;   // 資料庫已初始化，不需動作
            }

            // 3. 建立假資料
            var houses = new House[]
            {
                new House { Title = "大安區景觀豪宅", Address = "台北市大安區信義路三段", Price = 8500, SquareMeters = 65, Description = "近大安森林公園，高樓層視野佳", IsActive = true },
                new House { Title = "內湖科技園區美寓", Address = "台北市內湖區港墘路", Price = 2300, SquareMeters = 30, Description = "適合上班族，交通便利", IsActive = true },
                new House { Title = "信義區捷運小套房", Address = "台北市信義區永吉路", Price = 1250, SquareMeters = 12, Description = "收租自用兩相宜", IsActive = true },
                new House { Title = "板橋新站特區", Address = "新北市板橋區縣民大道", Price = 3600, SquareMeters = 45, Description = "三鐵共構，生活機能強", IsActive = true },
                new House { Title = "(已售出) 中山區美宅", Address = "台北市中山區林森北路", Price = 1800, SquareMeters = 25, Description = "投資客首選", IsActive = false }
            };

            // 4. 寫入資料庫
            context.Houses.AddRange(houses);
            context.SaveChanges();
        }
    }
}
