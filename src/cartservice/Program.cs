// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using cartservice.cartstore;
using cartservice.interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cartservice
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();
            string redisAddress = Environment.GetEnvironmentVariable("REDIS_ADDR");
            if (string.IsNullOrEmpty(redisAddress))
            {
                Console.WriteLine("Redis cache host(hostname+port) was not specified. Starting a cart service using local store");
                Console.WriteLine("If you wanted to use Redis Cache as a backup store, you should provide its address via REDIS_ADDRESS environment variable.");
                builder.Services.AddSingleton<ICartStore, LocalCartStore>();
            }
            else
            {
                builder.Services.AddSingleton<ICartStore>(new RedisCartStore(redisAddress));
            }


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.MapGrpcService<CartServiceImpl>();
            app.MapGrpcService<HealthImpl>();


            string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Run("http://0.0.0.0:" + port);
        }
    }
}
