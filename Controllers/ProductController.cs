using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> Get(
        [FromServices] DataContext context
    )
    {

        var product = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();

        return Ok(product);
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Product>> GetByCategory(int id,
                [FromServices] DataContext context
    )
    {
        var product = await context
                                    .Products
                                    .Include(x => x.Category)
                                    .AsNoTracking()
                                    .Where(x => x.CategoryId == id)
                                    .ToListAsync();
        return Ok(product);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Product>> Post(
            [FromBody] Product model,
            [FromServices] DataContext context
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Products.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(ModelState);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Put(int id,
        [FromBody] Category model,
        [FromServices] DataContext context)
    {
        if (id != model.Id)
            return NotFound(new { message = "Categoria não encontrada." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível atualizar a categoria." });
        }

    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<string>> Delete(int id,
                [FromServices] DataContext context)
    {

        var categoria = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (categoria == null)
            return NotFound(new { message = "Categoria não encontrada." });

        try
        {
            context.Categories.Remove(categoria);
            await context.SaveChangesAsync();
            return Ok(new { message = string.Format("Categoria: '{0}' removida com sucesso.", categoria.Title) });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possivel remover a categoria." });
        }

    }
}