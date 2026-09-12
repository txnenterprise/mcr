using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Repository;

namespace MCR.API.Controllers
{
    public class SqlQueryController : Controller
    {
        private readonly DbContextMCR _context;

        public SqlQueryController(DbContextMCR context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Execute([FromBody] SqlQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Script))
            {
                return Json(new { success = false, error = "Por favor, insira um script SQL." });
            }

            try
            {
                var sqlUpper = request.Script.Trim().ToUpper();
                bool isQuery = sqlUpper.StartsWith("SELECT") ||
                              sqlUpper.StartsWith("WITH") ||
                              sqlUpper.StartsWith("SHOW") ||
                              sqlUpper.StartsWith("DESCRIBE") ||
                              sqlUpper.StartsWith("EXPLAIN");

                if (isQuery)
                {
                    var result = await _context.ExecuteSqlQueryAsync(request.Script);
                    return Json(new
                    {
                        success = true,
                        isQuery = true,
                        data = result,
                        rowsAffected = result?.Count ?? 0
                    });
                }
                else
                {
                    int rowsAffected = await _context.ExecuteSqlScriptAsync(request.Script);
                    return Json(new
                    {
                        success = true,
                        isQuery = false,
                        rowsAffected = rowsAffected,
                        message = $"Script executado com sucesso! {rowsAffected} linha(s) afetada(s)."
                    });
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" Detalhes: {ex.InnerException.Message}";
                }
                return Json(new { success = false, error = errorMessage });
            }
        }
    }

    public class SqlQueryRequest
    {
        public string Script { get; set; }
    }
}
