namespace bmak_ecommerce.Application.Features.BusinessRules.Queries.GetBusinessRules
{
    public class GetBusinessRulesQuery
    {
        public BusinessRuleSpecParams Params { get; }

        public GetBusinessRulesQuery(BusinessRuleSpecParams @params)
        {
            Params = @params;
        }
    }
}
