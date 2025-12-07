using RealEstateManagement.Core.Models;

namespace RealEstateManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // 1. 確保資料庫已建立 (這行如果通過，代表連線成功)
            context.Database.EnsureCreated();

            // 1. 建立房仲 (Agents) - 後台管理者
            if (!context.Agents.Any())
            {
                var agents = new Agent[]
                {
                    new Agent {
                        Name = "系統管理員",
                        EmployeeCode = "admin",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin") // 密碼: admin
                    },
                    new Agent {
                        Name = "李大仁",
                        EmployeeCode = "A001",
                        Password = BCrypt.Net.BCrypt.HashPassword("1234")
                    }
                };
                context.Agents.AddRange(agents);
                context.SaveChanges();
            }

            // 2. 建立客戶 (Customers) - App 端使用者
            if (!context.Customers.Any())
            {
                var customers = new Customer[]
                {
                    new Customer {
                        Name = "陳小美", Email = "user1@test.com", Phone = "0912345678",
                        Password = BCrypt.Net.BCrypt.HashPassword("1111")
                    },
                    new Customer {
                        Name = "張志明", Email = "user2@test.com", Phone = "0987654321",
                        Password = BCrypt.Net.BCrypt.HashPassword("1111")
                    }
                };
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }

            // 3. 建立房源 (Houses) - 需綁定房仲
            if (!context.Houses.Any())
            {
                var agent1 = context.Agents.First(a => a.EmployeeCode == "admin");
                var agent2 = context.Agents.First(a => a.EmployeeCode == "A001");

                var houses = new House[]
                {
                    new House {
                        Title = "大安區景觀豪宅",
                        City = "台北市", District = "大安區", DetailAddress = "信義路三段",
                        Price = 8500, SquareMeters = 65, Description = "近大安森林公園，高樓層視野佳",
                        IsActive = true, IsDeleted = false,  AgentId = agent2.Id
                    },
                    new House {
                        Title = "內湖科技園區美寓",
                        City = "台北市", District = "內湖區", DetailAddress = "港墘路",
                        Price = 2300, SquareMeters = 30, Description = "適合上班族，交通便利",
                        IsActive = true, IsDeleted = false,  AgentId = agent2.Id
                    },
                    new House {
                        Title = "信義區捷運小套房",
                        City = "台北市", District = "信義區", DetailAddress = "永吉路",
                        Price = 1250, SquareMeters = 12, Description = "收租自用兩相宜",
                        IsActive = true, IsDeleted = false, AgentId = agent2.Id
                    },
                    new House {
                        Title = "板橋新站特區",
                        City = "新北市", District = "板橋區", DetailAddress = "縣民大道",
                        Price = 3600, SquareMeters = 45, Description = "三鐵共構，生活機能強",
                        IsActive = true, IsDeleted = false,  AgentId = agent2.Id
                    },
                    new House {
                        Title = "中山區美宅",
                        City = "台北市", District = "中山區", DetailAddress = "林森北路",
                        Price = 1800, SquareMeters = 25, Description = "投資客首選",
                        IsActive = false, IsDeleted = false,  AgentId = agent2.Id
                    }
                };
                context.Houses.AddRange(houses);
                context.SaveChanges();
            }
        }
    }
}
