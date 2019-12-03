using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Services;
using TravellerSpot.Model;
using System.Web.Http;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace TravellerSpot.Controllers
{
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonService _personService;
        
        public PersonController(PersonService personService)
        {
            _personService = personService;
        }

        // MAIN: get ALL data about persons in database
        [Route("api/person/getall")]
        public ActionResult<List<Person>> GetAll() => _personService.GetAll();

        //PERSON: Pokaż kogo obserwuje dana osoba
        [Route("api/person/whoisfollowedby/{nick}")]
        public ActionResult<List<Person>> WhoIsFollowedBy(string nick) => _personService.WhoIsFollowedBy(nick);



    }
}