using System;
using System.Collections.Generic;

namespace DMendez.Application.Common.Models
{
    public enum ErrorType
    {
        None = 0,
        Failure = 1,
        NotFound = 2,
        Validation = 3,
        Conflict = 4
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public ErrorType ErrorType { get; }
        public List<string> Errors { get; }

        protected Result(bool isSuccess, string? error, ErrorType errorType, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
            Errors = errors ?? new List<string>();
        }

        public static Result Success() => new(true, null, ErrorType.None);
        public static Result Failure(string error) => new(false, error, ErrorType.Failure);
        public static Result NotFound(string error = "El recurso solicitado no fue encontrado.") => new(false, error, ErrorType.NotFound);
        public static Result Validation(List<string> errors, string error = "Ocurrieron uno o más errores de validación.") => new(false, error, ErrorType.Validation, errors);
        public static Result Conflict(string error) => new(false, error, ErrorType.Conflict);

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
        public static Result<T> NotFound<T>(string error = "El recurso solicitado no fue encontrado.") => Result<T>.NotFound(error);
        public static Result<T> Validation<T>(List<string> errors, string error = "Ocurrieron uno o más errores de validación.") => Result<T>.Validation(errors, error);
        public static Result<T> Conflict<T>(string error) => Result<T>.Conflict(error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected internal Result(bool isSuccess, T? value, string? error, ErrorType errorType, List<string>? errors = null)
            : base(isSuccess, error, errorType, errors)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, null, ErrorType.None);
        public static new Result<T> Failure(string error) => new(false, default, error, ErrorType.Failure);
        public static new Result<T> NotFound(string error = "El recurso solicitado no fue encontrado.") => new(false, default, error, ErrorType.NotFound);
        public static new Result<T> Validation(List<string> errors, string error = "Ocurrieron uno o más errores de validación.") => new(false, default, error, ErrorType.Validation, errors);
        public static new Result<T> Conflict(string error) => new(false, default, error, ErrorType.Conflict);

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
