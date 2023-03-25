global using FastEndpoints;
global using FluentValidation;
global using IMapper = AutoMapper.IMapper;
global using AutoMapper;
global using Microsoft.AspNetCore.Builder;
global using Application.Features.Users.ValidateUserCredentials;
global using Application.Features.Jwt.GenerateJwtToken;
global using Application.Features.Users.CreateUser;
global using Domain.Entities;
global using Microsoft.AspNetCore.Identity;
global using Api.Features.Users.Core;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Http;
global using Api.Features.Core;
global using Domain.Interfaces;
global using Microsoft.Extensions.Logging;
global using System.Security.Claims;
