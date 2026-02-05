using api.Models;
using api.Provider;
using HandlebarsDotNet;
using Microsoft.Data.SqlClient;
using System.Data;

namespace api.Services;

public class OrderService(ConnectionStringProvider provider)
{
    private readonly string connectionString = provider.ConnectionString;

    public async Task<string> RenderOrderRequest(string orderGroupNumber, string facilityCode, string pid)
    {
        var data = await LoadOrderRequestAsync(orderGroupNumber, facilityCode, pid);
        var templateHtml = await File.ReadAllTextAsync("Forms/order-request.html");

        var template = Handlebars.Compile(templateHtml);
        return template(data);
    }

    private async Task<OrderRequest> LoadOrderRequestAsync(string orderGroupNumber, string facilityCode, string pid)
    {
        var result = new OrderRequest();
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("[form].[OrderRequest]", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        // Parameters
        cmd.Parameters.AddWithValue("@orderGroupNumber", orderGroupNumber);
        cmd.Parameters.AddWithValue("@facilityCode", facilityCode);
        cmd.Parameters.AddWithValue("@patientVisibleId", pid);

        await conn.OpenAsync();

        // Hospital Info
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            result.Hospital = new Hospital
            {
                Code = reader["code"]?.ToString(),
                Name = reader["name"]?.ToString(),
                Address = reader["address"]?.ToString(),
                Phone = reader["phone"]?.ToString()
            };
        }
        // Patient Info
        await reader.NextResultAsync();
        if (await reader.ReadAsync())
        {
            result.Patient = new Patient
            {
                Pid = reader["pid"]?.ToString(),
                SexRcd = reader["sex_rcd"]?.ToString(),
                Fullname = reader["fullname"]?.ToString(),
                Dob = reader["dob"]?.ToString(),
                Address = reader["address"]?.ToString(),
                Phone = reader["phone"]?.ToString(),
            };
        }
        // Order Items
        await reader.NextResultAsync();
        var items = new List<OrderItem>();
        while (await reader.ReadAsync())
        {
            items.Add(new OrderItem
            {
                Priority = reader["order_priority"]?.ToString(),
                VisitCode = reader["visit_code"]?.ToString(),
                Category = reader["order_category"]?.ToString(),
                Name = reader["order_item"]?.ToString(),
                Diagnosis = reader["diagnosis"]?.ToString(),
                RequestingPlace = reader["requesting_place"]?.ToString(),
                RequestingTime = reader["requesting_time"]?.ToString(),
                RequestingDoctor = reader["requesting_doctor"]?.ToString(),
                PerformingPlace = reader["performing_place"]?.ToString(),
                CreatingTime = reader["creating_time"]?.ToString(),
            });
        }
        result.Priority = items.FirstOrDefault()?.Priority;
        result.VisitCode = items.FirstOrDefault()?.VisitCode;
        result.Diagnosis = items.FirstOrDefault()?.Diagnosis;
        result.RequestingTime = items.FirstOrDefault()?.RequestingTime;
        result.RequestingTimeLong = GetRequestingTimeLong(result.RequestingTime ?? "");
        result.RequestingPlace = items.FirstOrDefault()?.RequestingPlace;
        result.RequestingDoctor = items.FirstOrDefault()?.RequestingDoctor;
        result.CategoryItems = [.. items.GroupBy(x => x.Category).Select(g => new OrderCategoryItem { Name = g.Key, Items = [.. g.OrderBy(x => x.CreatingTime)] })];

        return result;
    }

    private static string GetRequestingTimeLong(string requestingTime)
    {
        if (string.IsNullOrEmpty(requestingTime)) return "";
        DateTime date = DateTime.ParseExact(requestingTime, "dd/MM/yyyy HH:mm:ss", null);
        return $"Ngày {date.Day:D2} tháng {date.Month:D2} năm {date.Year}";
    }
}
