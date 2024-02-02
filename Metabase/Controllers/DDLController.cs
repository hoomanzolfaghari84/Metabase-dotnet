using Metabase.Contracts;
using Metabase.Interfaces;
using Metabase.Models;
using Metabase.Models.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Metabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DDLController : ControllerBase
    {
        private readonly IDDLService _DDLService;
        private readonly ILogger<DDLController> _logger;
        public DDLController(IDDLService dDLService, ILogger<DDLController> logger)
        {
            _DDLService = dDLService;
            _logger = logger;
        }

        [HttpPost("Database")]
        public async Task<ActionResult<CreateDatabaaseResponseDTO>> CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            return Ok(await _DDLService.CreateDatabaseAsync(databaseName, cancellationToken));
        }

        [HttpPost("Database/{databaseId}/Relation")]
        public async Task<ActionResult<CreateRelationResponseDTO>> CreateRelationAsync(int databaseId, CreateRelationRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _DDLService.CreateRelationAsync(databaseId, requestDTO, cancellationToken));
        }

        [HttpPost("Database/{databaseId}/Relation/{relationId}/Attribute")]
        public async Task<ActionResult<CreateAttributeResponseDTO>> CreateAttribute(int databaseId, int relationId, CreateAttributeRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _DDLService.CreateAttributeAsync(databaseId, relationId, requestDTO, cancellationToken));
        }

        [HttpPost("Database/{databaseId}/Relation/{relationId}/ForeignKeyConstraint")]
        public async Task<ActionResult<CreateForeignKeyResponseDTO>> CreateForeignKeyConstraintAsync(int databaseId, int relationId, CreateForeignKeyConstraintRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _DDLService.CreateForeginKeyConstraint(databaseId, relationId, requestDTO, cancellationToken));
        }






    }

}
