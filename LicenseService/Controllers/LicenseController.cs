using LicenseService.Handlers;
using LicenseService.Models;
using Microsoft.AspNetCore.Mvc;

namespace LicenseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseHandler _licenseHandler;

        public LicenseController(ILicenseHandler licenseHandler)
        {
            _licenseHandler = licenseHandler;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<License>> AddLicense([FromBody] AddLicenseRequestModel addLicenseRequest)
        {
            var createdLicense = await _licenseHandler.AddLicense(addLicenseRequest);
            return CreatedAtAction(nameof(Get), new { id = createdLicense.Id }, createdLicense);
        }

        [HttpGet]
        public async Task<ActionResult<List<LicenseResponseModel>>> GetLicenses()
        {
            return await _licenseHandler.GetLicenses();
        }

        [HttpPut]
        [Route("Rent")]
        public async Task<ActionResult<RentLicenseResponseModel>> RentLicense([FromBody] RentLicenseRequestModel rentLicenseRequest)
        {
            return await _licenseHandler.RentLicense(rentLicenseRequest);
        }

        [HttpGet]
        [Route("CheckRentalExpiration/{id}")]
        public async Task<bool> CheckLicenseRentalExpiration(string id)
        {
            return await _licenseHandler.HasLicenseRentalExpired(id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LicenseResponseModel>> Get(string id)
        {
            var license = await _licenseHandler.GetLicenseById(id);

            if (license == null)
            {
                return NotFound($"License with Id = {id} not found");
            }
            return license;
        }
    }
}
