using Core.Entities;
namespace Core.Specifications;

public class TypeListSpec : BaseSpecification<Product, string>
{
    public TypeListSpec()
    {
        AddSelect(x => x.Type);
        ApplyDistinct();
    }
}
