using MediaBrowser.Common.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Template.Controllers
{
    /// <summary>
    /// Sample controller for the plugin.
    /// </summary>
    [ApiController]
    [Route("MyPlugin")]
    [Authorize]
    public class MyPluginController : ControllerBase
    {
        /// <summary>
        /// Pings the plugin.
        /// </summary>
        /// <returns>pong.</returns>
        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Admin only endpoint.
        /// </summary>
        /// <returns>Admin greeting.</returns>
        [HttpGet("admin-only")]
        [Authorize(Policy = Policies.RequiresElevation)]
        public ActionResult<string> AdminOnly()
        {
            return Ok("Hello, admin!");
        }

        /// <summary>
        /// Local only endpoint.
        /// </summary>
        /// <returns>Local greeting.</returns>
        [HttpGet("local-only")]
        [Authorize(Policy = Policies.LocalAccessOnly)]
        public ActionResult<string> LocalOnly()
        {
            return Ok("Hello, local user!");
        }
    }
}
