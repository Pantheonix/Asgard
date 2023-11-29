global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using Api.Features.Auth.RefreshToken;
global using Api.Features.Core;
global using Api.Features.Users.Core;
global using Application.Features.Users.CreateUser;
global using Application.Features.Users.ValidateUserCredentials;
global using AutoMapper;
global using Domain.Configs;
global using Domain.Entities;
global using Domain.Interfaces;
global using FastEndpoints;
global using FastEndpoints.Security;
global using FluentValidation;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using IMapper = AutoMapper.IMapper;
