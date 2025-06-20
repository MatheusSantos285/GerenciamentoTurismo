using GerenciamentoTurismo.Data;
using GerenciamentoTurismo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoTurismo.Pages.PacotesTuristicos
{
    public class DetailsModel : PageModel
    {
        private readonly AgenciaTurismoContext _context;

        public DetailsModel(AgenciaTurismoContext context)
        {
            _context = context;
        }

        // A propriedade para armazenar os dados do pacote a ser exibido
        public PacoteTuristico PacoteTuristico { get; set; }

        // O m�todo OnGetAsync recebe o par�metro 'id' da rota.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                // Se nenhum ID for fornecido na URL, retorna um erro 404.
                return NotFound();
            }

            // Busca o PacoteTuristico pelo ID.
            // Usamos 'Include' e 'ThenInclude' para carregar os dados relacionados (eager loading).
            // Isso evita o problema de "N+1 queries" e garante que teremos todos os dados necess�rios.
            PacoteTuristico = await _context.PacotesTuristicos
                .Include(p => p.PacoteDestinos) // Inclui a tabela de associa��o
                    .ThenInclude(pd => pd.CidadeDestino) // A partir da associa��o, inclui a Cidade
                        .ThenInclude(c => c.Pais) // A partir da Cidade, inclui o Pa�s
                .FirstOrDefaultAsync(m => m.Id == id); // Encontra o pacote com o ID correspondente

            if (PacoteTuristico == null)
            {
                // Se nenhum pacote com esse ID for encontrado no banco, retorna um erro 404.
                return NotFound();
            }

            // Se tudo deu certo, exibe a p�gina.
            return Page();
        }
    }
}
