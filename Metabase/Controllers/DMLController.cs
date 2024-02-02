using Metabase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Metabase.Contracts.GetDTOs;

namespace Metabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DMLController : ControllerBase
    {
        private readonly IDMLService _DMLService;

        public DMLController(IDMLService dMLService)
        {
            _DMLService = dMLService;
        }

        [HttpGet("Database/{databaseId}")]
        public async Task<ActionResult<GetDatabaseResponseDTO>> GetDatabaseAsync(int databaseId, CancellationToken cancellationToken = default)
        {
            return Ok(await _DMLService.GetDatabaseAsync(databaseId, cancellationToken));
        }

        [HttpGet("Database/ByName/{databaseName}")]
        public async Task<ActionResult<GetDatabaseResponseDTO>> GetDatabaseByNameAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            return Ok(await _DMLService.GetDatabaseByNameAsync(databaseName, cancellationToken));
        }

        [HttpGet("Database/{databaseId}/Relation/{relationId}")]

        public async Task<ActionResult> GetRelationAsync(int databaseId, int relationId, CancellationToken cancellationToken = default)
        {
            return Ok(await _DMLService.GetRelationAsync(databaseId, relationId, cancellationToken));
        }

        [HttpGet("Database/ByName/{databaseName}/Relation/{relationName}")]

        public async Task<ActionResult> GetRelationByNameAsync(string databaseName, string relationName, CancellationToken cancellationToken = default)
        {
            return Ok(await _DMLService.GetRelationByNameAsync(databaseName, relationName, cancellationToken));
        }

    }
}
