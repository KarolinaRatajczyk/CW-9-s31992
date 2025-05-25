using System.ComponentModel.DataAnnotations;

namespace CW9.DTOs
{
    public class InsertPrescriptionDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public InsertPatientDto Patient { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required, MinLength(1), MaxLength(10)]
        public List<InsertMedicamentDto> Medicaments { get; set; }
            = new List<InsertMedicamentDto>();
    }

    public class InsertPatientDto
    {
        public int? IdPatient { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName  { get; set; } = null!;
    }

    public class InsertMedicamentDto
    {
        [Required]
        public int IdMedicament { get; set; }

        [Required]
        public int Dose { get; set; }

        [Required, MaxLength(100)]
        public string Details { get; set; } = null!;
    }
}