using ErrSendApplication.DTO;
using ErrSendApplication.Interfaces;
using ErrSendWebApi.Serviсe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ErrSendWebApi.Controllers
{
    /// <summary>
    /// Контролер для автентифікації та управління токенами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : BaseController
    {
        private readonly IJwtService jwtService;
        private readonly IUserService userService;

        public AuthController(IJwtService jwtService, IUserService userService)
        {
            this.jwtService = jwtService;
            this.userService = userService;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        /// <param name="request">Дані для реєстрації</param>
        /// <returns>Результат реєстрації</returns>
        /// <response code="200">Користувач успішно зареєстрований</response>
        /// <response code="400">Помилка валідації або користувач вже існує</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (await userService.UserExistsAsync(request.Username))
            {
                return BadRequest("Користувач з таким ім'ям вже існує");
            }

            var success = await userService.RegisterUserAsync(request);
            if (!success)
            {
                return BadRequest("Помилка при реєстрації користувача");
            }

            var token = jwtService.GenerateToken(request.Username);
            return Ok(new AuthResponse { Token = token });
        }

        /// <summary>
        /// Автентифікація користувача та отримання JWT токена
        /// </summary>
        /// <param name="request">Дані для автентифікації (логін та пароль)</param>
        /// <returns>JWT токен для подальшої авторизації</returns>
        /// <response code="200">Успішна автентифікація, повертає JWT токен</response>
        /// <response code="401">Невірний логін або пароль</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            var user = await userService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized("Невірний логін або пароль");
            }

            var token = jwtService.GenerateToken(request.Username);
            return Ok(new AuthResponse { Token = token });
        }

        /// <summary>
        /// Генерація тестового JWT токена з користувацькими параметрами
        /// </summary>
        /// <param name="request">Параметри для генерації токена</param>
        /// <returns>JWT токен з вказаними параметрами</returns>
        /// <response code="200">Токен успішно згенеровано</response>
        [HttpPost("generate-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        public ActionResult<AuthResponse> GenerateToken([FromBody] GenerateTokenRequest request)
        {
            var token = jwtService.GenerateCustomToken(request);
            return Ok(new AuthResponse { Token = token });
        }
    }
} 