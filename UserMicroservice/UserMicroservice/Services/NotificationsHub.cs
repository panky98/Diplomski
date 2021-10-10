using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Models.UserMicroservice;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserMicroservice.Repositories;

namespace UserMicroservice.Services
{
    [Authorize]
    public class NotificationsHub:Hub
    {
        readonly RedisClient redis = new RedisClient("redis-api", 6379);
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationsHub> _logger;
        public NotificationsHub(IUnitOfWork unitOfWork,ILogger<NotificationsHub> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        public async Task EventCreated()
        {

        }
        public override Task OnConnectedAsync()
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);
            _logger.LogInformation($"User with id{idClaim.Value} has connected using SignalR");

            redis.SetEntryInHash("allActiveUsers", idClaim.Value, Context.ConnectionId);

            IList<InterestByUser> allInterests = _unitOfWork.InterestsByUsers.GetAll().Where(x => x.UserId == Int32.Parse(idClaim.Value)).ToList();
            foreach (var interest in allInterests)
            {
                redis.AddItemToList("interest:" + interest.InterestId, Context.ConnectionId);
                _logger.LogInformation($"User with id{idClaim.Value} has been added to list {"interest:"+interest.InterestId} for interest with id {interest.InterestId}, currently inside:{redis.GetListCount("interest:" + interest.InterestId)}");
            }
             return base.OnConnectedAsync();
        }

        public override  Task OnDisconnectedAsync(Exception exception)
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);
            _logger.LogInformation($"User with id{idClaim.Value} has been disconected from SignalR");

            redis.RemoveEntryFromHash("allActiveUsers", idClaim.Value);


            IList<InterestByUser> allInterests = _unitOfWork.InterestsByUsers.GetAll().Where(x => x.UserId == Int32.Parse(idClaim.Value)).ToList();
            foreach (var interest in allInterests)
            {
                redis.RemoveItemFromList("interest:" + interest.InterestId, Context.ConnectionId);
                _logger.LogInformation($"User with id{idClaim.Value} has been removed from list {"interest:" + interest.InterestId} for interest with id {interest.InterestId}, currently inside:{redis.GetListCount("interest:" + interest.InterestId)}");
            }
            return base.OnDisconnectedAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            this._unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
