using API_CARGA.ViewModel;
using Swashbuckle.AspNetCore.Filters;

namespace API_CARGA.ModelExamples
{
    public class AddShapeConfigResponseError : IExamplesProvider<ErrorExample>
    {
        public ErrorExample GetExamples()
        {
            return new ErrorExample
            {
                Error = "Check that shape config with id {identifier} exist"
            };
        }
    }
}
