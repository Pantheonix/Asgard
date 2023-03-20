global using Xunit;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Hosting;
global using Infrastructure;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Tests.Integration.Core;
global using Bogus;
global using Bogus.Extensions;
global using FastEndpoints;
global using System.Net;
global using FluentAssertions;
global using Api.Features.Auth.Register;
global using Testcontainers.MsSql;
global using Bootstrapper;
global using Bootstrapper.Extensions;
global using Api.Features.Auth.Login;
global using Api.Features.Users.GetAll;
global using Api.Features.Users.Get;
global using Api.Features.Users.Core;
global using Api.Features.Users.Update;
global using Api.Features.Users.Delete;
global using Microsoft.AspNetCore.Http;
global using System.Net.Http.Headers;
global using System.Net.Mime;
global using Domain.Entities;
global using Microsoft.AspNetCore.Identity;
global using Api.Features.Core;
global using Api.Features.Images.Get;
