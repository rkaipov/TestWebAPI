namespace TestWebAPI.DataModels;

[Flags]
public enum OrderStatus
{
  New = 1,
  InProgress = 2,
  Completed = 4,
  Archived = 8,
}