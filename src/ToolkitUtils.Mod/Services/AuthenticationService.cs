// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using ToolkitUtils.Mod.Logging;

namespace ToolkitUtils.Mod.Services;

/// <summary>
///     Provides a device-based authentication service for managing authorization flows with an external
///     authentication provider.
/// </summary>
public sealed class AuthenticationService
{
    private const string ClientId = "9wyhma4y3tmx1haf19owy0u8jh7paj";
    private const string AuthenticationUrl = "https://id.twitch.tv/oauth2/device";
    private const string DeviceCodeGrantType = "urn:ietf:params:oauth:grant-type:device_code";
    private const string TokenUrl = "https://id.twitch.tv/oauth2/token";
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private static readonly RestClient Client = new();

    private static readonly Lazy<AuthenticationService> InstanceField = new(() => new AuthenticationService());
    private string _accessToken = "";
    private int _checkInterval = -1;
    private string _deviceCode = "";
    private string _refreshToken = "";
    private string[] _scopes = [];
    private string _userCode = "";
    private string _verificationUri = string.Empty;

    /// <summary>
    ///     Retrieves the singleton instance of the <see cref="AuthenticationService" />, facilitating centralized
    ///     management of device-based authentication flows.
    /// </summary>
    public static AuthenticationService Instance => InstanceField.Value;

    /// <summary>
    ///     Represents the most recently acquired access token used for authentication, enabling authorized interactions
    ///     with external services.
    /// </summary>
    public string AccessToken => _accessToken;

    /// <summary>
    ///     Represents the user code obtained during the device code flow, which the user needs to input in the
    ///     verification process to authorize the application.
    /// </summary>
    public string UserCode => _userCode;

    /// <summary>
    ///     Represents the device code obtained during the device authorization flow, which is used for polling the
    ///     authorization server to check the status of the user's authentication process.
    /// </summary>
    public string DeviceCode => _deviceCode;

    /// <summary>Indicates whether an authorization process, such as the device code flow, is currently active and ongoing.</summary>
    public bool AuthorizationInProgress { get; private set; }

    /// <summary>
    ///     Represents the verification URI where the user must navigate to complete the authentication process. This URI
    ///     is provided as part of the device code flow and facilitates user authentication by associating the device code with
    ///     the user's account.
    /// </summary>
    public string VerificationUri => _verificationUri;

    /// <summary>
    ///     Represents the timestamp indicating when the current authentication session or authorization code flow will
    ///     expire.
    /// </summary>
    public DateTimeOffset ExpiresIn { get; private set; } = DateTimeOffset.MinValue;

    /// <summary>
    ///     Initiates the device code flow for authentication by requesting a device code and user code from the
    ///     authentication endpoint.
    /// </summary>
    /// <param name="scopes">An array of strings representing the scopes that the application is requesting.</param>
    /// <returns>A URI for user verification, which the user should visit to complete the authorization process.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if a device code flow is already in progress or if the
    ///     authentication request fails.
    /// </exception>
    public async Task<Uri> InitiateDeviceCodeFlow(string[] scopes)
    {
        if (AuthorizationInProgress && !ExpiresIn.Equals(DateTimeOffset.UtcNow)) throw new InvalidOperationException("Device code flow is already in progress.");

        AuthorizationInProgress = true;

        IRestRequest request = new RestRequest(AuthenticationUrl, Method.POST).AddParameter(name: "client_id", ClientId)
                                                                              .AddParameter(name: "scopes", string.Join(separator: ' ', scopes));

        var response = await Client.PostAsync<DeviceCodeResponse>(request);

        if (response == null) throw new InvalidOperationException("Failed to initiate device code flow");

        string deviceCode = _deviceCode;
        Interlocked.CompareExchange(ref _deviceCode, response.DeviceCode, deviceCode);

        string userCode = _userCode;
        Interlocked.CompareExchange(ref _userCode, response.UserCode, userCode);

        _checkInterval = response.Interval;
        ExpiresIn = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);

        string[] scopesCopy = _scopes;
        Interlocked.CompareExchange(ref _scopes, scopes, scopesCopy);

        string verificationUri = _verificationUri;
        Interlocked.CompareExchange(ref _verificationUri, response.VerificationUri, verificationUri);

        _ = Task.Run(PollAuthorizationEndpoint);

        return new Uri(response.VerificationUri);
    }

    private async Task PollAuthorizationEndpoint()
    {
        while (AuthorizationInProgress)
        {
            await Task.Delay(TimeSpan.FromSeconds(_checkInterval));

            IRestRequest request = new RestRequest(TokenUrl, Method.POST).AddParameter(name: "client_id", ClientId).AddParameter(name: "scopes", value: "")
                                                                         .AddParameter(name: "device_code", _deviceCode).AddParameter(name: "grant_type", DeviceCodeGrantType);

            try
            {
                var response = await Client.PostAsync<TokenResponse>(request);

                string accessToken = _accessToken;
                Interlocked.CompareExchange(ref _accessToken, response.AccessToken, accessToken);

                string refreshToken = _refreshToken;
                Interlocked.CompareExchange(ref _refreshToken, response.RefreshToken, refreshToken);

                AuthorizationInProgress = false;
            }
            catch (Exception e)
            {
                Logger.Error(e, message: "Could not poll authorization endpoint");
            }
        }
    }

    private sealed record DeviceCodeResponse(
        [property: JsonProperty("device_code")] string DeviceCode,
        [property: JsonProperty("expires_in")] int ExpiresIn,
        [property: JsonProperty("interval")] int Interval,
        [property: JsonProperty("user_code")] string UserCode,
        [property: JsonProperty("verification_uri")] string VerificationUri
    );

    private sealed record TokenResponse(
        [property: JsonProperty("access_token")] string AccessToken,
        [property: JsonProperty("expires_in")] int ExpiresIn,
        [property: JsonProperty("refresh_token")] string RefreshToken,
        [property: JsonProperty("scope")] string[] Scope,
        [property: JsonProperty("token_type")] string TokenType
    );

    private sealed record DeviceErrorResponse([property: JsonProperty("message")] string Message, [property: JsonProperty("status")] int Status);
}
