using CW9.Services;
using Microsoft.AspNetCore.Mvc;
using CW9.DTOs;

namespace CW9.Controllers;

[Route("api/prescriptions")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly IDbService _dbService;

    public PrescriptionController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] InsertPrescriptionDto request)
    {
        try
        {
            var id = await _dbService.CreatePrescriptionAsync(request);
            return CreatedAtAction(nameof(GetPatientDetails), new { id = request.Patient.IdPatient ?? id }, id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetPatientDetails(int id)
    {
        try
        {
            var result = await _dbService.GetPatientDetailsByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}