using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Response;

namespace CompanyEmployees.Formatters;

public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }
    
    protected override bool CanWriteType(Type? type)
    {
        return (typeof(CompanyDto).IsAssignableFrom(type) ||
                typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type)) && base.CanWriteType(type);
    }
    
    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var response = context.HttpContext.Response;
        var buffer = new StringBuilder();
        if (context.Object is IEnumerable<CompanyDto> CompanyDtos)
        {
            foreach (var company in CompanyDtos)
            {
                FormatCsv(buffer, company);
            }
        }
        else
        {
            FormatCsv(buffer, (CompanyDto)context.Object);
        }
        await response.WriteAsync(buffer.ToString());
    }
    private static void FormatCsv(StringBuilder buffer, CompanyDto company)
    { 
        buffer.AppendLine($"{company.Id}\",\"{company.Name}\",\"{company.FullAddress}\"");
    }
}