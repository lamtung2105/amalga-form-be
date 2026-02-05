using api.Helpers;

namespace api.Models;

public class OrderRequest
{
    public Hospital? Hospital { get; set; }
    public Patient? Patient { get; set; }
    public List<OrderCategoryItem>? CategoryItems { get; set; } = [];
    public string? Priority { get; set; }
    public string? VisitCode { get; set; }
    public bool IsGeneral => Priority == "R";
    public bool IsUrgent => Priority == "S";
    public bool IsVeryUrgent => Priority == "W";
    public string? Diagnosis { get; set; }
    public string? RequestingTime { get; set; }
    public string? RequestingTimeLong { get; set; }
    public string? RequestingPlace { get; set; }
    public string? RequestingDoctor { get; set; }
    public string? PerformingPlace { get; set; }
}

public class Hospital
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
}

public class Patient
{
    public string? Pid { get; set; }
    public string? SexRcd { get; set; }
    public string Sex => SexRcd == "M" ? "Nam" : "Nữ";
    public string? Fullname { get; set; }
    public string? OfficialDocument { get; set; }
    public string? ParentName { get; set; }
    public string? Weight { get; set; }
    public string? Height { get; set; }

    public string? BMI
    {
        //tính BMI
        get
        {
            if (string.IsNullOrEmpty(Weight) || string.IsNullOrEmpty(Height))
            {
                return null;
            }
            if (double.TryParse(Weight, out double weightKg) && double.TryParse(Height, out double heightCm))
            {
                //db lưu cm(cần đổi)
                double heightM = heightCm / 100.0;
                if (heightM > 0)
                {
                    double bmi = weightKg / (heightM * heightM);
                    return bmi.ToString("F2");
                }
            }
            return null;
        }
    }
    public string? Nutrition { get; set; } //chưa có
    public string? Allergies { get; set; }
    public string? BHYT { get; set; } //chưa có
    public string? Dob { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? BarcodePid => BarcodeHelper.GenerateCode128A(Pid ?? "");
}

public class OrderCategoryItem
{
    public string? Name { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}

public class OrderItem
{
    public string? Priority { get; set; }
    public string? VisitCode { get; set; }
    public string? Category { get; set; }
    public string? Name { get; set; }
    public string? Diagnosis { get; set; }
    public string? RequestingTime { get; set; }
    public string? RequestingPlace { get; set; }
    public string? RequestingDoctor { get; set; }
    public string? PerformingPlace { get; set; }
    public string? CreatingTime { get; set; }
}