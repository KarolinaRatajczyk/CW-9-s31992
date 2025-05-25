namespace CW9.Models;

public class PrescriptionMedicament
{
    
    public int IdMedicament { get; set; }
    public int IdPrescription { get; set; }
    public int? Dose { get; set; }
}