using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Lab_1;

public class TicketService
{
    private readonly string _connectionString;

    public TicketService(string dbFileName = "tickets.db")
    {
        _connectionString = $"Data Source={dbFileName}";
    }

    public void SetupDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            PRAGMA foreign_keys = ON;

            CREATE TABLE IF NOT EXISTS Destinations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                DistanceKm REAL NOT NULL,
                BaseFare REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS CarriageTypes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TypeName TEXT NOT NULL,
                Surcharge REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Passengers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                Address TEXT NOT NULL,
                Phone TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Trains (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TrainNumber TEXT NOT NULL,
                TrainType TEXT NOT NULL,
                DestinationId INTEGER NOT NULL,
                DepartureTime TEXT NOT NULL,
                ArrivalTime TEXT NOT NULL,
                FOREIGN KEY (DestinationId) REFERENCES Destinations(Id)
            );

            CREATE TABLE IF NOT EXISTS Tickets (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PassengerId INTEGER NOT NULL,
                TrainId INTEGER NOT NULL,
                CarriageNumber INTEGER NOT NULL,
                CarriageTypeId INTEGER NOT NULL,
                DepartureDate TEXT NOT NULL,
                UrgencySurcharge REAL NOT NULL DEFAULT 0,
                FOREIGN KEY (PassengerId) REFERENCES Passengers(Id),
                FOREIGN KEY (TrainId) REFERENCES Trains(Id),
                FOREIGN KEY (CarriageTypeId) REFERENCES CarriageTypes(Id)
            );
        ";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT COUNT(1) FROM Passengers;";
        var count = Convert.ToInt64(cmd.ExecuteScalar() ?? 0);

        if (count == 0)
        {
            SeedInitialRecords(connection);
        }
    }

    private void SeedInitialRecords(SqliteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Destinations (Name, DistanceKm, BaseFare) VALUES
            ('Львів-Головний', 540, 320.00),
            ('Одеса-Головна', 480, 290.00),
            ('Харків-Пасажирський', 470, 280.00),
            ('Ужгород', 780, 410.00);

            INSERT INTO CarriageTypes (TypeName, Surcharge) VALUES
            ('Загальний', 0.00),
            ('Плацкартний', 50.00),
            ('Купе', 150.00),
            ('Люкс', 350.00);

            INSERT INTO Passengers (FullName, Address, Phone) VALUES
            ('Коваленко Олексій Іванович', 'м. Київ, вул. Хрещатик, 15', '+380501112233'),
            ('Шевченко Марія Василівна', 'м. Львів, вул. Городоцька, 42', '+380672223344'),
            ('Бондаренко Тарас Григорович', 'м. Одеса, вул. Дерибасівська, 8', '+380633334455'),
            ('Мельник Оксана Петрівна', 'м. Дніпро, пр. Яворницького, 102', '+380994445566'),
            ('Лисенко Богдан Романович', 'м. Харків, вул. Сумська, 21', '+380975556677');

            INSERT INTO Trains (TrainNumber, TrainType, DestinationId, DepartureTime, ArrivalTime) VALUES
            ('072К', 'Швидкісний', 1, '06:30', '12:45'),
            ('705К', 'Інтерсіті', 3, '17:20', '22:10'),
            ('142Л', 'Пасажирський', 2, '09:00', '15:40');

            INSERT INTO Tickets (PassengerId, TrainId, CarriageNumber, CarriageTypeId, DepartureDate, UrgencySurcharge) VALUES
            (1, 1, 1, 4, '2026-11-01', 50.00),
            (1, 2, 1, 3, '2026-11-15', 0.00),
            (2, 3, 2, 3, '2026-11-02', 30.00),
            (2, 1, 2, 3, '2026-11-20', 0.00),
            (3, 1, 3, 2, '2026-11-05', 0.00),
            (3, 3, 1, 2, '2026-11-18', 25.00),
            (4, 2, 2, 1, '2026-11-07', 0.00),
            (4, 1, 1, 4, '2026-11-22', 60.00),
            (5, 2, 3, 2, '2026-11-10', 20.00),
            (5, 3, 3, 1, '2026-11-25', 0.00);
        ";
        cmd.ExecuteNonQuery();
    }

    private DataTable ExecuteQuery(string query)
    {
        var dt = new DataTable();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var command = new SqliteCommand(query, conn);
        using var reader = command.ExecuteReader();
        dt.Load(reader);
        return dt;
    }

    public DataTable FetchAllReservations()
    {
        string query = @"
            SELECT 
                tk.Id AS TicketId,
                ps.FullName AS ClientName,
                ps.Address AS ClientAddress,
                ps.Phone AS ClientPhone,
                tr.TrainNumber,
                tr.TrainType,
                tk.CarriageNumber,
                ct.TypeName AS CarriageCategory,
                tk.DepartureDate,
                tr.DepartureTime,
                tr.ArrivalTime,
                dst.Name AS TargetStation,
                dst.DistanceKm,
                dst.BaseFare,
                tk.UrgencySurcharge,
                ct.Surcharge AS CarriageExtra,
                (dst.BaseFare + ct.Surcharge + tk.UrgencySurcharge) AS TotalCost
            FROM Tickets tk
            INNER JOIN Passengers ps ON tk.PassengerId = ps.Id
            INNER JOIN Trains tr ON tk.TrainId = tr.Id
            INNER JOIN Destinations dst ON tr.DestinationId = dst.Id
            INNER JOIN CarriageTypes ct ON tk.CarriageTypeId = ct.Id
            ORDER BY tk.Id;";

        return ExecuteQuery(query);
    }

    public DataTable GetPassengerStats()
    {
        string query = @"
            SELECT 
                ps.Id,
                ps.FullName,
                ps.Phone,
                COUNT(tk.Id) AS TotalBookings
            FROM Passengers ps
            LEFT JOIN Tickets tk ON ps.Id = tk.PassengerId
            GROUP BY ps.Id, ps.FullName, ps.Phone
            ORDER BY ps.Id;";

        return ExecuteQuery(query);
    }

    public DataTable GetTrainSchedules()
    {
        string query = @"
            SELECT 
                t.TrainNumber,
                t.TrainType,
                d.Name AS FinalStation,
                t.DepartureTime,
                t.ArrivalTime
            FROM Trains t
            INNER JOIN Destinations d ON t.DestinationId = d.Id
            ORDER BY t.Id;";

        return ExecuteQuery(query);
    }

    public DataTable GetStationList()
    {
        return ExecuteQuery("SELECT Name, DistanceKm, BaseFare FROM Destinations ORDER BY Id;");
    }
}