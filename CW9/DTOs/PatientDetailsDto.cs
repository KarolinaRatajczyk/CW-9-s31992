namespace CW9.DTOs
{
    public class PatientDetailsDto
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public List<PrescriptionDto> Prescriptions { get; set; }
            = new List<PrescriptionDto>();
    }

    public class PrescriptionDto
    {
        public int IdPrescription { get; set; }
        public DateTime Date      { get; set; }
        public DateTime DueDate   { get; set; }

        public DoctorDto Doctor   { get; set; } = null!;

        public List<MedicamentDto> Medicaments { get; set; }
            = new List<MedicamentDto>();
    }

    public class DoctorDto
    {
        public int IdDoctor   { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName  { get; set; } = null!;
        public string? Email    { get; set; }
    }

    public class MedicamentDto
    {
        public int IdMedicament { get; set; }
        public string Name      { get; set; } = null!;
        public int Dose         { get; set; }
        public string Details   { get; set; } = null!;
    }
}
