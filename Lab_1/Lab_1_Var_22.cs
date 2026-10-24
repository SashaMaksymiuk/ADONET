using System;
using System.Data;
using System.Globalization;
using System.Text;
using Lab_1;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

var service = new TicketService();
service.SetupDatabase();

Console.WriteLine("ЛАБОРАТОРНА РОБОТА №1. ADO.NET (ВАРІАНТ №7: РЕЗЕРВУВАННЯ КВИТКІВ)\n");

Console.WriteLine("ЖУРНАЛ ЗАМОВЛЕНИХ КВИТКІВ:");
DataTable reservations = service.FetchAllReservations();

foreach (DataRow row in reservations.Rows)
{
    Console.WriteLine($"\n[Квиток №{row["TicketId"]}]");
    Console.WriteLine($" 1) ПІБ пасажира:              {row["ClientName"]}");
    Console.WriteLine($" 2) Домашня адреса:            {row["ClientAddress"]}");
    Console.WriteLine($" 3) Номер телефону:            {row["ClientPhone"]}");
    Console.WriteLine($" 4) Номер поїзда:              {row["TrainNumber"]}");
    Console.WriteLine($" 5) Клас/тип поїзда:           {row["TrainType"]}");
    Console.WriteLine($" 6) Номер вагона:              {row["CarriageNumber"]}");
    Console.WriteLine($" 7) Категорія вагона:          {row["CarriageCategory"]}");
    Console.WriteLine($" 8) Дата поїздки:              {row["DepartureDate"]}");
    Console.WriteLine($" 9) Відправлення / Прибуття:   {row["DepartureTime"]} — {row["ArrivalTime"]}");
    Console.WriteLine($"10) Станція призначення:       {row["TargetStation"]}");
    Console.WriteLine($"11) Відстань маршруту:         {row["DistanceKm"]} км");
    Console.WriteLine($"12) Базовий тариф:             {Convert.ToDecimal(row["BaseFare"]):F2} грн");
    Console.WriteLine($"13) Націнка за терміновість:   {Convert.ToDecimal(row["UrgencySurcharge"]):F2} грн");
    Console.WriteLine($"14) Доплата за категорію:      {Convert.ToDecimal(row["CarriageExtra"]):F2} грн");
    Console.WriteLine($"    --------------------------------------------------");
    Console.WriteLine($"    ПІДСУМКОВА ВАРТІСТЬ:       {Convert.ToDecimal(row["TotalCost"]):F2} грн");
}

Console.WriteLine("\n\nЗВЕДЕНІ ДАНІ: АКТИВНІСТЬ ПАСАЖИРІВ (ВІД 2 ПОЇЗДОК)");
DataTable clients = service.GetPassengerStats();
foreach (DataRow r in clients.Rows)
{
    Console.WriteLine($"* {r["FullName"],-30} | {r["Phone"]} | Бронювань: {r["TotalBookings"]}");
}

Console.WriteLine("\n\nДОВІДНИК РЕЙСІВ ТА ПОЇЗДІВ");
DataTable trains = service.GetTrainSchedules();
foreach (DataRow r in trains.Rows)
{
    Console.WriteLine($"Поїзд №{r["TrainNumber"]} [{r["TrainType"]}] напрямок: {r["FinalStation"]} ({r["DepartureTime"]} -> {r["ArrivalTime"]})");
}

Console.WriteLine("\n\nДОВІДНИК ПУНКТІВ ПРИЗНАЧЕННЯ");
DataTable stations = service.GetStationList();
foreach (DataRow r in stations.Rows)
{
    Console.WriteLine($"* {r["Name"],-22} Відстань: {r["DistanceKm"],3} км | Тариф: {Convert.ToDecimal(r["BaseFare"]):F2} грн");
}

Console.WriteLine("\nВідображення завершено успішно.");