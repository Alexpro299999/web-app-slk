using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace MyWebApp.Services
{
    public class EquipmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOpenAiService _openAiService;

        public EquipmentService(ApplicationDbContext context, IOpenAiService openAiService)
        {
            _context = context;
            _openAiService = openAiService;
        }

        public async Task AddEquipmentAsync(Equipment equipment)
        {
            // Используем ModelName вместо Name
            var textToEmbed = $"Устройство: {equipment.ModelName}. Тип: {equipment.Type}. Описание: {equipment.Description}";

            float[] vector = await _openAiService.GetEmbeddingAsync(textToEmbed);

            equipment.Embedding = new Vector(vector);

            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEquipmentEmbeddingsFromDescriptionAsync()
        {
            var equipments = await _context.Equipments.ToListAsync();
            foreach (var equipment in equipments)
            {
                var textToEmbed = $"Устройство: {equipment.ModelName}. Тип: {equipment.Type}. Описание: {equipment.Description}";
                float[] vector = await _openAiService.GetEmbeddingAsync(textToEmbed);
                equipment.Embedding = new Vector(vector);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Equipment>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await _context.Equipments.ToListAsync();
            }

            float[] queryVector = await _openAiService.GetEmbeddingAsync(query);
            var searchVector = new Vector(queryVector);

            return await _context.Equipments
                .OrderBy(x => x.Embedding!.CosineDistance(searchVector))
                .Take(5)
                .ToListAsync();
        }

        // Метод для массового обновления векторов
        public async Task GenerateEmbeddingsForEmptyAsync()
        {
            // 1. Берем все товары, у которых нет вектора
            var emptyEquipments = await _context.Equipments
                .Where(e => e.Embedding == null)
                .ToListAsync();

            foreach (var item in emptyEquipments)
            {
                // 2. Формируем текст для нейросети
                var textToEmbed = $"Модель: {item.ModelName}. Тип: {item.Type}. Описание: {item.Description}";

                // 3. Спрашиваем Ollama
                try
                {
                    float[] vector = await _openAiService.GetEmbeddingAsync(textToEmbed);
                    item.Embedding = new Pgvector.Vector(vector);
                }
                catch (Exception ex)
                {
                    // Если Ollama тупит, просто пропускаем пока
                    Console.WriteLine($"Error processing {item.ModelName}: {ex.Message}");
                }
            }

            // 4. Сохраняем все изменения разом
            await _context.SaveChangesAsync();
        }

    }
}