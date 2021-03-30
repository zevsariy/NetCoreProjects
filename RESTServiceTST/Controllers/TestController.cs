using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace RESTServiceTST.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces(Application.Json)]
    [Route("api/v{api-version:apiVersion}/test")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Invert Number
        /// </summary>
        /// <param name="number">Number for inverting</param>
        /// <response code="200">Ok result.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Resource not found.</response>
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[HttpGet("number/{number}/inverted")]
		public ActionResult<int> InvertNumber([FromRoute]int number)
		{
            return TryInvertNumber(number);
		}

        /// <summary>
        /// Invert number
        /// </summary>
        /// <param name="number">Number for invertion</param>
        /// <returns>Inverted numver</returns>
        private int TryInvertNumber(int number)
        {
            return int.Parse(new string(number.ToString().Reverse().ToArray()));
        }
	}
}
