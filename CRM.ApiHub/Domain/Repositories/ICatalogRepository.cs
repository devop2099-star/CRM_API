using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface ICatalogRepository
{
    Task<IEnumerable<OrderStatus>> GetOrderStatusesAsync();
    Task<IEnumerable<OrderSubstatus>> GetOrderSubstatusesAsync(int idStatus);
    Task<IEnumerable<Product>> GetProductsAsync(int idCmpg);
    
    // Si no existe un modelo Currency, usamos un objeto dinámico o DTO temporal
    Task<IEnumerable<dynamic>> GetCurrenciesAsync(); 
}