using AutoMapper;
using Contract.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Infrastructure.Data;
using SaleFishClean.Infrastructure.Services;

namespace SaleFishClean.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly SaleFishProjectContext _context;
        private readonly IProductServices _service;
        private readonly IInventoryServices _inventoryService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IValidator<Product> _validator;
        public ProductsController(SaleFishProjectContext context, IProductServices service, IMapper mapper, IWebHostEnvironment env, IValidator<Product> validator, IInventoryServices inventoryService)
        {
            _context = context;
            _service = service;
            _mapper = mapper;
            _env = env;
            _validator = validator;
            _inventoryService = inventoryService;
        }
        public async Task CreateProductImage(Product product)
        {
            product.ProductImage = await product.ImageFile.SaveImageAsync(_env);
        }
        // GET: Products
        public async Task<IActionResult> Index()
        
        {
            var productsEntities = await _service.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ProductRequest>>(productsEntities);
            return View(result);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _service.GetDetailAsync(id);
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountValue");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,BrandId,ProductTypeId,DiscountId,ProductName," +
            "Description,Price,Weight,Manufacturer,Color,Unit,Quantity,HasSpecialFeatures,IsNew,IsBestseller," +
            "IsOnSale,SpecialNote,ProductImage,SupplierId,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                ValidationResult result = await _validator.ValidateAsync(product);
                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }
                await CreateProductImage(product);
                await _service.CreateAsync(product);
                await _service.SaveChangesAsync();
                var newProduct = await _service.GetProductWithMaxIdAsync();
                if (newProduct != null)
                {
                    var productId = newProduct.ProductId;

                    var inventory = new Inventory
                    {
                        EntryDate = DateTime.Now,
                        ProductId = productId,
                        Quantity = product.Quantity,
                    };
                    await _inventoryService.CreateByModelsAsync(inventory);
                    await _service.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountId", product.DiscountId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _service.GetByIdAsync((int)id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountId", product.DiscountId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,BrandId,ProductTypeId,DiscountId,ProductName,Description,Price,Weight,Manufacturer,Color,Unit,Quantity, HasSpecialFeatures,IsNew,IsBestseller,IsOnSale,SpecialNote,ProductImage,SupplierId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(id, product);
                    await _service.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ( await ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountId", product.DiscountId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Discount)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.BeginTransactionAsync();
            try
            {
                var inventory = await _context.Inventories.SingleOrDefaultAsync(x => x.ProductId == id);
                if (inventory != null)
                {
                    await _inventoryService.DeleteAsync(inventory);
                }
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    await _service.DeleteAsync(product);
                }
                await _context.SaveChangesAsync();
                await _service.EndTransactionAsync();
            }
            catch (Exception)
            {
                await _service.RollBackTransactionAsync();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            var result = await _service.ProductExistsAsync(id);
            return result;
        }
    }
}
