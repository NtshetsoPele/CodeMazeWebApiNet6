using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.ModelBinders;

public class ArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // We are creating a model binder for the IEnumerable type.
        // Therefore, we have to check if our parameter is the same type.
        if(!bindingContext.ModelMetadata.IsEnumerableType)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
        // Next, we extract the value (a comma-separated string of GUIDs)
        // with the ValueProvider.GetValue() expression.
        var providedValue = bindingContext.ValueProvider
            .GetValue(bindingContext.ModelName)
            .ToString();
        // Because it is a type string, we just check whether it is null
        // or empty. If it is, we return null as a result because we have
        // a null check in our action in the controller. If it is not,
        // we move on.
        if(string.IsNullOrEmpty(providedValue))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }
        // In the genericType variable, with the help of reflection, we
        // store the type the IEnumerable consists of. In our case, it
        // is GUID.
        var genericType = 
            bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
        // Creates a converter to a GUID type.
        // As you can see, we didn’t just force the GUID type in
        // this model binder; instead, we inspected what is the nested
        // type of the IEnumerable parameter and then created a converter
        // for that exact type, thus making this binder generic.
        var converter = TypeDescriptor.GetConverter(genericType);
        // Create an array of type object (objectArray) that
        // consist of all the GUID values we sent to the API and then
        // create an array of GUID types (guidArray), copy all the values
        // from the objectArray to the guidArray, and assign it to the
        // bindingContext.
        var objectArray = providedValue.Split(new[] { "," }, 
                StringSplitOptions.RemoveEmptyEntries)
            .Select(x => converter.ConvertFromString(x.Trim()))
            .ToArray();
        var guidArray = Array.CreateInstance(genericType, objectArray.Length);
        objectArray.CopyTo(guidArray, 0);
        bindingContext.Model = guidArray;
        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
        return Task.CompletedTask;
    } 
}