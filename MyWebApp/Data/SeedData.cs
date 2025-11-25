using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Tariffs.Any())
                {
                    return;   
                }

                var tariffs = new Tariff[]
                {
                    new Tariff { Name = "Эконом", SpeedMbps = 100, Price = 450 },
                    new Tariff { Name = "Стандарт", SpeedMbps = 300, Price = 650 },
                    new Tariff { Name = "Геймер", SpeedMbps = 800, Price = 990 },
                    new Tariff { Name = "Премиум", SpeedMbps = 1000, Price = 1200 }
                };
                context.Tariffs.AddRange(tariffs);
                context.SaveChanges(); 


                context.Equipments.AddRange(
                    new Equipment { ModelName = "TP-Link Archer C6", SerialNumber = "SN-102030", Type = "Роутер", IsInStock = true },
                    new Equipment { ModelName = "D-Link DIR-842", SerialNumber = "SN-506070", Type = "Роутер", IsInStock = true },
                    new Equipment { ModelName = "Huawei HG8245", SerialNumber = "GPON-998877", Type = "GPON Терминал", IsInStock = false },
                    new Equipment { ModelName = "Keenetic Giga", SerialNumber = "KN-1011", Type = "Роутер", IsInStock = true }
                );
                context.SaveChanges();

                context.Subscribers.AddRange(
                    new Subscriber
                    {
                        FullName = "Иванов Иван Иванович",
                        Address = "ул. Ленина, д. 10, кв. 5",
                        ContractNumber = "CTR-2024-001",
                        TariffId = tariffs[0].Id 
                    },
                    new Subscriber
                    {
                        FullName = "Петрова Анна Сергеевна",
                        Address = "пр. Мира, д. 45, кв. 120",
                        ContractNumber = "CTR-2024-002",
                        TariffId = tariffs[1].Id 
                    },
                    new Subscriber
                    {
                        FullName = "Сидоров Алексей Петрович",
                        Address = "ул. Гагарина, д. 7, кв. 33",
                        ContractNumber = "CTR-2024-003",
                        TariffId = tariffs[2].Id
                    }
                );
                context.SaveChanges();
            }
        }
    }
}