var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Movies_Api>("movies-api");

builder.Build().Run();
