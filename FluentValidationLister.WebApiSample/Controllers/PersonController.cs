namespace FluentValidationLister.WebApiSample.Controllers
{
    using System;
    using FluentValidationLister.WebApiSample.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        /// <summary>Add a person.</summary>
        /// <param name="person">Input parameters.</param>
        /// <response code="200">Successful post.</response>
        /// <response code="400">Invalid payload.</response>
        /// <returns>Instance of <see cref="OkResult"/> with a Message property.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult AddPerson(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            return this.Ok(new { Message = $"Person ({person.Forename} {person.Surname}) has been validated successfully!" });
        }
    }
}