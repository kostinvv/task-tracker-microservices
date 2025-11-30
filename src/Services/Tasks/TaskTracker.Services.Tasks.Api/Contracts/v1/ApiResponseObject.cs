namespace TaskTracker.Services.Tasks.Api.Contracts.v1;

public record ApiResponseObject<TObject>(string Message, TObject Result);