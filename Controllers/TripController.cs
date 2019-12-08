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
        public ActionResult<Trip> CreateEmpty([FromQuery]Trip t,[FromHeader] string p) => Ok(_tripService.CreateEmpty(t,p));


        // Creates a trip 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("GetTripsInProgress")]
        public ActionResult<List<Trip>> GetTripsInProgress([FromHeader] string p) => Ok(_tripService.GetTripsInProgress(p));



    }
}
