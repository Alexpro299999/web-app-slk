using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages
{
    public class EquipmentsModel : PageModel
    {
        private readonly EquipmentService _equipmentService;

        public EquipmentsModel(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        // --- ИСПРАВЛЕНИЕ 1: Список устройств ---
        // Инициализируем пустым списком, чтобы не было null
        public IList<Equipment> Equipments { get; set; } = new List<Equipment>();

        // --- ИСПРАВЛЕНИЕ 2: Одиночное устройство ---
        // Нужно для формы добавления (BindProperty связывает данные из формы с этой переменной)
        [BindProperty]
        public Equipment Equipment { get; set; } = default!;

        // --- ИСПРАВЛЕНИЕ 3: Поиск ---
        // Добавил '?', теперь может быть пустым (null)
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task OnGetAsync()
        {
            // Если строка поиска пустая, передаем пустую строку, чтобы метод не упал
            Equipments = await _equipmentService.SearchAsync(SearchString ?? string.Empty);
        }

        // Метод для добавления устройства (если у вас есть кнопка "Сохранить" на этой странице)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Если ошибка валидации, перезагружаем список и показываем страницу снова
                Equipments = await _equipmentService.SearchAsync(SearchString ?? string.Empty);
                return Page();
            }

            // Используем сервис для добавления (он сам сгенерирует вектор)
            await _equipmentService.AddEquipmentAsync(Equipment);

            return RedirectToPage();
        }
    }
}