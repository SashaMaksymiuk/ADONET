using System.ComponentModel.DataAnnotations;

namespace Lab_3.Models;

public class Destination
{
    public int Id { get; set; }
    [Display(Name = "Станція призначення")]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Відстань (км)")]
    public double DistanceKm { get; set; }
    [Display(Name = "Базовий тариф (грн)")]
    public decimal BaseFare { get; set; }

    public List<Train> Trains { get; set; } = new();
}

public class CarriageType
{
    public int Id { get; set; }
    [Display(Name = "Тип вагона")]
    public string TypeName { get; set; } = string.Empty;
    [Display(Name = "Доплата (грн)")]
    public decimal Surcharge { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}

public class Passenger
{
    public int Id { get; set; }
    [Display(Name = "ПІБ пасажира")]
    public string FullName { get; set; } = string.Empty;
    [Display(Name = "Домашня адреса")]
    public string Address { get; set; } = string.Empty;
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    public List<Ticket> Tickets { get; set; } = new();
}

public class Train
{
    public int Id { get; set; }
    [Display(Name = "Номер поїзда")]
    public string TrainNumber { get; set; } = string.Empty;
    [Display(Name = "Тип поїзда")]
    public string TrainType { get; set; } = string.Empty;
    public int DestinationId { get; set; }
    [Display(Name = "Час відправлення")]
    public string DepartureTime { get; set; } = string.Empty;
    [Display(Name = "Час прибуття")]
    public string ArrivalTime { get; set; } = string.Empty;

    public Destination? Destination { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}

public class Ticket
{
    public int Id { get; set; }

    [Display(Name = "Пасажир")]
    public int PassengerId { get; set; }
    public Passenger? Passenger { get; set; }

    [Display(Name = "Поїзд")]
    public int TrainId { get; set; }
    public Train? Train { get; set; }

    [Display(Name = "Номер вагона")]
    public int CarriageNumber { get; set; }

    [Display(Name = "Тип вагона")]
    public int CarriageTypeId { get; set; }
    public CarriageType? CarriageType { get; set; }

    [Display(Name = "Дата поїздки")]
    public string DepartureDate { get; set; } = string.Empty;

    [Display(Name = "Доплата за терміновість (грн)")]
    public decimal UrgencySurcharge { get; set; }

    [Display(Name = "Разом до сплати (грн)")]
    public decimal TotalCost => (Train?.Destination?.BaseFare ?? 0) 
                              + (CarriageType?.Surcharge ?? 0) 
                              + UrgencySurcharge;
}