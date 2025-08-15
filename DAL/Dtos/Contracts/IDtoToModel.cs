namespace DAL.Dtos.Contracts;

public interface IDtoToModel<in TDto, out TModel>
    where TModel : class
    where TDto : class
{
    /// <summary>
    ///     Maps the DTO to Model
    /// </summary>
    /// <param name="dto">The DTO to map.</param>
    /// <returns>The mapped model.</returns>
    static abstract TModel MapToModel(TDto dto);
}