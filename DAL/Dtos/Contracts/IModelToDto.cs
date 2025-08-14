namespace DAL.Dtos;

public interface IModelToDto<TModel, TDto>
    where TModel : class
    where TDto : class
{
    
    /// <summary>
    /// Maps the model to a DTO.
    /// </summary>
    /// <param name="model">The DTO to map.</param>
    /// <returns>The mapped model.</returns>
    static abstract TDto MapToDto(TModel model);
}