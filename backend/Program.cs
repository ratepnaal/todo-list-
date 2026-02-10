using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using AspSqlProject.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. إعداد سياسة الـ CORS
// ملاحظة: قمنا بتسميتها DevPolicy وجعلناها مرنة لتجنب مشاكل المنفذ (Port) أثناء التطوير
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.AllowAnyOrigin()  // يسمح بالوصول من أي مكان (مناسب للتطوير)
              .AllowAnyMethod()  // يسمح بـ GET, POST, PUT, DELETE
              .AllowAnyHeader(); // يسمح بجميع الرؤوس مثل Content-Type
    });
});

// 2. إعداد الاتصال بقاعدة البيانات SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. إضافة خدمات الـ Controllers مع توحيد شكل JSON (camelCase) لتطابق React
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

// 4. إعداد Swagger (اختياري لكنه أساسي لاختبار الـ API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. إعداد خط المعالجة (Middleware Pipeline) - الترتيب هنا "حرج جداً"
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// الترتيب الصحيح للميدل وير لضمان عمل الربط:
app.UseRouting();

// يجب أن يكون UseCors بعد UseRouting وقبل أي شيء آخر يخص البيانات أو الصلاحيات
app.UseCors("DevPolicy");

// إذا كان الطالب يستخدم HTTP في الرياكت، يفضل تعطيل Redirection مؤقتاً أو التأكد من إعدادها
// app.UseHttpsRedirection(); 

app.UseAuthorization();

// ربط المسارات بالكنترولر
app.MapControllers();

app.Run();