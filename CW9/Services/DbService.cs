using CW9.DTOs;
using CW9.Models;
using CW9.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CW9.Services;


public interface IDbService
{
    public Task<int> CreatePrescriptionAsync(InsertPrescriptionDto prescriptionData);
    
    Task<PatientDetailsDto> GetPatientDetailsByIdAsync(int patientId);
}


public class DbService : IDbService
{
    private readonly AppDbContext _db;
    public DbService(AppDbContext db) => _db = db;
    public async Task<int> CreatePrescriptionAsync(InsertPrescriptionDto prescriptionData)
    {
        if (prescriptionData.Medicaments == null || prescriptionData.Medicaments.Count < 1 || prescriptionData.Medicaments.Count > 10)
        {
            throw new ArgumentException("Prescription must contain between 1 and 10 medicaments");
        }
        
        if(prescriptionData.DueDate < prescriptionData.Date)
            throw new ArgumentException("Due date must be greater or equal to date");
        
        Patient? patient = null;
        if (prescriptionData.Patient.IdPatient.HasValue)
        {
            patient = await _db.Patients.FindAsync(prescriptionData.Patient.IdPatient.Value);
        }
        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = prescriptionData.Patient.FirstName,
                LastName  = prescriptionData.Patient.LastName,
                BirthDate = prescriptionData.Patient.BirthDate
            };
            await _db.Patients.AddAsync(patient);
            await _db.SaveChangesAsync();
        }
        
        var doctor = await _db.Doctors.FindAsync(prescriptionData.DoctorId);
        if (doctor == null)
            throw new KeyNotFoundException($"Doctor with id {prescriptionData.DoctorId} not found");
        
        var medIds = prescriptionData.Medicaments.Select(m => m.IdMedicament).ToList();
        var meds = await _db.Medicaments
            .Where(m => medIds.Contains(m.IdMedicament))
            .ToListAsync();
        if (meds.Count != medIds.Count)
            throw new KeyNotFoundException("One or more Medicaments not found");
        
        var prescription = new Prescription
        {
            Date      = prescriptionData.Date,
            DueDate   = prescriptionData.DueDate,
            IdPatient = patient.IdPatient,
            IdDoctor  = doctor.IdDoctor,
            PrescriptionMedicaments = prescriptionData.Medicaments
                .Select(m => new PrescriptionMedicament
                {
                    IdMedicament = m.IdMedicament,
                    Dose         = m.Dose,
                    Details      = m.Details
                })
                .ToList()
        };

        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync();

        return prescription.IdPrescription;
    }
    
    public async Task<PatientDetailsDto> GetPatientDetailsByIdAsync(int patientId)
    {
        var patient = await _db.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(r => r.Doctor)
            .Include(p => p.Prescriptions)
            .ThenInclude(r => r.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.IdPatient == patientId);

        if (patient == null)
            throw new KeyNotFoundException($"Patient with id {patientId} not found");

        var dto = new PatientDetailsDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.BirthDate,
            Prescriptions = patient.Prescriptions
                .OrderBy(r => r.DueDate)
                .Select(r => new PrescriptionDto
                {
                    IdPrescription = r.IdPrescription,
                    Date = r.Date,
                    DueDate = r.DueDate,
                    Doctor = new DoctorDto
                    {
                        IdDoctor = r.Doctor.IdDoctor,
                        FirstName = r.Doctor.FirstName,
                        LastName = r.Doctor.LastName,
                        Email = r.Doctor.Email
                    },
                    Medicaments = r.PrescriptionMedicaments
                        .Select(pm => new MedicamentDto
                        {
                            IdMedicament = pm.IdMedicament,
                            Name = pm.Medicament.Name,
                            Dose = pm.Dose,
                            Details = pm.Details
                        })
                        .ToList()
                })
                .ToList()
        };

        return dto;
    }
}