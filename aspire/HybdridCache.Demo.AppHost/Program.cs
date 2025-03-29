var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.Movies_Api>("movies-api")
    .WithReplicas(5)
    .WithReference(redis)
    .WaitFor(redis);

builder.Build().Run();
