
namespace DynamicFilter.Test;

internal sealed class JsonFileDataAttribute : DataAttribute
{
    private readonly string _filePath;

    private readonly Type _dataType;

    private readonly Type _resultType;

    public JsonFileDataAttribute(string filePath, Type dataType, Type resultType)
    {
        _filePath = filePath;
        _dataType = dataType;
        _resultType = resultType;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
        {
            throw new ArgumentNullException(nameof(testMethod));
        }

        var path = Path.IsPathRooted(_filePath)
            ? _filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        var fileData = File.ReadAllText(_filePath);
        return GetData(fileData);
    }

    private IEnumerable<object[]> GetData(string? jsonData)
    {
        var objectList = new List<object[]>();
        var specific = typeof(TestData<,>).MakeGenericType(_dataType, _resultType);
        var generic = typeof(List<>).MakeGenericType(specific);

        if (jsonData != null)
        {
            dynamic? datalist = JsonConvert.DeserializeObject(jsonData, generic);

            if (datalist != null)
            {
                foreach (var data in datalist)
                {
                    if (data != null)
                    {
                        objectList.Add(new object[] { data.Data, data.Result });
                    }
                }
            }
        }

        return objectList;
    }
}