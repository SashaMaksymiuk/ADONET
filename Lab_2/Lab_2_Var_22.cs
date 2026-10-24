using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Lab_2;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

using var db = new AppDbContext();
db.EnsureSeeded();

Console.WriteLine("ЛАБОРАТОРНА РОБОТА №2.\n");

Console.WriteLine("ЖУРНАЛ ЗАМОВЛЕНИХ КВИТКІВ (EF CORE LINQ):");

var tickets = db.Tickets
    .Include(t => t.Passenger)
    .Include(t => t.CarriageType)
    .Include(t => t.Train)
        .ThenInclude(tr => tr!.Destination)
    .OrderBy(t => t.Id)
    .ToList();

foreach (var t in tickets)
{
    Console.WriteLine($"\n[Квиток №{t.Id}]");
    Console.WriteLine($" 1) ПІБ пасажира:              {t.Passenger?.FullName}");
    Console.WriteLine($" 2) Домашня адреса:            {t.Passenger?.Address}");
    Console.WriteLine($" 3) Номер телефону:            {t.Passenger?.Phone}");
    Console.WriteLine($" 4) Номер поїзда:              {t.Train?.TrainNumber}");
    Console.WriteLine($" 5) Клас/тип поїзда:           {t.Train?.TrainType}");
    Console.WriteLine($" 6) Номер вагона:              {t.CarriageNumber}");
    Console.WriteLine($" 7) Категорія вагона:          {t.CarriageType?.TypeName}");
    Console.WriteLine($" 8) Дата поїздки:              {t.DepartureDate}");
    Console.WriteLine($" 9) Відправлення / Прибуття:   {t.Train?.DepartureTime} — {t.Train?.ArrivalTime}");
    Console.WriteLine($"10) Станція призначення:       {t.Train?.Destination?.Name}");
    Console.WriteLine($"11) Відстань маршруту:         {t.Train?.Destination?.DistanceKm} км");
    Console.WriteLine($"12) Базовий тариф:             {t.Train?.Destination?.BaseFare:F2} грн");
    Console.WriteLine($"13) Націнка за терміновість:   {t.UrgencySurcharge:F2} грн");
    Console.WriteLine($"14) Доплата за категорію:      {t.CarriageType?.Surcharge:F2} грн");
    Console.WriteLine($"    --------------------------------------------------");
    Console.WriteLine($"    ПІДСУМКОВА ВАРТІСТЬ:       {t.TotalCost:F2} грн");
}

Console.WriteLine("\n\nЗВЕДЕНІ ДАНІ: АКТИВНІСТЬ ПАСАЖИРІВ (LINQ GROUP/COUNT)");
var passengerStats = db.Passengers
    .Select(p => new
    {
        p.FullName,
        p.Phone,
        TotalTickets = p.Tickets.Count
    })
    .ToList();

foreach (var stat in passengerStats)
{
    Console.WriteLine($"* {stat.FullName,-30} | {stat.Phone} | Бронювань: {stat.TotalTickets}");
}

Console.WriteLine("\n\nДОВІДНИК РЕЙСІВ ТА ПОЇЗДІВ");
var trainList = db.Trains.Include(t => t.Destination).ToList();
foreach (var tr in trainList)
{
    Console.WriteLine($"Поїзд №{tr.TrainNumber} [{tr.TrainType}] напрямок: {tr.Destination?.Name} ({tr.DepartureTime} -> {tr.ArrivalTime})");
}

Console.WriteLine("\n\nДОВІДНИК ПУНКТІВ ПРИЗНАЧЕННЯ");
var stations = db.Destinations.ToList();
foreach (var st in stations)
{
    Console.WriteLine($"* {st.Name,-22} Відстань: {st.DistanceKm,3} км | Тариф: {st.BaseFare:F2} грн");
}

Console.WriteLine("\nВідображення завершено успішно.");