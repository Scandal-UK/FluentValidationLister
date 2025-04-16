// <copyright file="OptionalController.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample.Controllers;

using FluentValidationLister.WebApiSample.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary> Controller for the <see cref="Optional"/> model. </summary>
[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class OptionalController : ControllerBase
{
    /// <summary> Gets an instance of the <see cref="Optional"/> model. </summary>
    /// <returns>Instance of <see cref="ActionResult"/>.</returns>
    [HttpGet]
    public ActionResult<Optional> Get() => this.Ok(new Optional());

    /// <summary> Post a new instance of the <see cref="Optional"/> model. </summary>
    /// <param name="optional">Input model.</param>
    /// <returns>Instance of <see cref="ActionResult"/>.</returns>
    [HttpPost]
    public IActionResult Post(Optional optional) => this.Ok(optional);
}
