using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Services;
using TravellerSpot.Model;

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

        // MAIN: get ALL data about persons in database

        public ActionResult<List<Person>> Get() => _personService.GetAll();

        // SEARCH: get a person with nick alike...

        //public ActionResult<List<Person>> GetPersonsWithNickLike() => _personService.GetPersonsWithNickLike();


    }
}