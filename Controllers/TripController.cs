using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Models;
using TravellerSpot.Services;

namespace TravellerSpot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController : Controller
    {
        private readonly TripService _tripService;

        public TripController(TripService tripService)
        {
            _tripService = tripService;
        }

        // Creates a trip 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("CreateEmpty")]
        public ActionResult<string> CreateEmpty([FromQuery]Trip t, string personName) => Ok(_tripService.CreateEmpty(t, personName));


        // Creates a trip 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("GetTripsInProgress")]
        public ActionResult<List<Trip>> GetTripsInProgress([FromQuery] string personName) => Ok(_tripService.GetTripsInProgress(personName));

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("GetTripsFromObservedPersons")]
        public ActionResult<List<Trip>> GetTripsFromObservedPersons([FromQuery] string personName) => Ok(_tripService.GetTripsFromObservedPersons(personName));

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("GetTripsFrom")]
        public ActionResult<List<Trip>> GetTripsFrom([FromQuery] string personName, string followedName) => Ok(_tripService.GetTripsFrom(personName, followedName));

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("GetRandomTrips")]
        public ActionResult<List<Trip>> GetRandomTrips() => Ok(_tripService.GetRandomTrips());



    }
}
