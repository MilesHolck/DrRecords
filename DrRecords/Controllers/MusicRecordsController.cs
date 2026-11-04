using DrRecords.Models;
using DrRecords.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DrRecords.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class MusicRecordsController : ControllerBase
    {
        private readonly IMusicRecordsRepositoryList _repo;

        // Dependency injection: repository'et bliver givet til os af Program.cs
        public MusicRecordsController(IMusicRecordsRepositoryList repo)
        {
            _repo = repo;
        }

        // GET: api/musicrecords?titleStartsWith=born&artistStartsWith=jon
        // Alle må se pladerne (ingen [Authorize])
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MusicRecord>> Get([FromQuery] string? titleStartsWith, [FromQuery] string? artistStartsWith)
        {
            IEnumerable<MusicRecord> records = _repo.GetAllMusicRecords(titleStartsWith, artistStartsWith);
            return Ok(records); // 200 + liste (evt. tom liste [])
        }

        // GET: api/musicrecords/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MusicRecord> GetById([FromRoute] int id)
        {
            MusicRecord? record = _repo.GetMusicRecordById(id);
            if (record == null)
            {
                return NotFound("No music record with id: " + id);
            }
            return Ok(record);
        }

        // POST: api/musicrecords  (kræver login)
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<MusicRecord> Post([FromBody] MusicRecord newRecord)
        {
            MusicRecord created = _repo.AddMusicRecord(newRecord);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/musicrecords/5  (kræver login)
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<MusicRecord> Put([FromRoute] int id, [FromBody] MusicRecord updatedRecord)
        {
            MusicRecord? record = _repo.UpdateMusicRecord(id, updatedRecord);
            if (record == null)
            {
                return NotFound("No music record with id: " + id);
            }
            return Ok(record);
        }

        // DELETE: api/musicrecords/5  (kræver login)
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<MusicRecord> Delete([FromRoute] int id)
        {
            MusicRecord? deleted = _repo.DeleteByIdMusicRecord(id);
            if (deleted == null)
            {
                return NotFound("No music record with id: " + id);
            }
            return Ok(deleted);
        }
    }
}
