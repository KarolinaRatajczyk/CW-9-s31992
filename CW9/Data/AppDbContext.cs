using CW9.Models;
using Microsoft.EntityFrameworkCore;

namespace CW9.Data;

public class AppDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
    
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var doctors = new List<Doctor>
        {
            new Doctor { IdDoctor = 1, FirstName = "Jan",  LastName = "Kowalski", Email = "jan.kowalski@example.com" },
            new Doctor { IdDoctor = 2, FirstName = "Anna", LastName = "Nowak",    Email = "anna.nowak@example.com" }
        };
        
        
        var patients = new List<Patient>
        {
            new Patient { IdPatient = 1, FirstName = "Jan",   LastName = "Nowak",    BirthDate = new DateTime(1990, 1, 1) },
            new Patient { IdPatient = 2, FirstName = "Anna",  LastName = "Kowalczyk",BirthDate = new DateTime(1985, 3, 15) }
        };
        
        var medicaments = new List<Medicament>
        {
            new Medicament { IdMedicament = 1, Name = "Apap",          Description = "Lek przeciwbólowy",   Type = "Tabletki" },
            new Medicament { IdMedicament = 2, Name = "Ibuprom",       Description = "Lek przeciwzapalny",   Type = "Tabletki" },
            new Medicament { IdMedicament = 3, Name = "Amoxicillin",   Description = "Antybiotyk",           Type = "Kapsułki" }
        };
        
        var prescriptions = new List<Prescription>
        {
            new Prescription { IdPrescription = 1, Date = new DateTime(2023, 12, 1), DueDate = new DateTime(2023, 12, 31), IdDoctor = 1, IdPatient = 1 },
            new Prescription { IdPrescription = 2, Date = new DateTime(2024, 1, 5),  DueDate = new DateTime(2024, 1, 20), IdDoctor = 2, IdPatient = 2 }
        };
        
        var prescriptionMedicaments = new List<PrescriptionMedicament>
        {
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 1, Dose = 2, Details = "Take twice a day" },
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 2, Dose = 1, Details = "Once in the morning" },
            new PrescriptionMedicament { IdPrescription = 2, IdMedicament = 3, Dose = 3, Details = "After meals" }
        };

        modelBuilder.Entity<Doctor>().HasData(doctors);
        modelBuilder.Entity<Patient>().HasData(patients);
        modelBuilder.Entity<Medicament>().HasData(medicaments);
        modelBuilder.Entity<Prescription>().HasData(prescriptions);
        modelBuilder.Entity<PrescriptionMedicament>().HasData(prescriptionMedicaments);
    }
    
}