// <copyright file="PersonController.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample.Controllers;

using System;
using System.Web;
using FluentValidationLister.WebApiSample.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary> Controller for the <see cref="Person"/> entity. </summary>
[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    /// <summary>Gets a person based on the supplied ID.</summary>
    /// <param name="id">ID number of the <see cref="Person"/>.</param>
    /// <response code="200">Returns an instance of <see cref="Person"/>.</response>
    /// <response code="404">The supplied ID was not found.</response>
    /// <returns>Instance of <see cref="Person"/>.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Person), 200)]
    [ProducesResponseType(typeof(NotFoundResult), 404)]
    public ActionResult<Person> Get(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);

        if (id > 3)
        {
            return this.NotFound();
        }

        return this.Ok(new Person
        {
            Id = id,
            Surname = "Sample",
            Forename = "Bob",
            Email = "bob.sample@email.com",
            Age = 21,
            SaleOfSoulAgreed = true,
            Address = new()
            {
                Line1 = "123 Sample Street",
                Town = "Sample Town",
                County = "Sampleshire",
                Postcode = "SA1 4PL",
            },
        });
    }

    /// <summary>Add a person.</summary>
    /// <param name="person">Input parameters.</param>
    /// <response code="200">Successful post.</response>
    /// <response code="400">Invalid payload.</response>
    /// <returns>Instance of <see cref="OkResult"/> with a Message property.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public IActionResult Post(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        var fullName = $"{person.Forename} {person.Surname}";
        return this.Ok(new { Message = $"Person ({HttpUtility.HtmlEncode(fullName)}) has been validated successfully!" });
    }
}