﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Models;
using TravellerSpot.Services;

namespace TravellerSpot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotController : ControllerBase
    {
        private readonly SpotService _spotService;

        public SpotController(SpotService spotService)
        {
            _spotService = spotService;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("PostSpotToTravel")]
        public ActionResult<string> PostSpotToTrip([FromQuery]Spot spot, string personName, string tripName) => Ok(_spotService.PostSpotToTrip(spot,personName,tripName));
    }
}
