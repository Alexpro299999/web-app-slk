using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;

namespace MyWebApp.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        context.Database.EnsureCreated();

        if (context.Tariffs.Any())
        {
            return;
        }

        var tariffs = new Tariff[]
        {
            new Tariff { Name = "NEURAL_LINK_BASIC", SpeedMbps = 100, Price = 500 },
            new Tariff { Name = "CYBER_STREAM_PLUS", SpeedMbps = 500, Price = 850 },
            new Tariff { Name = "GIGABIT_MATRIX", SpeedMbps = 1000, Price = 1200 },
            new Tariff { Name = "CORPO_PREMIUM_10G", SpeedMbps = 10000, Price = 5000 }
        };
        context.Tariffs.AddRange(tariffs);
        context.SaveChanges();

        var equipments = new Equipment[]
        {
            new Equipment { ModelName = "MIKROTIK_HAP_AC2", SerialNumber = "MT-992-XLA", Type = "Router", IsInStock = true },
            new Equipment { ModelName = "CISCO_CATALYST_2960", SerialNumber = "CS-200-VBR", Type = "Switch", IsInStock = true },
            new Equipment { ModelName = "HUAWEI_GPON_ONT", SerialNumber = "HW-551-QWE", Type = "Modem", IsInStock = false },
            new Equipment { ModelName = "TP-LINK_ARCHER_AX73", SerialNumber = "TP-777-GOD", Type = "Router", IsInStock = true }
        };
        context.Equipments.AddRange(equipments);
        context.SaveChanges();

        context.Subscribers.AddRange(
            new Subscriber { FullName = "JOHNNY SILVERHAND", Address = "NIGHT_CITY_BLOCK_1", ContractNumber = "NC-2077-01", TariffId = tariffs[2].Id },
            new Subscriber { FullName = "ALT CUNNINGHAM", Address = "CYBERSPACE_VIRTUAL_NODE", ContractNumber = "NC-2077-02", TariffId = tariffs[3].Id },
            new Subscriber { FullName = "V", Address = "MEGABUILDING_H10", ContractNumber = "NC-2077-03", TariffId = tariffs[1].Id }
        );
        context.SaveChanges();
    }
}