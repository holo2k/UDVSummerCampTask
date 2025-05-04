using Microsoft.AspNetCore.Mvc;
using UDVSummerCampTask.DAL;
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

        [HttpPost("process")]
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

        [HttpGet("{userId}")]
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
