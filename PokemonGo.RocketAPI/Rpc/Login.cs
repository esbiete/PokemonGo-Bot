﻿#region using directives

using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Networking.Platform.Responses;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Login;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Responses;
using POGOProtos.Networking.Platform;
using POGOProtos.Networking.Requests.Messages;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public delegate void GoogleDeviceCodeDelegate(string code, string uri);

    public class Login : BaseRpc
    {
        //public event GoogleDeviceCodeDelegate GoogleDeviceCodeEvent;
        private readonly ILoginType _login;

        public Login(Client client) : base(client)
        {
            _login = SetLoginType(client.AuthType, client.Username,client.Password);
            Client.ApiUrl = Resources.RpcUrl;
        }

        private static ILoginType SetLoginType(AuthType type, string username, string password)
        {
            switch (type)
            {
                case AuthType.Google:
                    return new GoogleLogin(username, password);
                case AuthType.Ptc:
                    return new PtcLogin(username, password);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Unknown AuthType");
            }
        }

        public async Task DoLogin()
        {
            Client.AuthToken = await _login.GetAccessToken().ConfigureAwait(false);
            
            if ( Client.AuthToken == null){
                throw new LoginFailedException("Connection with Server failed. Please, check if niantic servers are up");
            }
                
            
            Client.StartTime = Utils.GetTime(true);
            
            var deviceInfo = DeviceSetup.SelectedDevice.DeviceInfo;

            await
                FireRequestBlock(CommonRequest.GetPlayerMessageRequest())
                    .ConfigureAwait(false);

            await RandomHelper.RandomDelay(2000).ConfigureAwait(false);
            Client.Map.GetMapObjects().Wait();
            /*
            Client.Download.GetRemoteConfigVersion(Client.AppVersion,deviceInfo.HardwareManufacturer,deviceInfo.DeviceModel, "", Client.Platform);
            await RandomHelper.RandomDelay(300).ConfigureAwait(false);
            Client.Download.GetAssetDigest(Client.AppVersion,deviceInfo.HardwareManufacturer,deviceInfo.DeviceModel, "", Client.Platform);
            await RandomHelper.RandomDelay(300).ConfigureAwait(false);
            Client.Download.GetItemTemplates();
            await RandomHelper.RandomDelay(300).ConfigureAwait(false);
            Client.Player.GetPlayerProfile(Client.Username);
            await RandomHelper.RandomDelay(300).ConfigureAwait(false);
            */

        }
        public async Task Login2()
        {
            Request [] requests  = {
                    new Request
                    {
                        RequestType = RequestType.GetPlayer
                    },
                    new Request
                    {
                        RequestType = RequestType.CheckChallenge,
                        RequestMessage = new CheckChallengeMessage
                        {
                            DebugRequest = false
                        }.ToByteString()
                    }
            };
            GetPlayerResponse playerResponse;
            var tries = 5;
            do
            {
                var request = GetRequestBuilder().GetRequestEnvelope(requests, true);
                
                Tuple<GetPlayerResponse, CheckChallengeResponse> response =
                    await
                        PostProtoPayload
                            <Request, GetPlayerResponse, CheckChallengeResponse>(request).ConfigureAwait(false);
    
                CheckChallengeResponse checkChallengeResponse = response.Item2;
                CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
                playerResponse = response.Item1;
                if (!playerResponse.Success)
                {
                    Logger.Debug("playerResponse: " + playerResponse);
                    await Task.Delay(1000);
                }
                tries --;
            } while (!playerResponse.Success && tries > 0);
            if ( playerResponse.Banned)
                Logger.Error("Error: This account seems be banned");
            
            if ( playerResponse.Warn)
                Logger.Warning("Warning: This account seems be flagged");
        }
        
        public async Task FireRequestBlock(Request request)
        {
            Logger.Debug("Client.ApiUrl: " + Client.ApiUrl);
            //var requests = CommonRequest.FillRequest(request, Client);
            var requests = CommonRequest.AddChallengeRequest(request, Client);

            var ll = new RequestBuilder(Client, Client.AuthToken, Client.AuthType, Client.CurrentLatitude, Client.CurrentLongitude, Client.CurrentAltitude);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests, true);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            var platfResponses = serverResponse.PlatformReturns;
            if (platfResponses != null)
            {
                var mapPlatform = platfResponses.FirstOrDefault(x => x.Type == PlatformRequestType.UnknownPtr8);
                if (mapPlatform != null)
                {
                    var unknownPtr8Response = UnknownPtr8Response.Parser.ParseFrom(mapPlatform.Response);
                    Resources.Api.UnknownPtr8Message = unknownPtr8Response.Message;
                    Logger.Debug("Receiving unknownPtr8Response: " + unknownPtr8Response.Message);
                }
            }

            switch (serverResponse.StatusCode)
            {
                case ResponseEnvelope.Types.StatusCode.SessionInvalidated:
                case ResponseEnvelope.Types.StatusCode.InvalidAuthToken:
                    Client.AuthToken = null;
                    throw new AccessTokenExpiredException();
                case ResponseEnvelope.Types.StatusCode.Redirect:
                    // 53 means that the api_endpoint was not correctly set, should be at this point, though, so redo the request
                    if (!string.IsNullOrEmpty(serverResponse.ApiUrl)){
                        Client.ApiUrl = "https://" + serverResponse.ApiUrl + "/rpc";
                        Logger.Debug("New Client.ApiUrl: " + Client.ApiUrl);
                    }
                    Logger.Debug("Redirecting");
                    await FireRequestBlock(request).ConfigureAwait(false);
                    return;
                case ResponseEnvelope.Types.StatusCode.BadRequest:
                    // Your account may be banned! please try from the official client.
                    throw new LoginFailedException("Your account may be banned! please try from the official client.");
                case ResponseEnvelope.Types.StatusCode.Unknown:
                    break;
                case ResponseEnvelope.Types.StatusCode.Ok:
                    break;
                case ResponseEnvelope.Types.StatusCode.OkRpcUrlInResponse:
                    if (!string.IsNullOrEmpty(serverResponse.ApiUrl)){
                        Client.ApiUrl = "https://" + serverResponse.ApiUrl + "/rpc";
                        Logger.Debug("New Client.ApiUrl: " + Client.ApiUrl);
                    }
                    break;
                case ResponseEnvelope.Types.StatusCode.InvalidRequest:
                    break;
                case ResponseEnvelope.Types.StatusCode.InvalidPlatformRequest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (serverResponse.AuthTicket != null){
                Client.AuthTicket = serverResponse.AuthTicket;
                Logger.Debug("Received AuthTicket: " + Client.AuthTicket);
            }

            var responses = serverResponse.Returns;
            if (responses != null)
            {
                var checkChallengeResponse = new CheckChallengeResponse();
                if (2 <= responses.Count)
                {
                    checkChallengeResponse.MergeFrom(responses[1]);

                    CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
                }

                var getInventoryResponse = new GetInventoryResponse();
                if (4 <= responses.Count)
                {
                    getInventoryResponse.MergeFrom(responses[3]);

                    CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);
                }

                var downloadSettingsResponse = new DownloadSettingsResponse();
                if (6 <= responses.Count)
                {
                    downloadSettingsResponse.MergeFrom(responses[5]);

                    CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);
                }
            }

        }

        public async Task FireRequestBlockTwo()
        {
            await FireRequestBlock(CommonRequest.GetGetAssetDigestMessageRequest(Client)).ConfigureAwait(false);
        }
    }
}