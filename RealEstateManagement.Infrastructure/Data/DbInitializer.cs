using RealEstateManagement.Core.Enums;
using RealEstateManagement.Core.Models;

namespace RealEstateManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // 確保資料庫已建立
            context.Database.EnsureCreated();

            // ---------------------------------------------------------
            // 1. 建立房仲 (Agents)
            // ---------------------------------------------------------
            if (!context.Agents.Any())
            {
                var agents = new Agent[]
                {
                    // 房仲 A：王小明 (主攻台北市中心與新北精華區)
                    new Agent {
                        Name = "王小明",
                        EmployeeCode = "A001",
                        Password = BCrypt.Net.BCrypt.HashPassword("1234")
                    },
                    // 房仲 B：李大仁 (主攻內湖、南港、信義)
                    new Agent {
                        Name = "李大仁",
                        EmployeeCode = "A002",
                        Password = BCrypt.Net.BCrypt.HashPassword("1234")
                    }
                };
                context.Agents.AddRange(agents);
                context.SaveChanges();
            }

            // ---------------------------------------------------------
            // 2. 建立客戶 (Customers)
            // ---------------------------------------------------------
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
                    },
                    new Customer {
                        Name = "林大戶", Email = "vip@test.com", Phone = "0900888888",
                        Password = BCrypt.Net.BCrypt.HashPassword("1111")
                    }
                };
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }

            // ---------------------------------------------------------
            // 3. 建立房源 (Houses) - 擴充至 12 筆
            // ---------------------------------------------------------
            if (!context.Houses.Any())
            {
                var agent1 = context.Agents.First(a => a.EmployeeCode == "A001"); // 王小明
                var agent2 = context.Agents.First(a => a.EmployeeCode == "A002"); // 李大仁

                var houses = new House[]
                {
                    // --- Agent 1 (王小明) 的案件 ---
                    new House {
                        Title = "大安森林公園景觀豪宅",
                        City = "台北市", District = "大安區", DetailAddress = "信義路三段147巷12弄5號8樓",
                        Price = 8800, SquareMeters = 65,
                        Description = "面公園第一排，永久棟距，百萬裝潢，附雙坡平車位。",
                        IsActive = true, IsDeleted = false, AgentId = agent1.Id
                    },
                    new House {
                        Title = "板橋新站特區三房",
                        City = "新北市", District = "板橋區", DetailAddress = "縣民大道二段88號15樓",
                        Price = 3600, SquareMeters = 45,
                        Description = "三鐵共構，生活機能強，新板特區核心地段，高樓層視野佳。",
                        IsActive = true, IsDeleted = false, AgentId = agent1.Id
                    },
                    new House {
                        Title = "中山區投資套房",
                        City = "台北市", District = "中山區", DetailAddress = "林森北路107巷20號3樓",
                        Price = 1200, SquareMeters = 15,
                        Description = "投資客首選，收租穩定，全新裝潢，管線已更新。",
                        IsActive = false, IsDeleted = false, AgentId = agent1.Id
                    },
                    new House {
                        Title = "松山區河景景觀宅",
                        City = "台北市", District = "松山區", DetailAddress = "塔悠路200號10樓",
                        Price = 4500, SquareMeters = 50,
                        Description = "直面基隆河景，採光通風極佳，近饒河夜市與捷運松山站。",
                        IsActive = true, IsDeleted = false, AgentId = agent1.Id
                    },
                    new House {
                        Title = "新店碧潭水岸宅",
                        City = "新北市", District = "新店區", DetailAddress = "環河路50號6樓",
                        Price = 2800, SquareMeters = 38,
                        Description = "碧潭風景區旁，休閒氛圍濃厚，適合退休養老。",
                        IsActive = true, IsDeleted = false, AgentId = agent1.Id
                    },
                    new House {
                        Title = "中和四號公園公寓",
                        City = "新北市", District = "中和區", DetailAddress = "安平路10巷5號2樓",
                        Price = 1580, SquareMeters = 28,
                        Description = "正對四號公園，圖書館旁，稀有低樓層公寓釋出。",
                        IsActive = true, IsDeleted = false, AgentId = agent1.Id
                    },

                    // --- Agent 2 (李大仁) 的案件 ---
                    new House {
                        Title = "內湖科學園區美寓",
                        City = "台北市", District = "內湖區", DetailAddress = "港墘路221巷5號4樓",
                        Price = 2350, SquareMeters = 30,
                        Description = "適合科技新貴，步行至捷運站僅5分鐘，生活機能便利。",
                        IsActive = true, IsDeleted = false, AgentId = agent2.Id
                    },
                    new House {
                        Title = "信義區捷運小豪宅",
                        City = "台北市", District = "信義區", DetailAddress = "永吉路30巷15號2樓",
                        Price = 1350, SquareMeters = 12,
                        Description = "近市府轉運站，美食街精華地段，低總價入手信義門牌。",
                        IsActive = true, IsDeleted = false, AgentId = agent2.Id
                    },
                    new House {
                        Title = "南港軟體園區大戶",
                        City = "台北市", District = "南港區", DetailAddress = "三重路66號12樓",
                        Price = 6200, SquareMeters = 70,
                        Description = "全新大樓，24小時保全，近南港展覽館，未來增值潛力高。",
                        IsActive = true, IsDeleted = false, AgentId = agent2.Id
                    },
                    new House {
                        Title = "汐止夢想社區樓中樓",
                        City = "新北市", District = "汐止區", DetailAddress = "湖前街30號8樓",
                        Price = 1980, SquareMeters = 40,
                        Description = "挑高設計，空間利用大，社區人文氣息濃厚，近金龍湖。",
                        IsActive = true, IsDeleted = false, AgentId = agent2.Id
                    },
                    new House {
                        Title = "內湖別墅",
                        City = "台北市", District = "內湖區", DetailAddress = "民權東路六段180巷",
                        Price = 9800, SquareMeters = 120,
                        Description = "獨棟別墅，自有庭院，隱密性高，屋主移民急售。",
                        IsActive = false, IsDeleted = false, AgentId = agent2.Id
                    },
                    new House {
                        Title = "永和頂溪捷運宅",
                        City = "新北市", District = "永和區", DetailAddress = "永和路二段100號5樓",
                        Price = 2100, SquareMeters = 32,
                        Description = "捷運頂溪站正樓上，到站即到家，交通絕對便利。",
                        IsActive = true, IsDeleted = false, AgentId = agent2.Id
                    }
                };
                context.Houses.AddRange(houses);
                context.SaveChanges();
            }

            // ---------------------------------------------------------
            // 4. 建立預約 (Appointments) - 多建立幾筆不同狀態的
            // ---------------------------------------------------------
            if (!context.Appointments.Any())
            {
                var agent1 = context.Agents.First(a => a.EmployeeCode == "A001");
                var agent2 = context.Agents.First(a => a.EmployeeCode == "A002");
                var user1 = context.Customers.First(c => c.Email == "user1@test.com");
                var user2 = context.Customers.First(c => c.Email == "user2@test.com");
                var vip = context.Customers.First(c => c.Email == "vip@test.com");

                // 找出房子
                var house1 = context.Houses.First(h => h.Title.Contains("大安"));
                var house2 = context.Houses.First(h => h.Title.Contains("內湖"));
                var house3 = context.Houses.First(h => h.Title.Contains("松山"));

                var appointments = new Appointment[]
                {
                    // 給王小明的預約 (待確認)
                    new Appointment {
                        HouseId = house1.Id, CustomerId = vip.Id,
                        VisitDate = DateTime.Now.AddDays(2).AddHours(14),
                        Status = AppointmentStatus.Pending, CreatedAt = DateTime.Now
                    },
                    // 給王小明的預約 (已確認)
                    new Appointment {
                        HouseId = house3.Id, CustomerId = user2.Id,
                        VisitDate = DateTime.Now.AddDays(1).AddHours(10),
                        Status = AppointmentStatus.Confirmed, CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    // 給李大仁的預約 (待確認)
                    new Appointment {
                        HouseId = house2.Id, CustomerId = user1.Id,
                        VisitDate = DateTime.Now.AddDays(5).AddHours(15),
                        Status = AppointmentStatus.Pending, CreatedAt = DateTime.Now
                    }
                };
                context.Appointments.AddRange(appointments);
                context.SaveChanges();
            }
        }
    }
}