using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Services;
using TravellerSpot.Models;
using Microsoft.AspNetCore.Http;

namespace TravellerSpot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly PersonService _personService;

        public PersonController(PersonService personService)
        {
            _personService = personService;
        }

        // Creates a person 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("AddPerson")]
        public ActionResult<string> Create([FromQuery]Person p) => _personService.Create(p);

        [HttpGet]
        // Get ALL data about persons in database
        public ActionResult<List<Person>> Get() => _personService.GetAll();

        [HttpPost]
        [Route("FollowPerson")]
        public ActionResult<string> FollowPerson([FromHeader] string personName,string followedName) => _personService.FollowPerson(personName,followedName);

        // Get all countries in which the person queried was
        //public ActionResult<List<Country>> GetVisitedCountries() => _personService.GetVisitedCountries();

        //// Get people who follow queried person
        //public ActionResult<List<Person>> GetFollowedPeopleBy() => _personService.GetFollowedPeopleBy();

        //// Get average stars number based on persons all travels
        //public ActionResult<List<Person>> GetAveragePersonTravelsRating() => _personService.GetAveragePersonTravelsRating();

        //// Create person in Database
        //public IActionResult PostNewPerson(Person p) => _personService.PostNewPerson();

    }
}