namespace PlayStack_game_catalog_service.Catalog.Application.Common
{
    public class Result<T>
    {
        public T? Data { get; private set; }
        public List<string> Errors { get; private set; } = new();
        public bool IsSuccess => !Errors.Any();

        private Result(T data) => Data = data;
        private Result(List<string> errors) => Errors = errors;

        public static Result<T> Success(T data) => new(data);
        public static Result<T> Failure(List<string> errors) => new(errors);
    }
}