namespace Catalog.Application.Common
{
    public class Result<T>
    {
        public T? Data { get; private set; }
        public List<string> Errors { get; private set; } = new();
        public bool IsSuccess => Errors.Count == 0;

        private Result(T data) => Data = data;
        private Result(List<string> errors) => Errors = errors;

        public static Result<T> Success(T data) => new(data);
        public static Result<T> Failure(List<string> errors) => new(errors);
    }
}