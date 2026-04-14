using Core.Entities;
namespace Core.Specifications;

public class BrandListSpec : BaseSpecification<Product, string>
{
    public BrandListSpec()
    {
        AddSelect(x => x.Brand);
        ApplyDistinct();
    }
}
