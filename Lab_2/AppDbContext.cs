using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lab_2;

public class Destination
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public decimal BaseFare { get; set; }

    public List<Train> Trains { get; set; } = new();
}

public class CarriageType
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal Surcharge { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}

public class Passenger
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public List<Ticket> Tickets { get; set; } = new();
}

public class Train
{
    public int Id { get; set; }
    public string TrainNumber { get; set; } = string.Empty;
    public string TrainType { get; set; } = string.Empty;
    public int DestinationId { get; set; }
    public string DepartureTime { get; set; } = string.Empty;
    public string ArrivalTime { get; set; } = string.Empty;

    public Destination? Destination { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}

public class Ticket
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int TrainId { get; set; }
    public int CarriageNumber { get; set; }
    public int CarriageTypeId { get; set; }
    public string DepartureDate { get; set; } = string.Empty;
    public decimal UrgencySurcharge { get; set; }

    public Passenger? Passenger { get; set; }
    public Train? Train { get; set; }
    public CarriageType? CarriageType { get; set; }

    public decimal TotalCost => (Train?.Destination?.BaseFare ?? 0) 
                              + (CarriageType?.Surcharge ?? 0) 
                              + UrgencySurcharge;
}

public class AppDbContext : DbContext
{
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Train> Trains => Set<Train>();
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<CarriageType> CarriageTypes => Set<CarriageType>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tickets_ef.db");
    }

    public void EnsureSeeded()
    {
        Database.EnsureCreated();

        if (!Passengers.Any())
        {
            var d1 = new Destination { Name = "Львів-Головний", DistanceKm = 540, BaseFare = 320.00m };
            var d2 = new Destination { Name = "Одеса-Головна", DistanceKm = 480, BaseFare = 290.00m };
            var d3 = new Destination { Name = "Харків-Пасажирський", DistanceKm = 470, BaseFare = 280.00m };
            var d4 = new Destination { Name = "Ужгород", DistanceKm = 780, BaseFare = 410.00m };
            Destinations.AddRange(d1, d2, d3, d4);

            var ct1 = new CarriageType { TypeName = "Загальний", Surcharge = 0.00m };
            var ct2 = new CarriageType { TypeName = "Плацкартний", Surcharge = 50.00m };
            var ct3 = new CarriageType { TypeName = "Купе", Surcharge = 150.00m };
            var ct4 = new CarriageType { TypeName = "Люкс", Surcharge = 350.00m };
            CarriageTypes.AddRange(ct1, ct2, ct3, ct4);

            var p1 = new Passenger { FullName = "Коваленко Олексій Іванович", Address = "м. Київ, вул. Хрещатик, 15", Phone = "+380501112233" };
            var p2 = new Passenger { FullName = "Шевченко Марія Василівна", Address = "м. Львів, вул. Городоцька, 42", Phone = "+380672223344" };
            var p3 = new Passenger { FullName = "Бондаренко Тарас Григорович", Address = "м. Одеса, вул. Дерибасівська, 8", Phone = "+380633334455" };
            var p4 = new Passenger { FullName = "Мельник Оксана Петрівна", Address = "м. Дніпро, пр. Яворницького, 102", Phone = "+380994445566" };
            var p5 = new Passenger { FullName = "Лисенко Богдан Романович", Address = "м. Харків, вул. Сумська, 21", Phone = "+380975556677" };
            Passengers.AddRange(p1, p2, p3, p4, p5);

            var tr1 = new Train { TrainNumber = "072К", TrainType = "Швидкісний", Destination = d1, DepartureTime = "06:30", ArrivalTime = "12:45" };
            var tr2 = new Train { TrainNumber = "705К", TrainType = "Інтерсіті", Destination = d3, DepartureTime = "17:20", ArrivalTime = "22:10" };
            var tr3 = new Train { TrainNumber = "142Л", TrainType = "Пасажирський", Destination = d2, DepartureTime = "09:00", ArrivalTime = "15:40" };
            Trains.AddRange(tr1, tr2, tr3);

            Tickets.AddRange(
                new Ticket { Passenger = p1, Train = tr1, CarriageNumber = 1, CarriageType = ct4, DepartureDate = "2026-11-01", UrgencySurcharge = 50.00m },
                new Ticket { Passenger = p1, Train = tr2, CarriageNumber = 1, CarriageType = ct3, DepartureDate = "2026-11-15", UrgencySurcharge = 0.00m },
                new Ticket { Passenger = p2, Train = tr3, CarriageNumber = 2, CarriageType = ct3, DepartureDate = "2026-11-02", UrgencySurcharge = 30.00m },
                new Ticket { Passenger = p2, Train = tr1, CarriageNumber = 2, CarriageType = ct3, DepartureDate = "2026-11-20", UrgencySurcharge = 0.00m },
                new Ticket { Passenger = p3, Train = tr1, CarriageNumber = 3, CarriageType = ct2, DepartureDate = "2026-11-05", UrgencySurcharge = 0.00m },
                new Ticket { Passenger = p3, Train = tr3, CarriageNumber = 1, CarriageType = ct2, DepartureDate = "2026-11-18", UrgencySurcharge = 25.00m },
                new Ticket { Passenger = p4, Train = tr2, CarriageNumber = 2, CarriageType = ct1, DepartureDate = "2026-11-07", UrgencySurcharge = 0.00m },
                new Ticket { Passenger = p4, Train = tr1, CarriageNumber = 1, CarriageType = ct4, DepartureDate = "2026-11-22", UrgencySurcharge = 60.00m },
                new Ticket { Passenger = p5, Train = tr2, CarriageNumber = 3, CarriageType = ct2, DepartureDate = "2026-11-10", UrgencySurcharge = 20.00m },
                new Ticket { Passenger = p5, Train = tr3, CarriageNumber = 3, CarriageType = ct1, DepartureDate = "2026-11-25", UrgencySurcharge = 0.00m }
            );

            SaveChanges();
        }
    }
}