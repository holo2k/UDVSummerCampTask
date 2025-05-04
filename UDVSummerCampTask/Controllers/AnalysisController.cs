using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;
using UDVSummerCampTask.Models;
using UDVSummerCampTask.Services;
using UDVSummerCampTask.Services.Analysis;
using UDVSummerCampTask.Services.Letter;

namespace UDVSummerCampTask.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyzeController : ControllerBase
    {
        private readonly VkService vkService;
        private readonly IAnalysisService analysisService;
        private readonly ILetterService letterService;
        private readonly ILogger<AnalyzeController> logger;

        public AnalyzeController(
            VkService vkService, 
            ILogger<AnalyzeController> logger, 
            IAnalysisService analysisService,
            ILetterService letterService)
        {
            this.vkService = vkService;
            this.logger = logger;
            this.analysisService = analysisService;
            this.letterService = letterService;
        }

        /// <summary>
        /// Анализирует последние посты пользователя ВК и подсчитывает частоту букв.
        /// </summary>
        /// <remarks>
        /// Этот метод использует VK API для получения последних постов пользователя и анализирует их содержимое.
        /// Результаты анализа частоты букв сохраняются в базу данных.
        /// </remarks>
        /// <param name="userId">Идентификатор пользователя ВК.</param>
        /// <returns>Список частот букв, отсортированный по алфавиту.</returns>
        [HttpPost("process")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Анализирует последние посты пользователя ВК и подсчитывает частоту букв.")]
        [SwaggerResponse(200, "Анализ завершён успешно", typeof(List<LetterFrequency>))]
        [SwaggerResponse(400, "Ошибка при анализе или сохранении данных")]
        public async Task<IActionResult> Analyze([FromQuery] string userId)
        {
            logger.LogInformation("Анализ запущен для: {Id}", userId);

            List<Post> posts;
            try
            {
                posts = await vkService.GetLastPostsAsync(userId);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Ошибка VK API: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }

            logger.LogInformation("Анализ завершён, постов получено: {Count}", posts.Count);

            var result = analysisService.CountLetters(posts.Select(x=>x.Content));

            try
            {
                await letterService.AddLetters(result, userId);
            }
            catch(Exception ex)
            {
                logger.LogWarning("Ошибка при добавлении в базу: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }

            return Ok(result.OrderBy(f => f.Letter));
        }

        /// <summary>
        /// Получает частоту букв по ID пользователя из базы данных.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя ВК.</param>
        /// <returns>Список частот букв для указанного пользователя.</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Получает частоту букв по ID пользователя из базы данных.")]
        [SwaggerResponse(200, "Данные частоты букв успешно получены", typeof(List<LetterFrequency>))]
        [SwaggerResponse(400, "Ошибка при получении данных из базы")]
        public IActionResult GetByUserId(string userId)
        {
            try
            {
                var freqs = letterService.GetByUserId(userId);
                return Ok(freqs);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Ошибка при получении из базы: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
