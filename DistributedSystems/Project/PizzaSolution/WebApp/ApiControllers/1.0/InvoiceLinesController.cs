using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.BLL.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
using Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PublicApi.DTO.v1;
using PublicApi.DTO.v1.Mappers;

namespace WebApp.ApiControllers._1._0
{
    /// <summary>
    /// Controller for InvoiceLines
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvoiceLinesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly InvoiceLineDTOMapper _mapper = new InvoiceLineDTOMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll"></param>
        public InvoiceLinesController(IAppBLL bll)
        {
            _bll = bll;
        }
        
        // GET: api/InvoiceLines
        /// <summary>
        /// Get the list of all InvoiceLines for specific user.
        /// </summary>
        /// <returns>List of InvoiceLines</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceLineDTO>>> GetInvoiceLines()
        {
            var invoiceLines = (await _bll.InvoiceLines.GetAllAsync(User.UserGuidId()))
                .Select(bllEntity => _mapper.Map(bllEntity));
            
            return Ok(invoiceLines);
        }

        // GET: api/InvoiceLines/5
        /// <summary>
        /// Get single InvoiceLine by given id
        /// </summary>
        /// <param name="id">Id of the InvoiceLine that we are returning</param>
        /// <returns>InvoiceLine</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceLineDTO>> GetInvoiceLine(Guid id)
        {
            var invoiceLine = await _bll.InvoiceLines.FirstOrDefaultAsync(id, User.UserGuidId());

            if (invoiceLine == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(invoiceLine));
        }

        // PUT: api/InvoiceLines/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Change existing InvoiceLine by given ID
        /// </summary>
        /// <param name="id">Given ID that we use to find the InvoiceLine from DB</param>
        /// <param name="invoiceLineDTO">DTO with new values tha we need to change</param>
        /// <returns>Nothing</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceLine(Guid id, InvoiceLineDTO invoiceLineDTO)
        {
            if (id != invoiceLineDTO.Id)
            {
                return BadRequest();
            }

            await _bll.InvoiceLines.UpdateAsync(_mapper.Map(invoiceLineDTO));
            await _bll.SaveChangesAsync();

            return NoContent();

        }

        // POST: api/InvoiceLines
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Add a new InvoiceLine to the DB.
        /// </summary>
        /// <param name="invoiceLineDTO">DTO with the values for the record tha will be inserted into DB.</param>
        /// <returns>DTO with the values from the record that was added to the DB.</returns>
        [HttpPost]
        public async Task<ActionResult<InvoiceLineDTO>> PostInvoiceLine(InvoiceLineDTO invoiceLineDTO)
        {
            var bllEntity = _mapper.Map(invoiceLineDTO);
            _bll.InvoiceLines.Add(bllEntity);
            await _bll.SaveChangesAsync();

            invoiceLineDTO.Id = bllEntity.Id;

            return Ok(invoiceLineDTO);
        }

        // DELETE: api/InvoiceLines/5
        /// <summary>
        /// Deletes a InvoiceLine record from the DB by id.
        /// </summary>
        /// <param name="id">Id for the record that will be removed from the DB.</param>
        /// <returns>:)</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<InvoiceLineDTO>> DeleteInvoiceLine(Guid id)
        {
            var invoiceLine = await _bll.InvoiceLines.FirstOrDefaultAsync(id, User.UserGuidId());
            if (invoiceLine == null)
            {
                return NotFound();
            }

            await _bll.InvoiceLines.RemoveAsync(id);
            await _bll.SaveChangesAsync();

            return Ok(_mapper.Map(invoiceLine));
        }
        
    }
}
