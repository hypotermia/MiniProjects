using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniProjects.Interfaces;
using MiniProjects.Models;
using static MiniProjects.Repository.WebApiHelper;
using System;
using System.Threading.Tasks;
using MiniProjects.MediaTR;
using FluentValidation;

namespace MiniProjects.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        
        private readonly IMediator _mediator;
        private readonly IValidator<Product> _validator;
        public ProductsController(IMediator mediator, IValidator<Product> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }
        [HttpPost]
        public async Task<ApiResponseObj> InsertUpdateProducts([FromBody] Product product)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(product);

                if (!validationResult.IsValid)
                {
                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Validation failed",
                        transactionId = "",
                        data = validationResult.Errors,
                        status = false
                    };
                }
                var checkId = await _mediator.Send(new GetProductByIdQuery { Id = product.Id });
                if (checkId == null)
                {
                    var command = new AddProductCommand
                    {
                        ProductsName = product.ProductsName,
                        ProductsPrices = product.ProductsPrices,
                        Quantity = product.Quantity
                    };
                    await _mediator.Send(command);
                }
                else
                {
                    var command = new UpdateProductCommand
                    {
                        Id = product.Id,
                        ProductsName = product.ProductsName,
                        ProductsPrices = product.ProductsPrices,
                        Quantity = product.Quantity
                    };
                    await _mediator.Send(command);
                }

                return new ApiResponseObj
                {
                    Success = true,
                    message = "Success add/update Data!",
                    transactionId = "",
                    data = null,
                    status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj
                {
                    Success = true,
                    message = ex.Message,
                    transactionId = "",
                    data = null,
                    status = false
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponseObj> DeleteProducts(Guid uId)
        {
            try
            {
                var checkId = await _mediator.Send(new GetProductByIdQuery { Id = uId });
                if (checkId == null)
                {
                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Data tidak ditemukan!",
                        transactionId = null,
                        data = null,
                        status = true
                    };
                }
                else
                {
                    await _mediator.Send(new DeleteProductCommand { Id = uId });
                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Success Menghapus data!!",
                        transactionId = uId.ToString(),
                        data = null,
                        status = true
                    };
                }
            }
            catch(Exception ex)
            {
                return new ApiResponseObj
                {
                    Success = true,
                    message = ex.Message,
                    transactionId = "",
                    data = null,
                    status = false
                };
            }
        }

        [HttpGet]
        public async Task<ApiResponseObj> GetById(Guid uId)
        {
            try
            {
                var checkId = await _mediator.Send(new GetProductByIdQuery { Id = uId });
                if (checkId == null)
                {
                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Data tidak ditemukan!",
                        data = null,
                        transactionId= null,   
                        status = false
                    };
                }
                else
                {
                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Sukses Menampilkan Data!",
                        data = checkId,
                        transactionId = checkId.Id.ToString(),
                        status = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseObj
                {
                    Success = true,
                    message = ex.Message,
                    data = null,
                    transactionId = null,
                    status = false
                };
            }
        }
        [HttpGet]
        public async Task<ApiResponseObj> GetAllProducts()
        {
            try
            {
                var checkId = await _mediator.Send(new GetAllProductsQuery());
                if (checkId == null)
                {
                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Data kosong mohon isi data terlebih dahulu!",
                        data = null,
                        transactionId = null,
                        status = false
                    };
                }
                else
                {

                    return new ApiResponseObj
                    {
                        Success = true,
                        message = "Sukses Menampilkan Data!",
                        data = checkId,
                        transactionId = null,
                        status = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseObj
                {
                    Success = true,
                    message = ex.Message,
                    data = null,
                    transactionId = null,
                    status = false
                };
            }
        }
    }
}
