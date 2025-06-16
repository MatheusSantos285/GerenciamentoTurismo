using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GerenciamentoTurismo.Pages
{
    public class ViewNotesModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public ViewNotesModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<string> NoteFiles { get; set; } = new List<string>();

        public string NoteContent { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O tit�lo da nota � obrigat�rio.")]
            [Display(Name = "T�tulo da Nota")]
            public string Title { get; set; }

            [Required(ErrorMessage = "O conte�do da nota � obrigat�rio.")]
            [Display(Name = "Conte�do da Nota")]
            public string Content { get; set; }
        }

        private string FilesPath => Path.Combine(_environment.WebRootPath, "files");

        public void OnGet()
        {
            Directory.CreateDirectory(FilesPath);

            NoteFiles = Directory.GetFiles(FilesPath, "*.txt")
                                 .Select(Path.GetFileName)
                                 .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }
            var fileName = $"{Path.GetInvalidFileNameChars().Aggregate(Input.Title, (current, c) => current.Replace(c, '_'))}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            var filePath = Path.Combine(FilesPath, fileName);

            await System.IO.File.WriteAllTextAsync(filePath, Input.Content);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetViewNoteAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine(FilesPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // L� o conte�do do arquivo
            NoteContent = await System.IO.File.ReadAllTextAsync(filePath);

            // Recarrega a lista de arquivos para exibi��o
            OnGet();

            return Page();
        }
    }
}