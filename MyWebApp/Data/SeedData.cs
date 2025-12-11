using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;
using MyWebApp.Services;

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
                var embeddingGenerator = serviceProvider.GetRequiredService<IEmbeddingGenerator>();

                // 1. Создаем тарифы, если нет
                if (!context.Tariffs.Any())
                {
                    var tariffs = new Tariff[]
                    {
                        new Tariff { Name = "Эконом", SpeedMbps = 100, Price = 450 },
                        new Tariff { Name = "Стандарт", SpeedMbps = 300, Price = 650 },
                        new Tariff { Name = "Геймер", SpeedMbps = 800, Price = 990 },
                        new Tariff { Name = "Премиум", SpeedMbps = 1000, Price = 1200 }
                    };
                    context.Tariffs.AddRange(tariffs);
                    context.SaveChanges();
                }

                // 2. Работаем с оборудованием
                var equipments = context.Equipments.ToList();

                // Если оборудования вообще нет, добавляем начальное
                if (!equipments.Any())
                {
                    context.Equipments.AddRange(
                        new Equipment { ModelName = "TP-Link Archer C6", SerialNumber = "SN-102030", Type = "Router", IsInStock = true, Embedding = embeddingGenerator.GenerateEmbedding("TP-Link Archer C6 Router") },
                        new Equipment { ModelName = "D-Link DIR-842", SerialNumber = "SN-506070", Type = "Router", IsInStock = true, Embedding = embeddingGenerator.GenerateEmbedding("D-Link DIR-842 Router") },
                        new Equipment { ModelName = "Huawei HG8245", SerialNumber = "GPON-998877", Type = "Modem", IsInStock = false, Embedding = embeddingGenerator.GenerateEmbedding("Huawei HG8245 Modem") },
                        new Equipment { ModelName = "Keenetic Giga", SerialNumber = "KN-1011", Type = "Router", IsInStock = true, Embedding = embeddingGenerator.GenerateEmbedding("Keenetic Giga Router") }
                    );
                    context.SaveChanges();
                }
                else
                {
                    // ВАЖНО: Если оборудование есть, обновляем ему векторы (fix для существующих данных)
                    bool wasUpdated = false;
                    foreach (var eq in equipments)
                    {
                        // Генерируем вектор заново, чтобы он соответствовал текущему алгоритму
                        var text = $"{eq.ModelName} {eq.Type}";
                        eq.Embedding = embeddingGenerator.GenerateEmbedding(text);
                        wasUpdated = true;
                    }

                    if (wasUpdated)
                    {
                        context.UpdateRange(equipments);
                        context.SaveChanges();
                    }
                }

                // 3. Создаем абонентов, если нет
                if (!context.Subscribers.Any())
                {
                    var tariffId = context.Tariffs.First().Id;
                    context.Subscribers.AddRange(
                        new Subscriber { FullName = "Иванов Иван Иванович", Address = "ул. Ленина, д. 10, кв. 5", ContractNumber = "CTR-2024-001", TariffId = tariffId },
                        new Subscriber { FullName = "Петрова Анна Сергеевна", Address = "пр. Мира, д. 45, кв. 120", ContractNumber = "CTR-2024-002", TariffId = tariffId },
                        new Subscriber { FullName = "Сидоров Алексей Петрович", Address = "ул. Гагарина, д. 7, кв. 33", ContractNumber = "CTR-2024-003", TariffId = tariffId }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}