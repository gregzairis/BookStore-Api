using AutoMapper;
using BookStore_Api.Contracts;
using BookStore_Api.Data;
using BookStore_Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IMapper mapper, ILoggerService logger, IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Autors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted Get All Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDto>>(authors);
                _logger.LogInfo("SuccessFully got all Authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return this.InternalError(e);
            }
        }

        /// <summary>
        /// Get an author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An author's record</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var author = await _authorRepository.FindById(id);
                if (author != null)
                {
                    var response = _mapper.Map<AuthorDto>(author);
                    return Ok(response);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="authorDto"></param>
        /// <returns> Created author</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateDto authorDto)
        {

            try
            {
                if (authorDto == null)
                {
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDto);
                var isSuccess = await _authorRepository.Create(author);

                if (!isSuccess)
                {
                    var expcetionMessage = "Author Creation Failed";
                    _logger.LogWarn(expcetionMessage);
                    throw new Exception(expcetionMessage);
                }

                return Created("Create", new { author });

            }
            catch (Exception e)
            {

                return InternalError(e);
            }
        }
        
       /// <summary>
       /// Updates an author
       /// </summary>
       /// <param name="id"></param>
       /// <param name="authorDto"></param>
       /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDto authorDto)
        {

            try
            {
                if (authorDto == null)
                {
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDto);
                var isSuccess = await _authorRepository.Update(author);

                if (!isSuccess)
                {
                    var expcetionMessage = "Author Update Failed";
                    _logger.LogWarn(expcetionMessage);
                    throw new Exception(expcetionMessage);
                }

                return NoContent();

            }
            catch (Exception e)
            {

                return InternalError(e);
            }
        } 
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {

            try
            {
                if ( id<1)
                {
                    return BadRequest();
                }

                var author = await _authorRepository.FindById(id);

                if (author == null)
                {
                    return NotFound();
                }

              
                var isSuccess = await _authorRepository.Delete(author);

                if (!isSuccess)
                {
                    var expcetionMessage = "Author Delete Failed";
                    _logger.LogWarn(expcetionMessage);
                    throw new Exception(expcetionMessage);
                }

                return NoContent();

            }
            catch (Exception e)
            {

                return InternalError(e);
            }
        }

        private ObjectResult InternalError(Exception e)
        {
            _logger.LogError($"{e.Message}");
            return StatusCode(500, "Something went wrong. Contact the Administrator");

        }

    }
}
